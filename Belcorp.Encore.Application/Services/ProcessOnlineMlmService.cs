using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class ProcessOnlineMlmService : IProcessOnlineMlmService
    {
        private readonly IUnitOfWork<EncoreCore_Context> _unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm;

        private readonly EncoreMongo_Context _encoreMongo_Context;

        private readonly IProcessOnlineRepository _processOnlineRepository;
        private readonly IAccountKPIsRepository _accountKPIsRepository;
        private readonly IAccountInformationRepository _accountsInformationRepository;

        public int accountId { get; set; }
        public int periodId { get; set; }
        public int orderId { get; set; }

        public ProcessOnlineMlmService(IUnitOfWork<EncoreCore_Context> unitOfWork_Core, IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm, IProcessOnlineRepository processOnlineRepository, IAccountKPIsRepository accountKPIsRepository, IAccountInformationRepository accountsInformationRepository)
        {
            _unitOfWork_Core = unitOfWork_Core;
            _unitOfWork_Comm = unitOfWork_Comm;
            _processOnlineRepository = processOnlineRepository;
            _accountKPIsRepository = accountKPIsRepository;
            _accountsInformationRepository = accountsInformationRepository;
        }

        public void ProcessMLM(int _periodId, int _accountId, int _orderId)
        {
            IRepository<Orders> ordersRepository = _unitOfWork_Core.GetRepository<Orders>();
            var order = ordersRepository.GetFirstOrDefault(o => o.OrderID == orderId, null, null, true);

            periodId = _periodId;
            accountId = _accountId;
            orderId = _orderId;

            KPIsInGroup_Process();
        }

        #region Metodos
        public List<CalculationTypes> GetCalculationTypesByCode(List<string> codigos)
        {
            IRepository<CalculationTypes> calculationTypesRepository = _unitOfWork_Comm.GetRepository<CalculationTypes>();
            var result = calculationTypesRepository.GetPagedList(c => codigos.Contains(c.Code), null, null, 0, codigos.Count, true);
            return result == null ? null : result.Items.ToList();
        }

        private List<OrderCalculationTypes> GetOrderCalculationTypesByCode(List<string> codigos)
        { 
            IRepository<OrderCalculationTypes> orderCalculationTypesRepository = _unitOfWork_Comm.GetRepository<OrderCalculationTypes>();
            var result = orderCalculationTypesRepository.GetPagedList(c => codigos.Contains(c.Code), null, null, 0, codigos.Count, true);
            return result == null ? null : result.Items.ToList();
        }

        public List<SponsorTree> GetAccounts_Group(int accountId)
        {
            IRepository<SponsorTree> sponsorTreeRepository = _unitOfWork_Comm.GetRepository<SponsorTree>();
            var result = sponsorTreeRepository.FromSql($"SELECT * FROM [fnGetAccounts_InGroup_Aux] ({accountId})").ToList();
            return result == null ? null : result.ToList();
        }

        public List<SponsorTree> GetAccounts_UpLine(int accountId)
        {
            IRepository<SponsorTree> sponsorTreeRepository = _unitOfWork_Comm.GetRepository<SponsorTree>();
            var result = sponsorTreeRepository.FromSql($"SELECT * FROM [fnGetAccount_Upline_Aux] ({accountId})").ToList();
            return result == null ? null : result.ToList();
        }

        private void KPIs_UpdateValue(List<int> accountsIds, int calculationType, decimal value)
        {
            _accountKPIsRepository.GetPagedList(a => accountsIds.Contains(a.AccountID) && a.PeriodID == periodId && a.CalculationTypeID == calculationType, null, null, 0, accountsIds.Count, true)
                                    .Items.
                                    ToList().ForEach(a =>
                                    {
                                        a.Value += value;
                                        a.DateModified = DateTime.Now;
                                        _accountKPIsRepository.Update(a);
                                    });
        }


        #endregion

        #region Calculos Personales
        public void KPIsInPersonal_Process(decimal QV, decimal CV, decimal RV)
        {
            List<string> codigos = new List<string> { "PQV", "PRV", "PCV" };
            var calculationTypesIds = GetCalculationTypesByCode(codigos);

            int calculationType_PQV = calculationTypesIds.Where(c => c.Code == "PQV").FirstOrDefault().CalculationTypeID;
            int calculationType_PRV = calculationTypesIds.Where(c => c.Code == "PRV").FirstOrDefault().CalculationTypeID;
            int calculationType_PCV = calculationTypesIds.Where(c => c.Code == "PCV").FirstOrDefault().CalculationTypeID;

            using (_unitOfWork_Comm)
            {
                KPIsInPersonal_UpdateValue(calculationType_PQV, QV);
                KPIsInPersonal_UpdateValue(calculationType_PRV, RV);
                KPIsInPersonal_UpdateValue(calculationType_PCV, CV);

                KPIsInPersonal_ForReports(QV, CV);

                _unitOfWork_Comm.SaveChanges();
            }
        }

        public void KPIsInPersonal_UpdateValue(int calculationType, decimal value)
        {
            int total = _accountKPIsRepository.Count(a => a.AccountID == accountId && a.PeriodID == periodId && a.CalculationTypeID == calculationType);

            if (total > 0)
            {
                List<int> accountsIds = new List<int> { accountId };
                KPIs_UpdateValue(accountsIds, calculationType, value);
            }
            else
            {
                _accountKPIsRepository.Insert(
                        new AccountKPIs
                        {
                            AccountID = accountId,
                            PeriodID = periodId,
                            CalculationTypeID = calculationType,
                            Value = value,
                            DateModified = DateTime.Now
                        });
            }
        }

        public void KPIsInPersonal_ForReports(decimal QV, decimal CV)
        {
            var result_account = _accountsInformationRepository.GetFirstOrDefault(a => a.AccountID == accountId && a.PeriodID == periodId, null, null, true);
            result_account.PQV += QV;
            result_account.PCV += CV;
            _accountsInformationRepository.Update(result_account);
        }

        public void KPIsInPersonal_ForReports(List<SponsorTree> accounts, decimal QV, decimal CV)
        {
            var lista = accounts.Select(x => x.AccountID);
            var accountInformations = _accountsInformationRepository.GetPagedList(x => lista.Contains(x.AccountID), null, null, 0, accounts.Count, true);

            foreach (var accountInformation in accountInformations.Items)
            {
                accountInformation.PQV += QV;
                accountInformation.PCV += CV;
                _accountsInformationRepository.Update(accountInformation);
            }

           

        }

        #endregion

        #region Calculos Divison
        public void KPIsInDivision_Process()
        {
            List<string> codigos = new List<string> { "PQV", "PRV", "PCV", "GQV", "GCV", "DQV", "DCV", "CQL", "DQVT" };
            var calculationTypesIds = GetCalculationTypesByCode(codigos);

            var accounts = GetAccounts_UpLine(accountId).ToList();

            using (_unitOfWork_Comm)
            {
                var accountsIds = accounts.Select(a => a.AccountID).ToList();
                KPIsInDivision_Initialize(accountsIds, calculationTypesIds);
                KPIsInDivision_Online(accountsIds, QV: 0, CV: 0);
                _unitOfWork_Comm.SaveChanges();
            }
        }

        public void KPIsInDivision_Initialize(List<int> accountsIds, List<CalculationTypes> calculationTypesIds)
        {
            var result = _processOnlineRepository.GetListAccounts_Initialize(accountsIds, calculationTypesIds, periodId);
            _accountKPIsRepository.Insert(result);
        }

        private void KPIsInDivision_Online(List<int> accountsIds, decimal QV, decimal CV)
        {
            List<string> codigos = new List<string> { "DCV", "DQVT" };
            var calculationTypesIds = GetCalculationTypesByCode(codigos);

            int calculationType_DCV = calculationTypesIds.Where(c => c.Code == "DCV").FirstOrDefault().CalculationTypeID;
            int calculationType_DQVT = calculationTypesIds.Where(c => c.Code == "DQVT").FirstOrDefault().CalculationTypeID;
            int calculationType_DQV = calculationTypesIds.Where(c => c.Code == "DQV").FirstOrDefault().CalculationTypeID;

            KPIs_UpdateValue(accountsIds, calculationType_DCV, value: 0);
            KPIs_UpdateValue(accountsIds, calculationType_DQVT, value: 0);
        }
        
        public void KPIsInDivision_Porcentaje(List<SponsorTree> accounts)
        {
            List<string> codigos = new List<string> { "PQV", "DQVT" };
            int currentAccountID, porcent, titleForDQV;
            int? currentAccountPAT;
            decimal dqvFinal;

            var calculationTypesIds = GetCalculationTypesByCode(codigos);

            int calculationType_PQV = calculationTypesIds.Where(c => c.Code == "PQV").FirstOrDefault().CalculationTypeID;
            int calculationType_DQVT = calculationTypesIds.Where(c => c.Code == "DQVT").FirstOrDefault().CalculationTypeID;
            int calculationType_DQV = calculationTypesIds.Where(c => c.Code == "DQV").FirstOrDefault().CalculationTypeID;


            IRepository<RuleTypes> ruleTypesRepository = _unitOfWork_Comm.GetRepository<RuleTypes>();
            
            var ruleType = ruleTypesRepository.GetFirstOrDefault(oc => oc.Name == "VolumenDivision" && oc.Active == true, null, null, false);

            if (ruleType != null)
            {
                titleForDQV = int.Parse(ruleType.RequirementRules.Value1);
                porcent = int.Parse(ruleType.RequirementRules.Value2);

                foreach (var account in accounts)
                {
                    currentAccountID = account.AccountID;
                    currentAccountPAT = account.CurrentPAT;

                    if (currentAccountPAT >= titleForDQV)
                    {
                        dqvFinal = _processOnlineRepository.GetDQV_Online(currentAccountID, calculationType_DQVT, calculationTypesIds, porcent);
                        KPIs_UpdateValue(new List<int> { currentAccountID }, calculationType_DQV, value: dqvFinal);
                    }
                    else
                        KPIs_UpdateValue(new List<int> { currentAccountID }, calculationType_DQV, value: 0);
                }
            }

        }

        #endregion

        #region Calculos Grupo

        public void KPIsInGroup_Process() {

            List<string> codigos_ct = new List<string> { "PQV", "PRV", "PCV", "GQV", "GCV", "DQV", "DCV", "CQL" };
            var calculationTypesIds = GetCalculationTypesByCode(codigos_ct);

            int calculationType_PQV = calculationTypesIds.Where(c => c.Code == "PQV").FirstOrDefault().CalculationTypeID;
            int calculationType_PCV = calculationTypesIds.Where(c => c.Code == "PCV").FirstOrDefault().CalculationTypeID;

            List<string> codigos_oct = new List<string> { "QV", "CV" };
            var orderCalculationTypesIds = GetOrderCalculationTypesByCode(codigos_oct);

            int orderCalculationType_QV = orderCalculationTypesIds.Where(c => c.Code == "QV").FirstOrDefault().OrderCalculationTypeID;
            int orderCalculationType_CV = orderCalculationTypesIds.Where(c => c.Code == "CV").FirstOrDefault().OrderCalculationTypeID;

            List<int> accountsIds = GetAccounts_Group(accountId).Select(a => a.AccountID).ToList();

            using (_unitOfWork_Comm)
            {
                KPIsInGroup_Initialize(accountsIds, calculationTypesIds);

                KPIsInGroup_Online(accountsIds, calculationType_PQV, orderCalculationType_QV);
                KPIsInGroup_Online(accountsIds, calculationType_PCV, orderCalculationType_CV);

                KPIsInGroup_ForReports(accountsIds, QV: 0, CV: 0);

                _unitOfWork_Comm.SaveChanges();
            }
        }

        public void KPIsInGroup_Initialize(List<int> accountsIds, List<CalculationTypes> calculationTypesIds)
        {
            var result_InitializesKPIsInGroup = _processOnlineRepository.GetListAccounts_Initialize(accountsIds, calculationTypesIds, periodId);
            _accountKPIsRepository.Insert(result_InitializesKPIsInGroup);
        }

        public void KPIsInGroup_Online(List<int> accountsIds, int calculationType, int orderCalculationTypeID)
        {
            IRepository<OrderCalculationsOnline> orderCalculationsOnlineRepository = _unitOfWork_Comm.GetRepository<OrderCalculationsOnline>();
            var value = orderCalculationsOnlineRepository.GetFirstOrDefault(oc => oc.AccountID == accountId && oc.OrderID == orderId && oc.OrderCalculationTypeID == orderCalculationTypeID, null, null, false).Value;
            KPIs_UpdateValue(accountsIds, calculationType, value);
        }

        public void KPIsInGroup_ForReports(List<int> accountsIds, decimal QV, decimal CV)
        {
            _accountsInformationRepository.GetPagedList(a => accountsIds.Contains(a.AccountID) && a.PeriodID == periodId, null, null, 0, accountsIds.Count, true)
                                    .Items.
                                    ToList().ForEach(a =>
                                    {
                                        a.GQV += QV;
                                        a.GCV += CV;
                                        _accountsInformationRepository.Update(a);
                                    });
        }
        #endregion
    }
}
