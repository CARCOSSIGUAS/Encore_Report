using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Belcorp.Encore.Entities.Constants;

namespace Belcorp.Encore.Application.Services
{
    public class ProcessOnlineMlmService : IProcessOnlineMlmService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;
        private readonly EncoreMongo_Context encoreMongo_Context;


        private readonly IProcessOnlineRepository processOnlineRepository;
        private readonly IAccountKPIsRepository accountKPIsRepository;
        private readonly IAccountInformationRepository accountsInformationRepository;

        public Orders Order { get; set; }
        public int periodId { get; set; }

        public ProcessOnlineMlmService(IUnitOfWork<EncoreCore_Context> _unitOfWork_Core, IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm, IProcessOnlineRepository _processOnlineRepository, IAccountKPIsRepository _accountKPIsRepository, IAccountInformationRepository _accountsInformationRepository)
        {
            unitOfWork_Core = _unitOfWork_Core;
            unitOfWork_Comm = _unitOfWork_Comm;
            encoreMongo_Context = new EncoreMongo_Context();

            processOnlineRepository = _processOnlineRepository;
            accountKPIsRepository = _accountKPIsRepository;
            accountsInformationRepository = _accountsInformationRepository;
        }

        public void ProcessMLM(int _orderId)
        {
            IRepository<Orders> ordersRepository = unitOfWork_Core.GetRepository<Orders>();
            Order = ordersRepository.GetFirstOrDefault(o => o.OrderID == _orderId, null, null, true);

            if (Order == null) {
                return;
            }

            periodId = (int)Order.CompletedPeriodID;

            decimal QV, CV, RV;
            QV = processOnlineRepository.GetQV_ByOrder(Order.OrderID);
            CV = Order.CommissionableTotal == null ? 0 : (decimal)Order.CommissionableTotal;
            RV = Order.Subtotal == null ? 0 : (decimal)Order.Subtotal;

            IRepository<OrderCalculationsOnline> orderCalculationOnlineRepository = unitOfWork_Comm.GetRepository<OrderCalculationsOnline>();
            int existsorderCalculationOnline =  orderCalculationOnlineRepository.Count(o => o.OrderID == Order.OrderID);

            if (existsorderCalculationOnline == 0)
            {
                IndicadoresInPersonal_Process(QV, CV, RV);
                IndicadoresInDivision_Process(QV, CV, RV);
                OrdercalculationsOnline_Process(QV, CV, RV);
                Migrate_AccountInformationByAccountId();
            }
        }

        #region Metodos

        public List<CalculationTypes> GetCalculationTypesByCode(List<string> codigos)
        {
            IRepository<CalculationTypes> calculationTypesRepository = unitOfWork_Comm.GetRepository<CalculationTypes>();
            var result = calculationTypesRepository.GetPagedList(c => codigos.Contains(c.Code), null, null, 0, codigos.Count, true);
            return result == null ? null : result.Items.ToList();
        }

        private List<OrderCalculationTypes> GetOrderCalculationTypesByCode(List<string> codigos)
        { 
            IRepository<OrderCalculationTypes> orderCalculationTypesRepository = unitOfWork_Comm.GetRepository<OrderCalculationTypes>();
            var result = orderCalculationTypesRepository.GetPagedList(c => codigos.Contains(c.Code), null, null, 0, codigos.Count, true);
            return result == null ? null : result.Items.ToList();
        }

        public List<SponsorTree> GetAccounts_UpLine(int accountId)
        {
            IRepository<SponsorTree> sponsorTreeRepository = unitOfWork_Comm.GetRepository<SponsorTree>();
            var result = sponsorTreeRepository.FromSql($"SELECT * FROM [fnGetAccount_Upline_Aux] ({accountId})").ToList();
            return result == null ? null : result.ToList();
        }

        private void Indicadores_UpdateValue_AccountKPIs(List<int> accountsIds, int calculationType, decimal value)
        {
            accountKPIsRepository.GetPagedList(a => accountsIds.Contains(a.AccountID) && a.PeriodID == periodId && a.CalculationTypeID == calculationType, null, null, 0, accountsIds.Count, false)
                                    .Items.
                                    ToList().ForEach(a =>
                                    {
                                        a.Value += value;
                                        a.DateModified = DateTime.Now;
                                        accountKPIsRepository.Update(a);
                                    });

            unitOfWork_Comm.SaveChanges();
        }


        #endregion

        #region Calculos Personales
        public void IndicadoresInPersonal_Process(decimal QV, decimal CV, decimal RV)
        {
            List<string> codigos = new List<string> { "PQV", "PCV", "PRV" };
            var calculationTypesIds = GetCalculationTypesByCode(codigos);

            int calculationType_PQV = calculationTypesIds.Where(c => c.Code == "PQV").FirstOrDefault().CalculationTypeID;
            int calculationType_PCV = calculationTypesIds.Where(c => c.Code == "PCV").FirstOrDefault().CalculationTypeID;
            int calculationType_PRV = calculationTypesIds.Where(c => c.Code == "PRV").FirstOrDefault().CalculationTypeID;

            IndicadoresInPersonal_UpdateValue(calculationType_PQV, QV);
            IndicadoresInPersonal_UpdateValue(calculationType_PCV, CV);
            IndicadoresInPersonal_UpdateValue(calculationType_PRV, RV);
            
            IndicadoresInPersonal_UpdateValue_AccountsInformation(QV, CV, RV);

        }

