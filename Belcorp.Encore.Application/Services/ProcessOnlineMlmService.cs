using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Application.OnlineMLM;
using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Application.Utilities;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Constants;
using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Repositories;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Belcorp.Encore.Application.Services
{
    public class ProcessOnlineMlmService : IProcessOnlineMlmService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;
        private readonly EncoreMongo_Context encoreMongo_Context;

        private readonly IMigrateService migrateService;
        private readonly ICalculationTypesService calculationTypesService;
        private readonly IOrderCalculationTypesService orderCalculationTypesService;
        private readonly IPersonalIndicatorLogService personalIndicatorLogService;
        private readonly IPersonalIndicatorDetailLogService personalIndicatorDetailLogService;

        private readonly IProcessOnlineRepository processOnlineRepository;
        private readonly IAccountKPIsRepository accountKPIsRepository;
        private readonly IAccountInformationRepository accountsInformationRepository;
        private readonly IHomeService homeService;

        public OnlineMLM_Statistics Statistics { get; set; }
        public PersonalIndicatorLog PersonalIndicatorLog { get; set; }

        public List<SponsorTree> Accounts_UpLine { get; set; }

        public ProcessOnlineMlmService
        (
            IUnitOfWork<EncoreCore_Context> _unitOfWork_Core,
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm,
            IMigrateService _migrateService,
            ICalculationTypesService _calculationTypesService,
            IOrderCalculationTypesService _orderCalculationTypesService,
            IPersonalIndicatorLogService _personalIndicatorLogService,
            IPersonalIndicatorDetailLogService _personalIndicatorDetailLogService,

            IProcessOnlineRepository _processOnlineRepository,
            IAccountKPIsRepository _accountKPIsRepository,
            IAccountInformationRepository _accountsInformationRepository,
            IConfiguration configuration,
            IHomeService _homeService
        )
        {
            unitOfWork_Core = _unitOfWork_Core;
            unitOfWork_Comm = _unitOfWork_Comm;
            encoreMongo_Context = new EncoreMongo_Context(configuration);

            migrateService = _migrateService;
            calculationTypesService = _calculationTypesService;
            orderCalculationTypesService = _orderCalculationTypesService;
            personalIndicatorLogService = _personalIndicatorLogService;
            personalIndicatorDetailLogService = _personalIndicatorDetailLogService;
            homeService = _homeService;

            processOnlineRepository = _processOnlineRepository;
            accountKPIsRepository = _accountKPIsRepository;
            accountsInformationRepository = _accountsInformationRepository;
        }

        public void ProcessMLMOrder(int _orderId, string country)
        {
            IRepository<Orders> ordersRepository = unitOfWork_Core.GetRepository<Orders>();
            var order = ordersRepository.GetFirstOrDefault(o => o.OrderID == _orderId, null, null, true);

            if (order == null)
            {
                return;
            }

            Statistics = new OnlineMLM_Statistics(processOnlineRepository, order);

            var existsorderCalculationOnline = processOnlineRepository.GetExists_OrderCalculationsOnline(Statistics.Order.OrderID);

            if (
                    (
                        Statistics.Order.OrderStatusID == (short)Constants.OrderStatus.Paid ||
                        Statistics.Order.OrderStatusID == (short)Constants.OrderStatus.Printed ||
                        Statistics.Order.OrderStatusID == (short)Constants.OrderStatus.Shipped ||
                        Statistics.Order.OrderStatusID == (short)Constants.OrderStatus.Invoiced ||
                        Statistics.Order.OrderStatusID == (short)Constants.OrderStatus.Embarked
                    ) && !existsorderCalculationOnline
               )
            {

                Statistics.PeriodID = GetPeriodId();
                Accounts_UpLine = GetAccounts_UpLine(Statistics.Order.AccountID);

                PersonalIndicatorLog = new PersonalIndicatorLog()
                {
                    OrderID = Statistics.Order.OrderID,
                    OrderStatusID = Statistics.Order.OrderStatusID
                };

                PersonalIndicatorLog = personalIndicatorLogService.Insert(PersonalIndicatorLog);

                Indicators_InPersonal();
                Indicators_InDivision();

                IRepository<Activities> activitiesRepository = unitOfWork_Core.GetRepository<Activities>();
                var activityPrev = activitiesRepository.GetFirstOrDefault(a => a.PeriodID == Statistics.PeriodID && a.AccountID == Statistics.Order.AccountID, null, null, true);
                var IsQualifiedPrev = activityPrev != null ? activityPrev.IsQualified : false;

                Execute_Activities();

                var activityCurrent = activitiesRepository.GetFirstOrDefault(a => a.PeriodID == Statistics.PeriodID && a.AccountID == Statistics.Order.AccountID, null, null, true);

                var IsQualifiedCurrent = activityCurrent != null ? activityCurrent.IsQualified : false;
                var activeDownline = false;
                if (IsQualifiedPrev == false && IsQualifiedCurrent == true)
                {
                    activeDownline = true;
                    Indicators_InCountConsultantActives();
                }

                Indicators_InTableReports(activeDownline);
                Indicators_InOrderCalculationsOnline();
                Migrate_AccountInformationByAccountId(country);
                UpdateTransactionDate(1, country);
                PersonalIndicatorLog = personalIndicatorLogService.Update(PersonalIndicatorLog);

                UpdateIngresosDiarios(country, Statistics.Order.AccountID);
            }
        }

        public void ProcessMLMLote(int loteId, string country)
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
                        ProcessMLMOrder(order.OrderId, country);
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
        public List<SponsorTree> GetAccounts_UpLine(int accountId)
        {
            IRepository<SponsorTree> sponsorTreeRepository = unitOfWork_Comm.GetRepository<SponsorTree>();
            var result = sponsorTreeRepository.FromSql($"SELECT * FROM [fnGetAccount_Upline_Aux] ({accountId})").ToList();
            return result == null ? null : result.ToList();
        }

        private void Indicadores_UpdateValue_AccountKPIs(List<int> accountsIds, int calculationType, decimal value)
        {
            accountKPIsRepository.GetPagedList(a => accountsIds.Contains(a.AccountID) && a.PeriodID == Statistics.PeriodID && a.CalculationTypeID == calculationType, null, null, 0, accountsIds.Count, false)
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
            var result = periodsRepository.GetFirstOrDefault(p => Statistics.Order.CompleteDateUTC >= p.StartDateUTC && Statistics.Order.CompleteDateUTC <= p.EndDateUTC && p.PlanID == 1, null, null, true);
            return result.PeriodID;
        }

        #endregion

        #region Calculos Personales
        public void Indicators_InPersonal()
        {
            int calculationType_PQV = calculationTypesService.GetCalculationTypeIdByCode("PQV");
            int calculationType_PCV = calculationTypesService.GetCalculationTypeIdByCode("PCV");
            int calculationType_PRV = calculationTypesService.GetCalculationTypeIdByCode("PRV");

            var detailLog = personalIndicatorDetailLogService.Insert(personalIndicatorDetailLogService.Create(PersonalIndicatorLog, "CodeSubProcessCalculationPersonalIndicator"));

            try
            {
                if (detailLog != null && detailLog.EndTime == null)
                {
                    IndicadoresInPersonal_UpdateValue(calculationType_PQV, (decimal)Statistics.QV);
                    IndicadoresInPersonal_UpdateValue(calculationType_PCV, (decimal)Statistics.CV);
                    IndicadoresInPersonal_UpdateValue(calculationType_PRV, (decimal)Statistics.RV);
                }
            }
            catch (Exception ex)
            {
                detailLog.RealError = "Error";
            }

            personalIndicatorDetailLogService.Update(detailLog);
        }

        public void IndicadoresInPersonal_UpdateValue(int calculationType, decimal value)
        {
            var result = accountKPIsRepository.GetFirstOrDefault(a => a.AccountID == Statistics.Order.AccountID && a.PeriodID == Statistics.PeriodID && a.CalculationTypeID == calculationType, null, null, false);

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
                            AccountID = Statistics.Order.AccountID,
                            PeriodID = Statistics.PeriodID,
                            CalculationTypeID = calculationType,
                            Value = value,
                            DateModified = DateTime.Now
                        });
            }

            unitOfWork_Comm.SaveChanges();
        }

        #endregion

        #region Calculos Divison
        public void Indicators_InDivision()
        {
            var detailLog = personalIndicatorDetailLogService.Insert(personalIndicatorDetailLogService.Create(PersonalIndicatorLog, "CodeSubProcessCalculationDivisionIndicator"));
            try
            {
                if (detailLog != null && detailLog.EndTime == null)
                {
                    IndicadoresInDivision_Initialize();
                    IndicadoresInDivision_UpdateValue();
                }
            }
            catch (Exception ex)
            {
                detailLog.RealError = "Error";
            }

            personalIndicatorDetailLogService.Update(detailLog);
        }

        public void IndicadoresInDivision_Initialize()
        {
            List<string> codes = new List<string> { "PQV", "PRV", "PCV", "GQV", "GCV", "DQV", "DCV", "CQL", "DQVT" };
            var calculationTypesIds = calculationTypesService.GetCalculationTypesByCodes(codes);

            var result = processOnlineRepository.GetListAccounts_Initialize(Accounts_UpLine.Select(a => a.AccountID).ToList(), calculationTypesIds, Statistics.PeriodID);
            accountKPIsRepository.Insert(result);

            unitOfWork_Comm.SaveChanges();
        }

        private void IndicadoresInDivision_UpdateValue()
        {
            var accountsIds = Accounts_UpLine.Select(a => a.AccountID).ToList();

            int calculationType_DCV = calculationTypesService.GetCalculationTypeIdByCode("DCV");
            int calculationType_DQVT = calculationTypesService.GetCalculationTypeIdByCode("DQVT");

            Indicadores_UpdateValue_AccountKPIs(accountsIds, calculationType_DCV, value: (decimal)Statistics.CV);
            Indicadores_UpdateValue_AccountKPIs(accountsIds, calculationType_DQVT, value: (decimal)Statistics.QV);

            IndicadoresInDivision_Valida_Porcentaje(Accounts_UpLine);
        }

        public void IndicadoresInDivision_Valida_Porcentaje(List<SponsorTree> accounts)
        {
            int currentAccountID, porcentForRuler, titleForRuler;
            int? currentAccountTitle;
            decimal DQV_Result;

            IRepository<RuleTypes> ruleTypesRepository = unitOfWork_Comm.GetRepository<RuleTypes>();

            var ruleType = ruleTypesRepository.GetFirstOrDefault(rt => rt.Name == "VolumenDivision" && rt.Active == true, null, rt => rt.Include(r => r.RequirementRules), true);

            if (ruleType != null)
            {
                porcentForRuler = int.Parse(ruleType.RequirementRules.Value1);
                titleForRuler = int.Parse(ruleType.RequirementRules.Value2);

                var list_AccountIDsNoApplied = accounts.Where(a => a.CurrentPAT < titleForRuler).Select(a => a.AccountID).ToList();

                int calculationType_DQV = calculationTypesService.GetCalculationTypeIdByCode("DQV");
                Indicadores_UpdateValue_AccountKPIs(list_AccountIDsNoApplied, calculationType_DQV, value: (decimal)Statistics.QV);

                foreach (var account in accounts.Where(a => a.CurrentPAT >= titleForRuler))
                {
                    currentAccountID = account.AccountID;
                    currentAccountTitle = account.CurrentPAT;

                    DQV_Result = 0;
                    DQV_Result = processOnlineRepository.GetQV_ByAccount_PorcentRuler(currentAccountID, Statistics.PeriodID, porcentForRuler);

                    var result = accountKPIsRepository.GetFirstOrDefault(a => a.AccountID == currentAccountID && a.PeriodID == Statistics.PeriodID && a.CalculationTypeID == calculationType_DQV, null, null, false);

                    if (result != null)
                    {
                        result.Value = DQV_Result;
                        result.DateModified = DateTime.Now;
                        accountKPIsRepository.Update(result);
                    }

                    unitOfWork_Comm.SaveChanges();
                }
            }

        }
        #endregion

        #region Calculos OrdercalculationsOnline
        public void Indicators_InOrderCalculationsOnline()
        {
            var detailLog = personalIndicatorDetailLogService.Insert(personalIndicatorDetailLogService.Create(PersonalIndicatorLog, "CodeSubProcessCalculationOrderCalculationIndicator"));

            IRepository<Entities.Entities.Core.Accounts> accountsRepository = unitOfWork_Core.GetRepository<Entities.Entities.Core.Accounts>();
            var account = accountsRepository.GetFirstOrDefault(a => a.AccountID == Statistics.Order.AccountID, null, null, true);

            int orderCalculationTypeID_CV = orderCalculationTypesService.GetOrderCalculationTypeIdByCode("CV");
            int orderCalculationTypeID_RP = orderCalculationTypesService.GetOrderCalculationTypeIdByCode("RP");
            int orderCalculationTypeID_QV = orderCalculationTypesService.GetOrderCalculationTypeIdByCode("QV");

            try
            {
                if (detailLog != null && detailLog.EndTime == null)
                {
                    OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_QV, (decimal)Statistics.QV, account.AccountTypeID);
                    OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_CV, (decimal)Statistics.CV, account.AccountTypeID);
                    OrdercalculationsOnline_UpdateValue(orderCalculationTypeID_RP, (decimal)Statistics.RV, account.AccountTypeID);
                }
            }
            catch (Exception ex)
            {
                detailLog.RealError = "Error";
            }

            personalIndicatorDetailLogService.Update(detailLog);
        }

        public void OrdercalculationsOnline_UpdateValue(int orderCalculationTypeID, decimal value, int accountTypeId)
        {
            IRepository<OrderCalculationsOnline> orderCalculationOnlineRepository = unitOfWork_Comm.GetRepository<OrderCalculationsOnline>();

            var result = orderCalculationOnlineRepository.GetFirstOrDefault(o => o.OrderID == Statistics.Order.OrderID && o.OrderCalculationTypeID == orderCalculationTypeID, null, null, false);

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
                            AccountID = Statistics.Order.AccountID,
                            OrderID = Statistics.Order.OrderID,
                            OrderCalculationTypeID = orderCalculationTypeID,
                            OrderStatusID = Statistics.Order.OrderStatusID,
                            Value = value,
                            CalculationDateUTC = Statistics.Order.CommissionDateUTC,
                            ParentOrderID = Statistics.Order.ParentOrderID,
                            AccountTypeID = accountTypeId,
                            OrderTypeID = Statistics.Order.OrderTypeID,
                            DateModifiedUTC = DateTime.Now
                        });
            }

            unitOfWork_Comm.SaveChanges();
        }
        #endregion

        #region Actualizar Reportes
        public void Indicators_InTableReports(bool activeDownline)
        {
            var detailLog = personalIndicatorDetailLogService.Insert(personalIndicatorDetailLogService.Create(PersonalIndicatorLog, "CodeSubProcessCalculationTableReportsIndicator"));

            try
            {
                if (detailLog != null && detailLog.EndTime == null)
                {
                    var accountInformation_Current = accountsInformationRepository.GetFirstOrDefault(a => a.AccountID == Statistics.Order.AccountID && a.PeriodID == Statistics.PeriodID, null, null, false);

                    accountInformation_Current.PCV += Statistics.CV;
                    accountInformation_Current.PQV += Statistics.QV;
                    accountsInformationRepository.Update(accountInformation_Current);

                    unitOfWork_Comm.SaveChanges();

                    var listAccounts = Accounts_UpLine.Select(a => a.AccountID);
                    var accountInformations = accountsInformationRepository.GetPagedList(a => a.PeriodID == Statistics.PeriodID && listAccounts.Contains(a.AccountID), null, null, 0, Accounts_UpLine.Count, false);

                    IRepository<AccountKPIsDetails> accountKPIsDetailsRepository = unitOfWork_Comm.GetRepository<AccountKPIsDetails>();
                    var listKpisCode = new List<string>(new[] { "DQV", "DCV", "GQV", "GCV", "NCA" });

                    int calculationType_DQV = calculationTypesService.GetCalculationTypeIdByCode("DQV");

                    foreach (var accountInformation_Up in accountInformations.Items)
                    {
                        var result_accountKPIs = accountKPIsRepository.GetFirstOrDefault(akp => akp.AccountID == accountInformation_Up.AccountID && akp.PeriodID == Statistics.PeriodID && akp.CalculationTypeID == calculationType_DQV, null, null, true);

                        accountInformation_Up.DQV = result_accountKPIs.Value;
                        accountInformation_Up.DCV += Statistics.CV;
                        accountInformation_Up.DQVT += Statistics.QV;
                        accountInformation_Up.ActiveDownline = activeDownline ? ((accountInformation_Up.ActiveDownline ?? 0) + 1) : accountInformation_Up.ActiveDownline;
                        accountsInformationRepository.Update(accountInformation_Up);

                        unitOfWork_Comm.SaveChanges();

                        foreach (var kpiCode in listKpisCode)
                        {
                            var accountKPIsDetails = accountKPIsDetailsRepository.GetFirstOrDefault(a => a.PeriodID == Statistics.PeriodID &&
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
                                            PeriodID = Statistics.PeriodID,
                                            SponsorID = accountInformation_Up.AccountID,
                                            SponsorName = accountInformation_Up.AccountName,
                                            DownlineID = accountInformation_Current.AccountID,
                                            DownlineName = accountInformation_Current.AccountName,
                                            KPICode = kpiCode,
                                            Value = ((kpiCode == "DQV" || kpiCode == "GQV") ? Statistics.QV : Statistics.CV),
                                            Percentage = 1,
                                            DownlinePaidAsTitle = null,
                                            TreeLevel = accountInformation_Up.LEVEL,
                                            DateModified = DateTime.Now
                                        }
                                    );
                            }
                            else
                            {

                                accountKPIsDetails.Value = accountKPIsDetails.Value + ((kpiCode == "DQV" || kpiCode == "GQV") ? Statistics.QV : Statistics.CV);
                                accountKPIsDetails.Percentage = 1;
                                accountKPIsDetails.DownlinePaidAsTitle = null;
                                accountKPIsDetails.TreeLevel = accountInformation_Current.LEVEL;
                                accountKPIsDetails.DateModified = DateTime.Now;
                            }

                            unitOfWork_Comm.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                detailLog.RealError = "Error";
            }

            personalIndicatorDetailLogService.Update(detailLog);
        }

        #endregion

        #region Actividades
        public void Execute_Activities()
        {
            var detailLog = personalIndicatorDetailLogService.Insert(personalIndicatorDetailLogService.Create(PersonalIndicatorLog, "CodeSubProcessExecutionActivities"));
            try
            {
                if (detailLog != null && detailLog.EndTime == null)
                {
                    processOnlineRepository.Execute_Activities(Statistics.Order.OrderID);
                }
            }
            catch (Exception ex)
            {
                detailLog.RealError = "Error";
            }

            personalIndicatorDetailLogService.Update(detailLog);
        }
        #endregion

        #region Actualizar Mongo
        public void Migrate_AccountInformationByAccountId(string country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);

            var detailLog = personalIndicatorDetailLogService.Insert(personalIndicatorDetailLogService.Create(PersonalIndicatorLog, "CodeSubProcessCalculationMigrateMongoIndicator"));

            IRepository<Titles> titlesRepository = unitOfWork_Comm.GetRepository<Titles>();
            var titles = titlesRepository.GetAll().ToList();

            var accountsIds = GetAccounts_UpLine(Statistics.Order.AccountID).Select(a => a.AccountID).ToList();
            var accountsInformations = accountsInformationRepository.GetListAccountInformationByPeriodIdAndAccountId(Statistics.PeriodID, accountsIds).ToList();

            IRepository<Activities> activitiesRepository = unitOfWork_Core.GetRepository<Activities>();
            var activity = activitiesRepository.GetFirstOrDefault(a => a.PeriodID == Statistics.PeriodID && a.AccountID == Statistics.Order.AccountID, null, a => a.Include(aa => aa.ActivityStatuses).Include(aa => aa.AccountConsistencyStatuses), true);

            IEnumerable<AccountsInformation_Mongo> result = migrateService.GetAccountInformations(titles, accountsInformations, activity, Statistics.Order.AccountID);
            try
            {
                if (detailLog != null && detailLog.EndTime == null)
                {
                    foreach (var item in result)
                    {
                        var item_Mongo = accountInformationCollection.Find(ai => ai.AccountsInformationID == item.AccountsInformationID).FirstOrDefault();
                        if (item_Mongo != null)
                        {
                            accountInformationCollection.ReplaceOne(ai => ai.AccountsInformationID == item.AccountsInformationID, item, new UpdateOptions { IsUpsert = true });
                        }
                        else
                        {
                            accountInformationCollection.InsertOne(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                detailLog.RealError = "Error";
            }

            personalIndicatorDetailLogService.Update(detailLog);
        }
        #endregion

        #region Actualizar AccountInformation (NCA)
        public void Indicators_InCountConsultantActives()
        {
            var detailLog = personalIndicatorDetailLogService.Insert(personalIndicatorDetailLogService.Create(PersonalIndicatorLog, "CodeSubProcessCalculationUpdateNCA"));

            try
            {
                if (detailLog != null && detailLog.EndTime == null)
                {
                    int calculationType_NCA = calculationTypesService.GetCalculationTypeIdByCode("NCA");

                    var listAccounts = Accounts_UpLine.Select(a => a.AccountID).ToList();

                    var result = accountKPIsRepository.GetFirstOrDefault(a => a.AccountID == Statistics.Order.AccountID && a.PeriodID == Statistics.PeriodID && a.CalculationTypeID == calculationType_NCA, null, null, false);

                    if (result == null)
                    {
                        accountKPIsRepository.Insert(
                                new AccountKPIs
                                {
                                    AccountID = Statistics.Order.AccountID,
                                    PeriodID = Statistics.PeriodID,
                                    CalculationTypeID = calculationType_NCA,
                                    Value = 0,
                                    DateModified = DateTime.Now
                                });
                    }

                    unitOfWork_Comm.SaveChanges();

                    Indicadores_UpdateValue_AccountKPIs(listAccounts, calculationType_NCA, value: 1);
                }
            }
            catch (Exception ex)
            {
                detailLog.RealError = "Error";
            }

            personalIndicatorDetailLogService.Update(detailLog);
        }
        #endregion

        #region TransactionDate
        public void UpdateTransactionDate(int typeTransaction, string country)
        {
            TransactionMonitor_Mongo item = new TransactionMonitor_Mongo
            {
                TransactionMonitorID = typeTransaction,
                TransactionDate = DateTime.Now
            };

            IMongoCollection<TransactionMonitor_Mongo> transactionMonitorCollection = encoreMongo_Context.TransactionMonitorProvider(country);

            transactionMonitorCollection.ReplaceOne(ai => ai.TransactionMonitorID == typeTransaction, item, new UpdateOptions { IsUpsert = true });
        }
        #endregion


        public void UpdateIngresosDiarios(string country, int accountID)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<PerformanceIndicatorDay_Mongo> performanceIndicatorDayCollection = encoreMongo_Context.PerformanceIndicatorDayProvider(country);
            IMongoCollection<EnrrollmentAccountsByDayTemp_Mongo> enrrolmentAccountByDayTemp = encoreMongo_Context.EnrrollmentAccountsByDayTempProvider(country);

            var period = homeService.GetCurrentPeriod(country).PeriodID;
            var date = DateTime.Now.Date;

            var lista = accountInformationCollection.Find(a => a.PeriodID == period && (a.JoinDate >= date && a.JoinDate <= DateTime.Now), null).ToList();
            var filterDefinition = Builders<EnrrollmentAccountsByDayTemp_Mongo>.Filter.In(ai => ai.AccountID, lista.Select(x => x.AccountID));

            var listaTemp = enrrolmentAccountByDayTemp.
                Aggregate()
                .Match(filterDefinition).ToList();

            List<EnrrollmentAccountsByDayTemp_Mongo> listaNuevos = new List<EnrrollmentAccountsByDayTemp_Mongo>();

            foreach (var item in lista)
            {
                if (!listaTemp.Any(q => q.AccountID == item.AccountID))
                {
                    listaNuevos.Add(new EnrrollmentAccountsByDayTemp_Mongo
                    {
                        AccountID = item.AccountID,
                        DayOfMonth = DateTime.Now.Day,
                        PeriodID = period
                    });
                }
            }

            foreach (var item in listaNuevos)
            {
                var accountRoot = AccountsUtils.RecursivoWithoutSponsor(accountInformationCollection, item.AccountID, period);
                foreach (var account in accountRoot)
                {
                    var record = performanceIndicatorDayCollection.Find(ai => ai.AccountID == account.AccountID && ai.PeriodID == period).FirstOrDefault();
                    performanceIndicatorDayCollection.ReplaceOne(ai => ai.AccountID == account.AccountID && ai.PeriodID == period, new PerformanceIndicatorDay_Mongo
                    {
                        AccountID = account.AccountID,
                        DayOfMonth = DateTime.Now.Day,
                        Ingresos = record != null ? record.Ingresos + 1 : 1,
                        PeriodID = period
                    }, new UpdateOptions { IsUpsert = true });
                }

                enrrolmentAccountByDayTemp.ReplaceOne(ai => ai.AccountID == item.AccountID && ai.DayOfMonth == DateTime.Now.Day, new EnrrollmentAccountsByDayTemp_Mongo
                {
                    AccountID = item.AccountID,
                    DayOfMonth = DateTime.Now.Day,
                    PeriodID = period
                }, new UpdateOptions { IsUpsert = true });
            }
        }
    }
}
