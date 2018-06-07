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

namespace Belcorp.Encore.Application.Services
{
    public class MigrateService : IMigrateService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAccountInformationRepository accountInformationRepository;

        public MigrateService
        (
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm,
            IUnitOfWork<EncoreCore_Context> _unitOfWork_Core,
            IAccountInformationRepository _accountInformationRepository,
            IOptions<Settings> settings
        )
        {
            unitOfWork_Comm = _unitOfWork_Comm;
            unitOfWork_Core = _unitOfWork_Core;
            accountInformationRepository = _accountInformationRepository;
            encoreMongo_Context = new EncoreMongo_Context(settings);
        }

        public void MigrateAccountInformationByPeriod(int? periodId = null)
        {
            if (periodId == null)
            {
                IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();
                var date = DateTime.Now;
                var result = periodsRepository.GetFirstOrDefault(p => date >= p.StartDateUTC && date <= p.EndDateUTC && p.PlanID == 1, null, null, true);
                periodId = result.PeriodID;
            }

            var total = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            IRepository<Titles> titlesRepository = unitOfWork_Comm.GetRepository<Titles>();
            var titles = titlesRepository.GetAll().ToList();

            encoreMongo_Context.AccountsInformationProvider.DeleteMany(p => p.PeriodID == periodId);

            for (int i = 0; i < ii; i++)
            {
                var accountsInformation = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 10000, true).Items;
                IEnumerable<AccountsInformation_Mongo> result = GetAccountInformations(titles, accountsInformation);

                encoreMongo_Context.AccountsInformationProvider.InsertMany(result);
            }
        }

        public IEnumerable<AccountsInformation_Mongo> GetAccountInformations(List<Titles> titles, IList<AccountsInformation> accountsInformation, Activities activity = null, int ? AccountID = null)
        {
            return from accountsInfo in accountsInformation
                   join titlesInfo_Career in titles on Int32.Parse(accountsInfo.CareerTitle) equals titlesInfo_Career.TitleID
                   join titlesInfo_Paid in titles on Int32.Parse(accountsInfo.PaidAsCurrentMonth) equals titlesInfo_Paid.TitleID
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
                       NewStatus = accountsInfo.NewStatus,
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
                       LeftBower = accountsInfo.LeftBower,
                       RightBower = accountsInfo.RightBower,
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
                       UplineLeaderM3 = accountsInfo.UplineLeaderM3,
                       UplineLeaderM3Name = accountsInfo.UplineLeaderM3Name,
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

                       Activity = (AccountID.HasValue && AccountID == accountsInfo.AccountID && activity != null) ? activity.ActivityStatuses.ExternalName : accountsInfo.Activity
                   };
        }

        public void MigrateAccounts()
        {
            IRepository<Entities.Entities.Core.Accounts> accountsRepository = unitOfWork_Core.GetRepository<Entities.Entities.Core.Accounts>();
            encoreMongo_Context.AccountsProvider.DeleteMany(new BsonDocument { });
            var total = accountsRepository.GetPagedList(null, null, null, 0, 5000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var accounts = accountsRepository.GetPagedList(null, null, a => a.Include(p => p.AccountPhones).Include(p => p.AccountAddresses).ThenInclude(p => p.Addresses), i, 5000, true).Items;

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

                    account_Mongo.AccountPhones = account.AccountPhones;
                    account_Mongo.Addresses = account.AccountAddresses.Select(a => a.Addresses).Where(a => a.AddressTypeID == 1).ToList();

                    accounts_Mongo.Add(account_Mongo);
                }

                encoreMongo_Context.AccountsProvider.InsertManyAsync(accounts_Mongo);
            }
        }

        public void MigratePeriods()
        {
            IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();
            encoreMongo_Context.PeriodsProvider.DeleteMany(new BsonDocument { });
            var total = periodsRepository.GetPagedList(null, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var periods = periodsRepository.GetPagedList(null, null, null, i, 10000, true).Items;

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

                encoreMongo_Context.PeriodsProvider.InsertMany(periods_Mongo);
            }
        }

        public void MigrateTermTranslations()
        {
            IRepository<TermTranslationsMongo> termTranslationsRepository = unitOfWork_Core.GetRepository<TermTranslationsMongo>();
            encoreMongo_Context.TermTranslationsProvider.DeleteMany(new BsonDocument { });

            var total = termTranslationsRepository.GetPagedList(null, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var termTranslations = termTranslationsRepository.GetPagedList(null, null, t => t.Include(l => l.Languages), i, 10000, true).Items;

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

                encoreMongo_Context.TermTranslationsProvider.InsertMany(termTranslations_Mongo);
            }
        }
    }
}