        public void IndicadoresInPersonal_UpdateValue(int calculationType, decimal value)
        {
            var result = accountKPIsRepository.GetFirstOrDefault(a => a.AccountID == Order.AccountID && a.PeriodID == periodId && a.CalculationTypeID == calculationType, null, null, false);

            if (result == null)
            {
                result.Value += value;
                result.DateModified = DateTime.Now;
                accountKPIsRepository.Update(result);
            }
            else
            {
                accountKPIsRepository.Insert(
                        new AccountKPIs
                        {
                            AccountID = Order.AccountID,
                            PeriodID = periodId,
                            CalculationTypeID = calculationType,
                            Value = value,
                            DateModified = DateTime.Now
                        });
            }

            unitOfWork_Comm.SaveChanges();
        }

        public void IndicadoresInPersonal_UpdateValue_AccountsInformation(decimal QV, decimal CV, decimal RV)
        {
            accountsInformationRepository.GetPagedList(a => a.AccountID == Order.AccountID && a.PeriodID == periodId, null, null, 0, 1, false).Items.
                                                    ToList().ForEach(a =>
                                                    {
                                                        a.PQV += QV;
                                                        a.PCV += CV;
                                                        accountsInformationRepository.Update(a);
                                                    });

            unitOfWork_Comm.SaveChanges();
        }

        #endregion

        #region Calculos Divison
        public void IndicadoresInDivision_Process(decimal QV, decimal CV, decimal RV)
        {
            var accounts = GetAccounts_UpLine(Order.AccountID).ToList();

            var accountsIds = accounts.Select(a => a.AccountID).ToList();

            IndicadoresInDivision_Initialize(accountsIds);
            if (Order.OrderTypeID != (short)Constants.OrderType.ReturnOrder)
            {
                IndicadoresInDivision_UpdateValue(accounts, QV, CV, RV);
                IndicadoresInDivision_UpdateValue_AccountsInformation(accounts, QV, CV, RV);
            }
        }

        public void IndicadoresInDivision_Initialize(List<int> accountsIds)
        {
            List<string> codigos = new List<string> { "PQV", "PRV", "PCV", "GQV", "GCV", "DQV", "DCV", "CQL", "DQVT" };
            var calculationTypesIds = GetCalculationTypesByCode(codigos);

            var result = processOnlineRepository.GetListAccounts_Initialize(accountsIds, calculationTypesIds, periodId);
            accountKPIsRepository.Insert(result);

            unitOfWork_Comm.SaveChanges();
        }

        private void IndicadoresInDivision_UpdateValue(List<SponsorTree> accounts, decimal QV, decimal CV, decimal RV)
        {
            List<string> codigos = new List<string> { "DCV", "DQVT" };
            var accountsIds = accounts.Select(a => a.AccountID).ToList();

            var calculationTypesIds = GetCalculationTypesByCode(codigos);

            int calculationType_DCV =  calculationTypesIds.Where(c => c.Code == "DCV").FirstOrDefault().CalculationTypeID;
            int calculationType_DQVT = calculationTypesIds.Where(c => c.Code == "DQVT").FirstOrDefault().CalculationTypeID;

            Indicadores_UpdateValue_AccountKPIs(accountsIds, calculationType_DQVT, value: QV);
            Indicadores_UpdateValue_AccountKPIs(accountsIds, calculationType_DCV,  value: CV);

            IndicadoresInDivision_Valida_Porcentaje(accounts, QV);
        }
        
        public void IndicadoresInDivision_Valida_Porcentaje(List<SponsorTree> accounts, decimal QV)
        {
            List<string> codigos = new List<string> { "PQV", "DQV", "DQVT" };
            int currentAccountID, porcentForRuler, titleForRuler;
            int? currentAccountTitle;
            decimal DQV_Result;

            var calculationTypesIds = GetCalculationTypesByCode(codigos);
            int calculationType_DQV =   calculationTypesIds.Where(c => c.Code == "DQV").FirstOrDefault().CalculationTypeID;

            IRepository<RuleTypes> ruleTypesRepository = unitOfWork_Comm.GetRepository<RuleTypes>();
            
            var ruleType = ruleTypesRepository.GetFirstOrDefault(rt => rt.Name == "VolumenDivision" && rt.Active == true, null, rt => rt.Include(r => r.RequirementRules), true);

            if (ruleType != null)
            {
                porcentForRuler = int.Parse(ruleType.RequirementRules.Value1);
                titleForRuler = int.Parse(ruleType.RequirementRules.Value2);

                foreach (var account in accounts)
                {
                    currentAccountID  = account.AccountID;
                    currentAccountTitle = account.CurrentPAT;

                    if (currentAccountTitle >= titleForRuler)
                    {
                        DQV_Result = 0;
                        DQV_Result = processOnlineRepository.GetQV_ByAccount_PorcentRuler(currentAccountID, periodId, calculationTypesIds, porcentForRuler);

                        Indicadores_UpdateValue_AccountKPIs(new List<int> { currentAccountID }, calculationType_DQV, value: DQV_Result);
                    }
                    else
                        Indicadores_UpdateValue_AccountKPIs(new List<int> { currentAccountID }, calculationType_DQV, value: QV);
                }
            }

        }

