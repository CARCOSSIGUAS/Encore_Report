using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Belcorp.Encore.Repositories.Interfaces;
using Belcorp.Encore.Entities.Constants;
using Belcorp.Encore.Application.Utilities;

namespace Belcorp.Encore.Application.Services
{
    public class MigrateService : IMigrateService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAccountInformationRepository accountInformationRepository;
        private readonly IBonusDetailsRepository bonusDetailsRepository;
        private readonly IAccountKPIsDetailsRepository accountKPIsDetailsRepository;

        public MigrateService
        (
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm,
            IUnitOfWork<EncoreCore_Context> _unitOfWork_Core,
            IAccountInformationRepository _accountInformationRepository,
            IBonusDetailsRepository _bonusDetailsRepository,
            IAccountKPIsDetailsRepository _accountKPIsDetailsRepository,
            IConfiguration configuration
        )
        {
            unitOfWork_Comm = _unitOfWork_Comm;
            unitOfWork_Core = _unitOfWork_Core;
            accountInformationRepository = _accountInformationRepository;
            bonusDetailsRepository = _bonusDetailsRepository;
            accountKPIsDetailsRepository = _accountKPIsDetailsRepository;
            encoreMongo_Context = new EncoreMongo_Context(configuration);
        }

        public void MigrateAccountInformationByPeriod(int? periodId = null, string country = null)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);

            if (periodId == null)
            {
                periodId = GetCurrentPeriod();
            }

            var total = accountInformationRepository.GetPagedList(a => a.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            IRepository<Titles> titlesRepository = unitOfWork_Comm.GetRepository<Titles>();
            var titles = titlesRepository.GetAll().ToList();

            accountInformationCollection.DeleteMany(a => a.PeriodID == periodId);

            for (int i = 0; i < ii; i++)
            {
                var accountsInformation = accountInformationRepository.GetPagedList(a => a.PeriodID == periodId, a => a.OrderBy(o => o.AccountsInformationID), null, i, 10000, true).Items;
                IEnumerable<AccountsInformation_Mongo> result = GetAccountInformations(titles, accountsInformation);

                accountInformationCollection.InsertMany(result);
            }
            UpdateTransactionDate(2, country);
        }

        public IEnumerable<AccountsInformation_Mongo> GetAccountInformations(List<Titles> titles, IList<AccountsInformation> accountsInformation, Activities activityPrevious = null, Activities activityCurrent = null, int? AccountID = null)
        {
            var UplineLeader0 = 0;
            var UplineLeaderM3 = 0;

            int? LeftRighBower = null;
            if (AccountID.HasValue && activityPrevious != null &&
                    (
                        activityPrevious.AccountConsistencyStatuses.AccountConsistencyStatusID == (short)Constants.AccountConsistencyStatuses.BegunEnrollment
                    )
              )
            {
                var account = accountsInformation.Where(a => a.AccountID == AccountID).FirstOrDefault();
                var sponsor = accountsInformation.Where(a => a.AccountID == account.SponsorID).FirstOrDefault();

                if (sponsor != null)
                {
                    UplineLeader0 = sponsor.UplineLeader0 ?? 0;
                    UplineLeaderM3 = sponsor.UplineLeaderM3 ?? 0;
                    LeftRighBower = sponsor.LeftBower;
                }
            }

            var result = from accountsInfo in accountsInformation
                         join titlesInfo_Career in titles on Int32.Parse(string.IsNullOrEmpty(accountsInfo.CareerTitle) ? "0" : accountsInfo.CareerTitle) equals titlesInfo_Career.TitleID
                         join titlesInfo_Paid in titles on Int32.Parse(string.IsNullOrEmpty(accountsInfo.PaidAsCurrentMonth) ? "0" : accountsInfo.PaidAsCurrentMonth) equals titlesInfo_Paid.TitleID
                         where !accountsInfo.AccountName.Contains("TempName")
                         select new AccountsInformation_Mongo
                         {
                             AccountsInformationID = accountsInfo.AccountsInformationID,
                             PeriodID = accountsInfo.PeriodID,
                             AccountID = accountsInfo.AccountID,
                             AccountNumber = accountsInfo.AccountNumber,
                             AccountName = String.IsNullOrEmpty(accountsInfo.AccountName) ? "" : accountsInfo.AccountName.ToUpper(),
                             SponsorID = accountsInfo.SponsorID,
                             SponsorName = String.IsNullOrEmpty(accountsInfo.SponsorName) ? "" : accountsInfo.SponsorName.ToUpper(),
                             Address = accountsInfo.Address,
                             PostalCode = accountsInfo.PostalCode,
                             City = accountsInfo.City,
                             STATE = accountsInfo.STATE,
                             Region = accountsInfo.Region,
                             NewStatus = (AccountID.HasValue && AccountID == accountsInfo.AccountID && activityCurrent != null) ? activityCurrent.AccountConsistencyStatuses.Name : accountsInfo.NewStatus,
                             TimeLimitToBeDemote = accountsInfo.TimeLimitToBeDemote,
                             CareerTitle = accountsInfo.CareerTitle,
                             PaidAsCurrentMonth = accountsInfo.PaidAsCurrentMonth,
                             PaidAsLastMonth = accountsInfo.PaidAsLastMonth,
                             VolumeForCareerTitle = accountsInfo.VolumeForCareerTitle,
                             NineMonthsPQV = accountsInfo.NineMonthsPQV,
                             PQV = accountsInfo.PQV,
                             PCV = accountsInfo.PCV,
                             GQV = accountsInfo.GQV,
                             GCV = accountsInfo.GCV,
                             DQVT = accountsInfo.DQVT,
                             DCV = accountsInfo.DCV,
                             DQV = accountsInfo.DQV,
                             JoinDate = accountsInfo.JoinDate,
                             Generation = accountsInfo.Generation,
                             LEVEL = accountsInfo.LEVEL,
                             SortPath = accountsInfo.SortPath,
                             LeftBower = (AccountID.HasValue && AccountID == accountsInfo.AccountID && LeftRighBower.HasValue) ? LeftRighBower : accountsInfo.LeftBower,
                             RightBower = (AccountID.HasValue && AccountID == accountsInfo.AccountID && LeftRighBower.HasValue) ? LeftRighBower : accountsInfo.RightBower,
                             RequirementNewGeneration = accountsInfo.RequirementNewGeneration,
                             TimeLimitForNewGeneration = accountsInfo.TimeLimitForNewGeneration,
                             Title1Legs = accountsInfo.Title1Legs,
                             Title2Legs = accountsInfo.Title2Legs,
                             Title3Legs = accountsInfo.Title3Legs,
                             Title4Legs = accountsInfo.Title4Legs,
                             Title5Legs = accountsInfo.Title5Legs,
                             Title6Legs = accountsInfo.Title6Legs,
                             Title7Legs = accountsInfo.Title7Legs,
                             Title8Legs = accountsInfo.Title8Legs,
                             Title9Legs = accountsInfo.Title9Legs,
                             Title10Legs = accountsInfo.Title10Legs,
                             Title11Legs = accountsInfo.Title11Legs,
                             Title12Legs = accountsInfo.Title12Legs,
                             Title13Legs = accountsInfo.Title13Legs,
                             Title14Legs = accountsInfo.Title14Legs,
                             EmailAddress = accountsInfo.EmailAddress,
                             CQL = accountsInfo.CQL,
                             LastOrderDate = accountsInfo.LastOrderDate,
                             IsCommissionQualified = accountsInfo.IsCommissionQualified,
                             BirthdayUTC = accountsInfo.BirthdayUTC,

                             UplineLeaderL1 = accountsInfo.UplineLeaderL1,
                             UplineLeaderL1Name = accountsInfo.UplineLeaderL1Name,
                             TotalDownline = accountsInfo.TotalDownline,
                             CreditAvailable = accountsInfo.CreditAvailable,
                             DebtsToExpire = accountsInfo.DebtsToExpire,
                             ExpiredDebts = accountsInfo.ExpiredDebts,
                             GenerationM3 = accountsInfo.GenerationM3,
                             ActiveDownline = accountsInfo.ActiveDownline,
                             TitleMaintainance = accountsInfo.TitleMaintainance,
                             SalesAverage = accountsInfo.SalesAverage,
                             NewQualification = accountsInfo.NewQualification,
                             NewEnrollments = accountsInfo.NewEnrollments,
                             NineMonthsGQV = accountsInfo.NineMonthsGQV,
                             NineMonthsDQV = accountsInfo.NineMonthsDQV,
                             ConsultActive = accountsInfo.ConsultActive,

                             CareerTitle_Des = titlesInfo_Career.ClientName,
                             PaidAsCurrentMonth_Des = titlesInfo_Paid.ClientName,

                             Activity = (AccountID.HasValue && AccountID == accountsInfo.AccountID && activityCurrent != null) ? activityCurrent.ActivityStatuses.ExternalName : accountsInfo.Activity,
                             NCWP = accountsInfo.NCWP,
                             UplineLeader0 = (AccountID.HasValue && AccountID == accountsInfo.AccountID) ? UplineLeader0 : accountsInfo.UplineLeader0,
                             UplineLeaderM3 = (AccountID.HasValue && AccountID == accountsInfo.AccountID) ? UplineLeaderM3 : accountsInfo.UplineLeaderM3,
                             UplineLeaderM3Name = accountsInfo.UplineLeaderM3Name,

                             IsQualified = (AccountID.HasValue && AccountID == accountsInfo.AccountID && activityCurrent != null) ? activityCurrent.IsQualified : accountsInfo.IsQualified,
                             HasContinuity = (AccountID.HasValue && AccountID == accountsInfo.AccountID && activityCurrent != null) ? activityCurrent.HasContinuity : accountsInfo.HasContinuity,
                             MonthBirthday = accountsInfo.BirthdayUTC.HasValue ? accountsInfo.BirthdayUTC.Value.Month : 0,
                             DayBirthday = accountsInfo.BirthdayUTC.HasValue ? accountsInfo.BirthdayUTC.Value.Day : 0,
                         };

            return result;
        }

        public void MigrateBonusDetailsByPeriod(int? periodId = null, string country = null)
        {
            IMongoCollection<BonusDetails_Mongo> bonusDetailsCollection = encoreMongo_Context.BonusDetailsProvider(country);

            if (periodId == null)
            {
                periodId = GetCurrentPeriod();
            }

            IRepository<BonusTypes> bonusTypesRepository = unitOfWork_Comm.GetRepository<BonusTypes>();
            var bonusTypes = bonusTypesRepository.GetAll().ToList();

            var total = bonusDetailsRepository.GetPagedList(b => b.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            bonusDetailsCollection.DeleteMany(b => b.PeriodID == periodId);

            for (int i = 0; i < ii; i++)
            {
                var bonusDetails = bonusDetailsRepository.GetPagedList(b => b.PeriodID == periodId, b => b.OrderBy(o => o.BonusDetailID), null, i, 10000, true).Items;
                IEnumerable<BonusDetails_Mongo> result = GetBonusDetails(bonusDetails, bonusTypes);

                bonusDetailsCollection.InsertMany(result);
            }
        }

        private IEnumerable<BonusDetails_Mongo> GetBonusDetails(IList<BonusDetails> bonusDetails, IList<BonusTypes> bonusTypes)
        {
            return from bonusDetailsInfo in bonusDetails
                   select new BonusDetails_Mongo
                   {
                       BonusDetailID = bonusDetailsInfo.BonusDetailID,
                       SponsorID = bonusDetailsInfo.SponsorID,
                       SponsorName = bonusDetailsInfo.SponsorName,
                       DownlineID = bonusDetailsInfo.DownlineID,
                       DownlineName = bonusDetailsInfo.DownlineName,
                       BonusTypeID = bonusDetailsInfo.BonusTypeID,
                       BonusCode = bonusDetailsInfo.BonusCode,
                       OrderID = bonusDetailsInfo.OrderID == null ? 0 : bonusDetailsInfo.OrderID,
                       QV = bonusDetailsInfo.QV,
                       CV = bonusDetailsInfo.CV,
                       Percentage = bonusDetailsInfo.Percentage,
                       OriginalAmount = bonusDetailsInfo.OriginalAmount,
                       Adjustment = bonusDetailsInfo.Adjustment,
                       PayoutAmount = bonusDetailsInfo.PayoutAmount,
                       CurrencyTypeID = bonusDetailsInfo.CurrencyTypeID,
                       AccountSponsorTypeID = bonusDetailsInfo.AccountSponsorTypeID,
                       TreeLevel = bonusDetailsInfo.TreeLevel,
                       PeriodID = bonusDetailsInfo.PeriodID,
                       ParentOrderID = bonusDetailsInfo.ParentOrderID,
                       CorpOriginalAmount = bonusDetailsInfo.CorpOriginalAmount,
                       CorpAdjustment = bonusDetailsInfo.CorpAdjustment,
                       CorpPayoutAmount = bonusDetailsInfo.CorpPayoutAmount,
                       CorpCurrencyTypeID = bonusDetailsInfo.CorpCurrencyTypeID,
                       DateModified = bonusDetailsInfo.DateModified,
                       INDICATORPAYMENT = bonusDetailsInfo.INDICATORPAYMENT,
                       PERIODIDPAYMENT = bonusDetailsInfo.PERIODIDPAYMENT,
                       BonusClass = bonusTypes.Where(b => b.BonusTypeID == bonusDetailsInfo.BonusTypeID).FirstOrDefault() == null ? "" : bonusTypes.Where(b => b.BonusTypeID == bonusDetailsInfo.BonusTypeID).FirstOrDefault().BonusClass
                   };
        }

        public void MigrateAccounts(string country)
        {
            IMongoCollection<Accounts_Mongo> accountCollection = encoreMongo_Context.AccountsProvider(country);

            IRepository<Entities.Entities.Core.Accounts> accountsRepository = unitOfWork_Core.GetRepository<Entities.Entities.Core.Accounts>();
            accountCollection.DeleteMany(new BsonDocument { });
            var total = accountsRepository.GetPagedList(null, null, null, 0, 5000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var accounts = accountsRepository.GetPagedList(null, a => a.OrderBy(o => o.AccountID), a => a.Include(p => p.AccountPhones)
                                                                                                             .Include(p => p.AccountAddresses).ThenInclude(p => p.Addresses)
                                                                                                             .Include(p => p.AccountAdditionalTitulars)
                                                                                                     , i, 5000, true).Items;

                List<Accounts_Mongo> accounts_Mongo = new List<Accounts_Mongo>();
                foreach (var account in accounts)
                {
                    Accounts_Mongo account_Mongo = new Accounts_Mongo();

                    account_Mongo.CountryID = 0;
                    account_Mongo.AccountID = account.AccountID;

                    account_Mongo.AccountNumber = account.AccountNumber;
                    account_Mongo.AccountTypeID = account.AccountTypeID;
                    account_Mongo.FirstName = account.FirstName;
                    account_Mongo.MiddleName = account.MiddleName;
                    account_Mongo.LastName = account.LastName;
                    account_Mongo.EmailAddress = account.EmailAddress;
                    account_Mongo.SponsorID = account.SponsorID;
                    account_Mongo.EnrollerID = account.EnrollerID;
                    account_Mongo.EnrollmentDateUTC = account.EnrollmentDateUTC;
                    account_Mongo.IsEntity = account.IsEntity;
                    account_Mongo.AccountStatusChangeReasonID = account.AccountStatusChangeReasonID;
                    account_Mongo.AccountStatusID = account.AccountStatusID;
                    account_Mongo.EntityName = account.EntityName;

                    account_Mongo.BirthdayUTC = account.BirthdayUTC;
                    account_Mongo.TerminatedDateUTC = account.TerminatedDateUTC;
                    account_Mongo.DefaultLanguageID = account.DefaultLanguageID;

                    account_Mongo.AccountPhones = account.AccountPhones;
                    account_Mongo.Addresses = account.AccountAddresses.Select(a => a.Addresses).Where(a => a.AddressTypeID == 1).ToList();
                    account_Mongo.AccountAdditionalTitulars = account.AccountAdditionalTitulars.ToList();

                    accounts_Mongo.Add(account_Mongo);
                }

                accountCollection.InsertManyAsync(accounts_Mongo);
            }
        }

        public void MigratePeriods(string country)
        {
            IMongoCollection<Periods_Mongo> periodsCollection = encoreMongo_Context.PeriodsProvider(country);

            IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();
            periodsCollection.DeleteMany(new BsonDocument { });
            var total = periodsRepository.GetPagedList(null, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var periods = periodsRepository.GetPagedList(null, p => p.OrderBy(o => o.PeriodID), null, i, 10000, true).Items;

                List<Periods_Mongo> periods_Mongo = new List<Periods_Mongo>();
                foreach (var period in periods)
                {
                    Periods_Mongo registro = new Periods_Mongo()
                    {
                        PeriodID = period.PeriodID,

                        StartDate = period.StartDate,
                        EndDate = period.EndDate,
                        ClosedDate = period.ClosedDate,
                        PlanID = period.PlanID,
                        EarningsViewable = period.EarningsViewable,
                        BackOfficeDisplayStartDate = period.BackOfficeDisplayStartDate,
                        DisbursementsProcessed = period.DisbursementsProcessed,
                        Description = period.Description,
                        StartDateUTC = period.StartDateUTC,
                        EndDateUTC = period.EndDateUTC,
                        LockDate = period.LockDate
                    };

                    periods_Mongo.Add(registro);
                }

                periodsCollection.InsertMany(periods_Mongo);
            }
        }

        public void MigrateTermTranslations(string country)
        {
            IRepository<TermTranslationsMongo> termTranslationsRepository = unitOfWork_Core.GetRepository<TermTranslationsMongo>();

            IMongoCollection<TermTranslations_Mongo> termTranslationsCollection = encoreMongo_Context.TermTranslationsProvider(country);
            termTranslationsCollection.DeleteMany(new BsonDocument { });

            var total = termTranslationsRepository.GetPagedList(null, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var termTranslations = termTranslationsRepository.GetPagedList(null, t => t.OrderBy(o => o.TermTranslationID), t => t.Include(l => l.Languages), i, 10000, true).Items;

                List<TermTranslations_Mongo> termTranslations_Mongo = new List<TermTranslations_Mongo>();
                foreach (var termTranslation in termTranslations)
                {
                    TermTranslations_Mongo registro = new TermTranslations_Mongo()
                    {
                        TermTranslationID = termTranslation.TermTranslationID,
                        LanguageID = termTranslation.LanguageID,
                        Active = termTranslation.Active,
                        LastUpdatedUTC = termTranslation.LastUpdatedUTC,
                        Term = termTranslation.Term,
                        TermName = termTranslation.TermName,
                        LanguageCode = termTranslation.Languages.LanguageCode.ToLower()
                    };

                    termTranslations_Mongo.Add(registro);
                }

                termTranslationsCollection.InsertMany(termTranslations_Mongo);
            }
        }

        public void MigrateAccountKPIsDetailsByPeriod(int? periodId = null, string country = null)
        {
            IMongoCollection<AccountKPIsDetails_Mongo> AccountKPIsDetailsCollection = encoreMongo_Context.AccountKPIsDetailsProvider(country);

            if (periodId == null)
            {
                periodId = GetCurrentPeriod();
            }

            var total = accountKPIsDetailsRepository.GetPagedList(a => a.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            AccountKPIsDetailsCollection.DeleteMany(a => a.PeriodID == periodId);

            for (int i = 0; i < ii; i++)
            {
                var AccountKPIsDetailsInformation = accountKPIsDetailsRepository.GetPagedList(a => a.PeriodID == periodId, a => a.OrderBy(o => o.AccountKPIDetailID), null, i, 10000, true).Items;
                IEnumerable<AccountKPIsDetails_Mongo> result = GetAccountKPIsDetails(AccountKPIsDetailsInformation);

                AccountKPIsDetailsCollection.InsertMany(result);
            }
        }

        private IEnumerable<AccountKPIsDetails_Mongo> GetAccountKPIsDetails(IList<AccountKPIsDetails> AccountKPIsDetailsInformation)
        {
            return from kpiInfo in AccountKPIsDetailsInformation
                   select new AccountKPIsDetails_Mongo
                   {
                       AccountKPIDetailID = kpiInfo.AccountKPIDetailID,
                       PeriodID = kpiInfo.PeriodID,
                       SponsorID = kpiInfo.SponsorID,
                       SponsorName = kpiInfo.SponsorName,
                       DownlineID = kpiInfo.DownlineID,
                       DownlineName = kpiInfo.DownlineName,
                       KPICode = kpiInfo.KPICode,
                       Value = kpiInfo.Value,
                       Percentage = kpiInfo.Percentage,
                       DownlinePaidAsTitle = kpiInfo.DownlinePaidAsTitle,
                       CurrencyTypeID = kpiInfo.CurrencyTypeID,
                       AccountSponsorTypeID = kpiInfo.AccountSponsorTypeID,
                       TreeLevel = kpiInfo.TreeLevel,
                       DateModified = kpiInfo.DateModified
                   };
        }

        private int? GetCurrentPeriod()
        {
            int? periodId;
            IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();
            var date = DateTime.Now;
            var result = periodsRepository.GetFirstOrDefault(p => date >= p.StartDateUTC && date <= p.EndDateUTC && p.PlanID == 1, null, null, true);
            periodId = result.PeriodID;
            return periodId;
        }

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

        public void RequirementTitleCalculations(string country)
        {
            IRepository<RequirementTitleCalculations> requirementTitleCalculationsRepository = unitOfWork_Comm.GetRepository<RequirementTitleCalculations>();
            IMongoCollection<RequirementTitleCalculations_Mongo> RequirementTitleCalculations_MongoCollection = encoreMongo_Context.RequirementTitleCalculationsProvider(country);


            var total = requirementTitleCalculationsRepository.GetPagedList(null, null, null, 0, 10000, true);
            int ii = total.TotalPages;


            for (int i = 0; i < ii; i++)
            {
                var RequirementTitleCalculations = requirementTitleCalculationsRepository.GetPagedList(null, a => a.OrderBy(o => o.CalculationtypeID), null, i, 10000, true).Items;
                IEnumerable<RequirementTitleCalculations_Mongo> result = GetRequirementTitleCalculations(RequirementTitleCalculations);
                RequirementTitleCalculations_MongoCollection.InsertMany(result);
            }
        }

        private IEnumerable<RequirementTitleCalculations_Mongo> GetRequirementTitleCalculations(IList<RequirementTitleCalculations> RequirementTitleCalculations)
        {
            return from item in RequirementTitleCalculations
                   select new RequirementTitleCalculations_Mongo
                   {
                       TitleID = item.TitleID,
                       CalculationtypeID = item.CalculationtypeID,
                       PlanID = item.PlanID,
                       MinValue = item.MinValue,
                       MaxValue = item.MaxValue,
                       DateModified = item.DateModified
                   };
        }

        public void RequirementLegs(string country)
        {
            IRepository<RequirementLegs> requirementLegsRepository = unitOfWork_Comm.GetRepository<RequirementLegs>();
            IMongoCollection<RequirementLegs_Mongo> RequirementLegs_MongoCollection = encoreMongo_Context.RequirementLegsProvider(country);

            var total = requirementLegsRepository.GetPagedList(null, null, aa => aa.Include(p => p.Titles), 0, 10000, true);

            int ii = total.TotalPages;


            for (int i = 0; i < ii; i++)
            {
                var RequirementLegs = requirementLegsRepository.GetPagedList(null, a => a.OrderBy(o => o.TitleID), aa => aa.Include(p => p.Titles), i, 10000, true).Items;
                IEnumerable<RequirementLegs_Mongo> result = GetRequirementLegs(RequirementLegs);
                RequirementLegs_MongoCollection.InsertMany(result);
            }
        }

        private IEnumerable<RequirementLegs_Mongo> GetRequirementLegs(IList<RequirementLegs> RequirementTitleCalculations)
        {
            return from item in RequirementTitleCalculations
                   select new RequirementLegs_Mongo
                   {
                       TitleID = item.TitleID,
                       PlanID = item.PlanID,
                       TitleRequired = item.TitleRequired,
                       Generation = item.Generation,
                       Level = item.Level,
                       TitleQty = item.TitleQty,
                       TitleDescription = item.Titles.ClientName
                   };
        }

        public void Performance(string country, int? periodId)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);

            if (periodId == null)
            {
                periodId = GetCurrentPeriod();
            }

            var total = accountInformationRepository.GetPagedList(a => a.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            IRepository<Titles> titlesRepository = unitOfWork_Comm.GetRepository<Titles>();
            var titles = titlesRepository.GetAll().ToList();

            for (int i = 0; i < ii; i++)
            {
                var accountsInformation = accountInformationRepository.GetPagedList(a => a.PeriodID == periodId, a => a.OrderBy(o => o.AccountsInformationID), null, i, 10000, true).Items;
                UpdateIngresosDiarios(country, accountsInformation.Select(x => x.AccountID));
            }
        }

        public void UpdateIngresosDiarios(string country, IEnumerable<int> accountID)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<PerformanceIndicatorDay_Mongo> performanceIndicatorDayCollection = encoreMongo_Context.PerformanceIndicatorDayProvider(country);
            IMongoCollection<EnrrollmentAccountsByDayTemp_Mongo> enrrolmentAccountByDayTemp = encoreMongo_Context.EnrrollmentAccountsByDayTempProvider(country);

            var period = GetCurrentPeriod();
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
                        PeriodID = period ?? 0
                    });
                }
            }

            foreach (var item in listaNuevos)
            {
                var accountRoot = AccountsUtils.RecursivoWithoutSponsor(accountInformationCollection, item.AccountID, period ?? 0);
                foreach (var account in accountRoot)
                {
                    var record = performanceIndicatorDayCollection.Find(ai => ai.AccountID == account.AccountID && ai.PeriodID == period).FirstOrDefault();
                    performanceIndicatorDayCollection.ReplaceOne(ai => ai.AccountID == account.AccountID && ai.PeriodID == period, new PerformanceIndicatorDay_Mongo
                    {
                        AccountID = account.AccountID,
                        DayOfMonth = DateTime.Now.Day,
                        Ingresos = record != null ? record.Ingresos + 1 : 1,
                        PeriodID = period ?? 0
                    }, new UpdateOptions { IsUpsert = true });
                }

                enrrolmentAccountByDayTemp.ReplaceOne(ai => ai.AccountID == item.AccountID && ai.DayOfMonth == DateTime.Now.Day, new EnrrollmentAccountsByDayTemp_Mongo
                {
                    AccountID = item.AccountID,
                    DayOfMonth = DateTime.Now.Day,
                    PeriodID = period ?? 0
                }, new UpdateOptions { IsUpsert = true });
            }
        }


    }
}
