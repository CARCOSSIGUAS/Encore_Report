using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Repositories;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Belcorp.Encore.Entities.Constants;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.Mongo;

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
        public int PeriodId { get; set; }
        public List<CalculationTypes> CalculationTypes { get; set; }
        public List<SponsorTree> Accounts_UpLine { get; set; }

        public ProcessOnlineMlmService
        (
            IUnitOfWork<EncoreCore_Context> _unitOfWork_Core, 
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm, 
            IProcessOnlineRepository _processOnlineRepository, 
            IAccountKPIsRepository _accountKPIsRepository, 
            IAccountInformationRepository _accountsInformationRepository
        )
        {
            unitOfWork_Core = _unitOfWork_Core;
            unitOfWork_Comm = _unitOfWork_Comm;
            encoreMongo_Context = new EncoreMongo_Context();

            processOnlineRepository = _processOnlineRepository;
            accountKPIsRepository = _accountKPIsRepository;
            accountsInformationRepository = _accountsInformationRepository;
            CalculationTypes = GetCalculationTypesByCode();
        }

        public void ProcessMLM_Order(int _orderId)
        {
            IRepository<Orders> ordersRepository = unitOfWork_Core.GetRepository<Orders>();
            Order = ordersRepository.GetFirstOrDefault(o => o.OrderID == _orderId, null, null, true);

            if (Order == null) {
                return;
            }

            var existsorderCalculationOnline = processOnlineRepository.GetExists_OrderCalculationsOnline(Order.OrderID);

            if (
                    (
                        Order.OrderStatusID == (short)Constants.OrderStatus.Paid ||
                        Order.OrderStatusID == (short)Constants.OrderStatus.Printed ||
                        Order.OrderStatusID == (short)Constants.OrderStatus.Shipped ||
                        Order.OrderStatusID == (short)Constants.OrderStatus.Invoiced ||
                        Order.OrderStatusID == (short)Constants.OrderStatus.Embarked
                    ) && !existsorderCalculationOnline
                )
            {

                PeriodId = GetPeriodId();
                Accounts_UpLine = GetAccounts_UpLine(Order.AccountID);

                decimal QV, CV, RV;
                QV = processOnlineRepository.GetQV_ByOrder(Order.OrderID);
                CV = Order.CommissionableTotal == null ? 0 : (decimal)Order.CommissionableTotal;
                RV = Order.Subtotal == null ? 0 : (decimal)Order.Subtotal;

                if (Order.OrderTypeID == (short)Constants.OrderType.ReturnOrder)
                {
                    QV = QV * -1;
                    CV = CV * -1;
                    RV = RV * -1;
                }

                Indicadores_InPersonal(QV, CV, RV);
                Indicadores_InDivision(QV, CV, RV);
                Indicadores_InTableReports(QV, CV, RV);
                Indicadores_InOrdercalculationsOnline(QV, CV, RV);
                Migrate_AccountInformationByAccountId();
            }
        }

        public void ProcessMLM_Lote(int loteId)
        {
            IRepository<MonitorLotes> monitorLotesRepository = unitOfWork_Core.GetRepository<MonitorLotes>();
            IRepository<MonitorOrders> monitorOrdersRepository = unitOfWork_Core.GetRepository<MonitorOrders>();


            int total = monitorOrdersRepository.Count(o => o.LoteId == loteId);
            if (total > 0)
            {
                var ordenes = monitorOrdersRepository.GetPagedList(l => l.LoteId == loteId && l.Process == false, null, null, 0, total, false).Items;

                foreach (var order in ordenes)
                {
                    try
                    {
                        ProcessMLM_Order(order.OrderId);
                        order.Process = true;
                        order.DateProcess = DateTime.Now;

                        unitOfWork_Core.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                    }
                }

                int ordenes_pendientes = monitorLotesRepository.Count(l => l.LoteId == loteId && l.MonitorOrders.Any(o => o.Process == false));
                if (ordenes_pendientes == 0)
                {
                    var lote = monitorLotesRepository.GetFirstOrDefault(l => l.LoteId == loteId, null, null, false);
                    lote.DateProcess = DateTime.Now;
                    lote.Process = true;
                    unitOfWork_Core.SaveChanges();
                }
            }
        }

        #region Metodos

        public List<CalculationTypes> GetCalculationTypesByCode()
        {
            IRepository<CalculationTypes> calculationTypesRepository = unitOfWork_Comm.GetRepository<CalculationTypes>();
            var result = calculationTypesRepository.GetAll();
            return result == null ? null : result.ToList();
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
            accountKPIsRepository.GetPagedList(a => accountsIds.Contains(a.AccountID) && a.PeriodID == PeriodId && a.CalculationTypeID == calculationType, null, null, 0, accountsIds.Count, false)
                                    .Items.
                                    ToList().ForEach(a =>
                                    {
                                        a.Value += value;
                                        a.DateModified = DateTime.Now;
                                        accountKPIsRepository.Update(a);
                                    });

            unitOfWork_Comm.SaveChanges();
        }

        public int GetPeriodId()
        {
            IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();
            var result = periodsRepository.GetFirstOrDefault(p => Order.CompleteDateUTC >= p.StartDateUTC && Order.CompleteDateUTC <= p.EndDateUTC && p.PlanID == 1, null, null, true);
            return result.PeriodID;
        }

        #endregion

        #region Calculos Personales
        public void Indicadores_InPersonal(decimal QV, decimal CV, decimal RV)
        {
            int calculationType_PQV = CalculationTypes.Where(c => c.Code == "PQV").FirstOrDefault().CalculationTypeID;
            int calculationType_PCV = CalculationTypes.Where(c => c.Code == "PCV").FirstOrDefault().CalculationTypeID;
            int calculationType_PRV = CalculationTypes.Where(c => c.Code == "PRV").FirstOrDefault().CalculationTypeID;

            IndicadoresInPersonal_UpdateValue(calculationType_PQV, QV);
            IndicadoresInPersonal_UpdateValue(calculationType_PCV, CV);
            IndicadoresInPersonal_UpdateValue(calculationType_PRV, RV);
        }

        public void IndicadoresInPersonal_UpdateValue(int calculationType, decimal value)
        {
            var result = accountKPIsRepository.GetFirstOrDefault(a => a.AccountID == Order.AccountID && a.PeriodID == PeriodId && a.CalculationTypeID == calculationType, null, null, false);

            if (result != null)
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
                            PeriodID = PeriodId,
                            CalculationTypeID = calculationType,
                            Value = value,
                            DateModified = DateTime.Now
                        });
            }

            unitOfWork_Comm.SaveChanges();
        }

        #endregion

        #region Calculos Divison
        public void Indicadores_InDivision(decimal QV, decimal CV, decimal RV)
        {
            IndicadoresInDivision_Initialize();
            if (Order.OrderTypeID != (short)Constants.OrderType.ReturnOrder)
            {
                IndicadoresInDivision_UpdateValue(QV, CV, RV);
            }
        }

        public void IndicadoresInDivision_Initialize()
        {
            List<string> codigos = new List<string> { "PQV", "PRV", "PCV", "GQV", "GCV", "DQV", "DCV", "CQL", "DQVT" };
            var calculationTypesIds = CalculationTypes.Where(c => codigos.Contains(c.Code)).ToList();

            var result = processOnlineRepository.GetListAccounts_Initialize(Accounts_UpLine.Select(a => a.AccountID).ToList(), calculationTypesIds, PeriodId);
            accountKPIsRepository.Insert(result);

            unitOfWork_Comm.SaveChanges();
        }

        private void IndicadoresInDivision_UpdateValue(decimal QV, decimal CV, decimal RV)
        {
            var listAccounts = Accounts_UpLine.Select(a => a.AccountID).ToList();

            int calculationType_DCV =  CalculationTypes.Where(c => c.Code == "DCV").FirstOrDefault().CalculationTypeID;
            int calculationType_DQVT = CalculationTypes.Where(c => c.Code == "DQVT").FirstOrDefault().CalculationTypeID;

            Indicadores_UpdateValue_AccountKPIs(listAccounts, calculationType_DQVT, value: QV);
            Indicadores_UpdateValue_AccountKPIs(listAccounts, calculationType_DCV,  value: CV);

            IndicadoresInDivision_Valida_Porcentaje(Accounts_UpLine, QV);
        }
        
        public void IndicadoresInDivision_Valida_Porcentaje(List<SponsorTree> accounts, decimal QV)
        {
            List<string> codigos = new List<string> { "PQV", "DQV", "DQVT" };
            int currentAccountID, porcentForRuler, titleForRuler;
            int? currentAccountTitle;
            decimal DQV_Result;

            var calculationTypesIds =   CalculationTypes.Where(c => codigos.Contains(c.Code)).ToList();
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
                        DQV_Result = processOnlineRepository.GetQV_ByAccount_PorcentRuler(currentAccountID, PeriodId, calculationTypesIds, porcentForRuler);

                        Indicadores_UpdateValue_AccountKPIs(new List<int> { currentAccountID }, calculationType_DQV, value: DQV_Result);
                    }
                    else
                        Indicadores_UpdateValue_AccountKPIs(new List<int> { currentAccountID }, calculationType_DQV, value: QV);
                }
            }

        }
        #endregion

        #region Calculos OrdercalculationsOnline
        public void Indicadores_InOrdercalculationsOnline(decimal QV, decimal CV, decimal RV)
        {
            List<string> codigos = new List<string> { "QV", "CV", "RP" };
            var calculationTypesIds = GetOrderCalculationTypesByCode(codigos);

            IRepository<Entities.Entities.Core.Accounts> accountsRepository = unitOfWork_Core.GetRepository<Entities.Entities.Core.Accounts>();
            var account = accountsRepository.GetFirstOrDefault(a => a.AccountID == Order.AccountID, null, null, true);

            int orderCalculationTypeID_QV = calculationTypesIds.Where(c => c.Code == "QV").FirstOrDefault().OrderCalculationTypeID;
            int orderCalculationTypeID_CV = calculationTypesIds.Where(c => c.Code == "CV").FirstOrDefault().OrderCalculationTypeID;
            int orderCalculationTypeID_RP = calculationTypesIds.Where(c => c.Code == "RP").FirstOrDefault().OrderCalculationTypeID;

            OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_QV, QV, account.AccountTypeID);
            OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_CV, CV, account.AccountTypeID);
            OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_RP, RV, account.AccountTypeID);
        }

        public void OrdercalculationsOnline_UpdateValue(int calculationType, decimal value, int accountTypeId)
        {
            IRepository<OrderCalculationsOnline> orderCalculationOnlineRepository = unitOfWork_Comm.GetRepository<OrderCalculationsOnline>();

            var result = orderCalculationOnlineRepository.GetFirstOrDefault(o => o.OrderID == Order.OrderID && o.OrderCalculationTypeID == calculationType, null, null, false);

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
                            AccountTypeID = accountTypeId,
                            OrderTypeID = Order.OrderTypeID,
                            DateModifiedUTC = DateTime.Now
                        });
            }

            unitOfWork_Comm.SaveChanges();
        }
        #endregion

        #region Actualizar Reportes
        public void Indicadores_InTableReports(decimal QV, decimal CV, decimal RV)
        {
            int calculationType_DQV = CalculationTypes.Where(c => c.Code == "DQV").FirstOrDefault().CalculationTypeID;

            var accountInformation_Current = accountsInformationRepository.GetFirstOrDefault(a => a.AccountID == Order.AccountID && a.PeriodID == PeriodId, null, null, false);

            accountInformation_Current.PCV += CV;
            accountInformation_Current.PQV += QV;
            accountsInformationRepository.Update(accountInformation_Current);

            unitOfWork_Comm.SaveChanges();

            var listAccounts = Accounts_UpLine.Select(a => a.AccountID);
            var accountInformations = accountsInformationRepository.GetPagedList(a => a.PeriodID == PeriodId && listAccounts.Contains(a.AccountID), null, null, 0, Accounts_UpLine.Count, false);

            IRepository<AccountKPIsDetails> accountKPIsDetailsRepository = unitOfWork_Comm.GetRepository<AccountKPIsDetails>();
            var listKpisCode = new List<string>(new[] { "DQV", "GQV", "DCV", "GCV" });

            foreach (var accountInformation_Up in accountInformations.Items)
            {
                var result_accountKPIs = accountKPIsRepository.GetFirstOrDefault(akp => akp.AccountID == accountInformation_Up.AccountID && akp.PeriodID == PeriodId && akp.CalculationTypeID == calculationType_DQV, null, null, true);

                accountInformation_Up.DQV = result_accountKPIs.Value;
                accountInformation_Up.DCV += CV;
                accountInformation_Up.DQVT += QV;
                accountsInformationRepository.Update(accountInformation_Up);

                unitOfWork_Comm.SaveChanges();

                foreach (var kpiCode in listKpisCode)
                {
                    var accountKPIsDetails = accountKPIsDetailsRepository.GetFirstOrDefault(a => a.PeriodID == PeriodId &&
                                                                                                        a.SponsorID == accountInformation_Up.AccountID &&
                                                                                                        a.DownlineID == accountInformation_Current.AccountID &&
                                                                                                        a.KPICode == kpiCode,
                                                                                                    null, null, false
                                                                                                  );
                    if (accountKPIsDetails == null)
                    {
                        accountKPIsDetailsRepository.Insert(
                                new AccountKPIsDetails
                                {
                                    PeriodID = PeriodId,
                                    SponsorID = accountInformation_Up.AccountID,
                                    SponsorName = accountInformation_Up.AccountName,
                                    DownlineID = accountInformation_Current.AccountID,
                                    DownlineName = accountInformation_Current.AccountName,
                                    KPICode = kpiCode,
                                    Value = ((kpiCode == "DQV" || kpiCode == "GQV") ? QV : CV),
                                    Percentage = 1,
                                    DownlinePaidAsTitle = null,
                                    TreeLevel = accountInformation_Up.LEVEL
                                }
                            );
                    }
                    else
                    {

                        accountKPIsDetails.Value = accountKPIsDetails.Value + ((kpiCode == "DQV" || kpiCode == "GQV") ? QV : CV);
                        accountKPIsDetails.Percentage = 1;
                        accountKPIsDetails.DownlinePaidAsTitle = null;
                        accountKPIsDetails.TreeLevel = accountInformation_Current.LEVEL;
                        accountKPIsDetails.DateModified = DateTime.Now;
                    }

                    unitOfWork_Comm.SaveChanges();
                }
            }
        }

        #endregion

        #region Actualizar Mongo
        public void Migrate_AccountInformationByAccountId()
        {
            IRepository<Titles> titlesRepository = unitOfWork_Comm.GetRepository<Titles>();
            var titles = titlesRepository.GetAll().ToList();

            var accountsId = GetAccounts_UpLine(Order.AccountID).Select(a => a.AccountID).ToList();
            var accountsInformation = accountsInformationRepository.GetListAccountInformationByPeriodIdAndAccountId(PeriodId, accountsId);

            var result = from ai in accountsInformation
                         join titlesInfo_Career in titles on Int32.Parse(ai.CareerTitle) equals titlesInfo_Career.TitleID
                         join titlesInfo_Paid in titles on Int32.Parse(ai.PaidAsCurrentMonth) equals titlesInfo_Paid.TitleID
                         select new AccountsInformation_Mongo
                         {
                             CountryID = 0,
                             AccountsInformationID = ai.AccountsInformationID,

                             PeriodID = ai.PeriodID,
                             AccountID = ai.AccountID,
                             AccountNumber = ai.AccountNumber,
                             AccountName = ai.AccountName,
                             SponsorID = ai.SponsorID,
                             SponsorName = ai.SponsorName,
                             Address = ai.Address,
                             PostalCode = ai.PostalCode,
                             City = ai.City,
                             STATE = ai.STATE,

                             PQV = ai.PQV,
                             DQV = ai.DQV,
                             DQVT = ai.DQVT,

                             CareerTitle = ai.CareerTitle,
                             PaidAsCurrentMonth = ai.PaidAsCurrentMonth,
                             CareerTitle_Des = titlesInfo_Career.Name,
                             PaidAsCurrentMonth_Des = titlesInfo_Paid.Name,

                             JoinDate = ai.JoinDate,
                             Generation = ai.Generation,
                             LEVEL = ai.LEVEL,
                             SortPath = ai.SortPath,
                             LeftBower = ai.LeftBower,
                             RightBower = ai.RightBower,
                             Activity = ai.Activity
                         };

            foreach (var item in result)
            {
                var item_Mongo = encoreMongo_Context.AccountsInformationProvider.Find(ai => ai.CountryID == 0 && ai.PeriodID == PeriodId && ai.AccountID == item.AccountID).FirstOrDefault();
                if (item_Mongo != null)
                {
                    item.Id = item_Mongo.Id;
                    encoreMongo_Context.AccountsInformationProvider.ReplaceOne(ai => ai.CountryID == 0 && ai.PeriodID == PeriodId && ai.AccountID == item.AccountID, item, new UpdateOptions { IsUpsert = true });
                }
                else
                {
                    encoreMongo_Context.AccountsInformationProvider.InsertOne(item);
                }
            }
        }
        #endregion
    }
}