        public void IndicadoresInDivision_UpdateValue_AccountsInformation(List<SponsorTree> accounts, decimal QV, decimal CV, decimal RV)
        {
            var calculationTypesIds = GetCalculationTypesByCode(new List<string> { "DQV" });
            int calculationType_DQV = calculationTypesIds.Where(c => c.Code == "DQV").FirstOrDefault().CalculationTypeID;

            var result_account = accountsInformationRepository.GetFirstOrDefault(a => a.AccountID == Order.AccountID && a.PeriodID == periodId, null, null, false);
            result_account.DCV += CV;
            result_account.DQVT += QV;
            accountsInformationRepository.Update(result_account);

            unitOfWork_Comm.SaveChanges();

            var lista = accounts.Select(a => a.AccountID);
            var accountInformations = accountsInformationRepository.GetPagedList(a => lista.Contains(a.AccountID), null, null, 0, accounts.Count, false);

            foreach (var accountInformation in accountInformations.Items)
            {
                var result = accountKPIsRepository.GetFirstOrDefault(akp => akp.AccountID == accountInformation.AccountID && akp.PeriodID == periodId && akp.CalculationTypeID == calculationType_DQV, null, null, true);
                accountInformation.DQV = result.Value;
                accountsInformationRepository.Update(accountInformation);
            }

            unitOfWork_Comm.SaveChanges();
        }


        #endregion

        #region Calculos OrdercalculationsOnline
        public void OrdercalculationsOnline_Process(decimal QV, decimal CV, decimal RV)
        {
            List<string> codigos = new List<string> { "QV", "CV", "RP" };
            var calculationTypesIds = GetOrderCalculationTypesByCode(codigos);

            int orderCalculationTypeID_QV = calculationTypesIds.Where(c => c.Code == "QV").FirstOrDefault().OrderCalculationTypeID;
            int orderCalculationTypeID_CV = calculationTypesIds.Where(c => c.Code == "CV").FirstOrDefault().OrderCalculationTypeID;
            int orderCalculationTypeID_RP = calculationTypesIds.Where(c => c.Code == "RP").FirstOrDefault().OrderCalculationTypeID;

            OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_QV, QV);
            OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_CV, CV);
            OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_RP, RV);
        }

        public void OrdercalculationsOnline_UpdateValue(int calculationType, decimal value)
        {
            IRepository<OrderCalculationsOnline> orderCalculationOnlineRepository = unitOfWork_Comm.GetRepository<OrderCalculationsOnline>();
            IRepository<Accounts> accountsRepository = unitOfWork_Comm.GetRepository<Accounts>();

            var result = orderCalculationOnlineRepository.GetFirstOrDefault(o => o.OrderID == Order.OrderID && o.OrderCalculationTypeID == calculationType, null, null, false);
            var account = accountsRepository.GetFirstOrDefault(a => a.AccountID == Order.AccountID, null, null, true);

            if (result != null)
            {
                result.Value = value;
                result.DateModifiedUTC = DateTime.Now;
                orderCalculationOnlineRepository.Update(result);
            }
            else
            {
                orderCalculationOnlineRepository.Insert(
                        new OrderCalculationsOnline
                        {
                            AccountID = Order.AccountID,
                            OrderID = Order.OrderID,
                            OrderCalculationTypeID = calculationType,
                            OrderStatusID = Order.OrderStatusID,
                            Value = value,
                            CalculationDateUTC = Order.CommissionDateUTC,
                            ParentOrderID = Order.ParentOrderID,
                            AccountTypeID = account.AccountTypeID,
                            OrderTypeID = Order.OrderTypeID,
                            DateModifiedUTC = DateTime.Now
                        });
            }

            unitOfWork_Comm.SaveChanges();
        }
        #endregion

        public void Migrate_AccountInformationByAccountId()
        {
            var accountsId = GetAccounts_UpLine(Order.AccountID).Select(a => a.AccountID).ToList();
            var result = accountsInformationRepository.GetListAccountInformationByPeriodIdAndAccountId(periodId, accountsId).ToList();

            encoreMongo_Context.AccountsInformationProvider.DeleteMany(ai => ai.PeriodID == periodId && accountsId.Contains(ai.AccountID));
            encoreMongo_Context.AccountsInformationProvider.InsertMany(result);
        }
    }
}
