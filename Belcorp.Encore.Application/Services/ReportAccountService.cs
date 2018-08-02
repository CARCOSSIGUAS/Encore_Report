using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Application.Utilities;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Mongo.Extension;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Belcorp.Encore.Entities.Constants.Constants;

namespace Belcorp.Encore.Application
{
    public class ReportAccountService : IReportAccountService
    {
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IHomeService homeService;

        public ReportAccountService
        (
            IConfiguration configuration,
            IHomeService _homeService
        )
        {
            homeService = _homeService;
            encoreMongo_Context = new EncoreMongo_Context(configuration);
        }

        public dataForFilter_DTO GetDataForFilterByPeriods(int accountID, int periodID, string country)
        {
            dataForFilter_DTO list = new dataForFilter_DTO();
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);

            periodID = periodID == 0 ? homeService.GetCurrentPeriod(country).PeriodID : periodID;

            var accountRoot = accountInformationCollection.Find(a => a.AccountID == accountID && a.PeriodID == periodID, null).FirstOrDefault();
            if (accountRoot == null)
            {
                return null;
            }
            var generation = accountRoot.Generation;
            var level = accountRoot.LEVEL;

            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, periodID);

            if (accountRoot.LeftBower.HasValue && accountRoot.RightBower.HasValue)
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.LeftBower, accountRoot.LeftBower);
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.RightBower, accountRoot.RightBower);
            }
            else
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.AccountID, accountRoot.AccountID);
            }

            List<string> accountStatusExcluded = new List<string>() { "Terminated", "Cessada" };
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Nin(ai => ai.Activity, accountStatusExcluded);

            var orderDefinitionState = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ai.STATE);
            var orderDefinitionGeneration = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ai.Generation);
            var orderDefinitionLevel = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ai.LEVEL);

            var resultS = accountInformationCollection
                .Aggregate()
                .Match(filterDefinition)
                .Sort(orderDefinitionState)
                .Group(
                    x => x.STATE,
                    g => new { State = g.Select(x => x.STATE).Max() })
                .ToList();

            var resultG = accountInformationCollection
                .Aggregate()
                .Match(filterDefinition)
                .Sort(orderDefinitionGeneration)
                .Group(
                    x => x.Generation,
                    g => new { Generation = g.Select(x => x.Generation).Max() })
                .ToList();

            var resultN = accountInformationCollection
               .Aggregate()
               .Match(filterDefinition)
               .Sort(orderDefinitionLevel)
               .Group(
                   x => x.LEVEL,
                   g => new { Level = g.Select(x => x.LEVEL).Max() })
               .ToList();

            list.state = new List<stateFilter>();
            list.generation = new List<generationFilter>();
            list.level = new List<levelFilter>();
            foreach (var item in resultS)
            {
                if (item.State != null)
                {
                    list.state.Add(new stateFilter()
                    {
                        state = item.State.ToString()
                    });

                }
            }

            foreach (var item in resultG)
            {
                if (item.Generation != null)
                {
                    list.generation.Add(new generationFilter()
                    {
                        generation = Convert.ToInt32(item.Generation ?? 0) - generation ?? 0
                    });

                }
            }

            foreach (var item in resultN)
            {
                if (item.Level != null)
                {
                    list.level.Add(new levelFilter()
                    {
                        level = Convert.ToInt32(item.Level ?? 0) - level ?? 0
                    });

                }
            }

            list.state = list.state.OrderBy(x => x.state).ToList();
            list.generation = list.generation.OrderBy(x => x.generation).ToList();
            list.level = list.level.OrderBy(x => x.level).ToList();
            return list;
        }

        public List<BirthDayAccount_DTO> GetDataBirthday(int accountID, int? periodID, string country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);

            periodID = periodID == 0 ? homeService.GetCurrentPeriod(country).PeriodID : periodID;

            var accountRoot = accountInformationCollection.Find(a => a.AccountID == accountID && a.PeriodID == periodID, null).FirstOrDefault();
            if (accountRoot == null)
            {
                return null;
            }

            var Hoy = DateTime.Now;
            int Day = Hoy.Day;
            int Month = Hoy.Month;
            int Year = Hoy.Year;
            var LastDay = DateTime.DaysInMonth(Year, Month);
            DateTime zeroTime = new DateTime(1, 1, 1);


            var GenerationIds = "0,1";
            //var listGenerationIds = GetIdsFromString(GenerationIds).Select(s => int.Parse(s) + accountRoot.Generation).ToList();
            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, periodID);

            if (accountRoot.LeftBower.HasValue && accountRoot.RightBower.HasValue)
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.LeftBower, accountRoot.LeftBower);
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.RightBower, accountRoot.RightBower);
            }
            else
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.AccountID, accountRoot.AccountID);
            }

            //filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.In(ai => ai.Generation, listGenerationIds);

            List<string> accountStatusExcluded = new List<string>() { "Terminated", "Cessada" };
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Nin(ai => ai.Activity, accountStatusExcluded);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.DayBirthday, Day);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.MonthBirthday, Month);

            var result = new List<AccountsInformation_MongoWithAccountAndSponsor>();

            result = accountInformationCollection
                .Aggregate()
                .Match(filterDefinition)
                .Lookup<AccountsInformation_Mongo, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                    accountsCollection,
                    ai => ai.AccountID,
                    a => a.AccountID,
                    r => r.Account
                )
                .Unwind(a => a.Account, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                .ToList();

            List<BirthDayAccount_DTO> list = new List<BirthDayAccount_DTO>();
            foreach (var item in result)
            {
                if (item.Account != null)
                {
                    if (item.Account.AccountAdditionalTitulars != null && item.Account.AccountAdditionalTitulars.Count > 0)
                    {
                        foreach (var titular in item.Account.AccountAdditionalTitulars)
                        {

                            list.Add(new BirthDayAccount_DTO()
                            {
                                AccountID = item.AccountID,
                                AccountName = titular.FirstName + " " + titular.LastName,
                                BirthdayUTC = titular.Brithday,
                                Generation = -1,
                                LEVEL = -1,
                                HB = titular.Brithday.HasValue ? titular.Brithday.Value.ToString("dd/MM/yyyy") : "",
                                Anios = titular.Brithday.HasValue ? (zeroTime + (DateTime.Now - titular.Brithday.Value)).Year - 1 : 0,
                                Phones = item.Account != null ? String.Join(" - ", item.Account.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
                            });
                        }
                    }
                    if (item.BirthdayUTC != null)
                    {
                        list.Add(new BirthDayAccount_DTO()
                        {
                            AccountID = item.AccountID,
                            AccountName = item.AccountName.ToLower(),
                            HB = item.BirthdayUTC.HasValue ? item.BirthdayUTC.Value.ToString("dd/MM/yyyy") : "",
                            AccountNumber = item.AccountNumber,
                            AccountsInformationID = item.AccountsInformationID,
                            ActiveDownline = item.ActiveDownline,
                            Activity = item.Activity,
                            Address = item.Address,
                            CareerTitle = item.CareerTitle,
                            CareerTitle_Des = item.CareerTitle_Des,
                            City = item.City,
                            ConsultActive = item.ConsultActive,
                            country = country,
                            CQL = item.CQL,
                            CreditAvailable = item.CreditAvailable,
                            DCV = item.DCV,
                            DebtsToExpire = item.DebtsToExpire,
                            DQV = item.DQV,
                            DQVT = item.DQVT,
                            EmailAddress = item.EmailAddress,
                            ExpiredDebts = item.ExpiredDebts,
                            GCV = item.GCV,
                            Generation = item.Generation,
                            GenerationM3 = item.GenerationM3,
                            GQV = item.GQV,
                            IsCommissionQualified = item.IsCommissionQualified,
                            JoinDate = item.JoinDate,
                            //LastName1 = item.LastName1,
                            //LastName2 = item.LastName2,
                            LastOrderDate = item.LastOrderDate,
                            LeftBower = item.LeftBower,
                            LEVEL = item.LEVEL,
                            //Name1 = item.Name1,
                            //Name2 = item.Name2,
                            PaidAsCurrentMonth = item.PaidAsCurrentMonth,
                            PaidAsCurrentMonth_Des = item.PaidAsCurrentMonth_Des,
                            NewStatus = item.NewStatus,
                            PaidAsLastMonth = item.PaidAsLastMonth,
                            PCV = item.PCV,
                            PeriodID = item.PeriodID,
                            PQV = item.PQV,
                            PostalCode = item.PostalCode,
                            Region = item.Region,
                            RightBower = item.RightBower,
                            SortPath = item.SortPath,
                            //SPLastName = item.SPLastName,
                            //SPName = item.SPName,
                            SponsorID = item.SponsorID,
                            SponsorName = item.SponsorName,
                            STATE = item.STATE,
                            TotalDownline = item.TotalDownline,
                            VolumeForCareerTitle = item.VolumeForCareerTitle,
                            Anios = item.BirthdayUTC.HasValue ? (zeroTime + (DateTime.Now - item.BirthdayUTC.Value)).Year - 1 : 0,
                            Phones = item.Account != null ? String.Join(" - ", item.Account.AccountPhones.Select(p => p.PhoneNumber).ToList()) : "",
                        });

                    }
                }


            }

            list = list.OrderBy(x => x.HB).ToList();
            return list;
        }

        public PagedList<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter, string country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);

            filter.PeriodId = filter.PeriodId == null ? homeService.GetCurrentPeriod(country).PeriodID : filter.PeriodId;

            var accountRoot = accountInformationCollection.Find(a => a.AccountID == filter.AccountId && a.PeriodID == filter.PeriodId, null).FirstOrDefault();
            if (accountRoot == null)
            {
                return null;
            }

            var listLevelIds = GetIdsFromString(filter.LevelIds).Select(s => int.Parse(s) + accountRoot.LEVEL).ToList();
            var listGenerationIds = GetIdsFromString(filter.GenerationIds).Select(s => int.Parse(s) + accountRoot.Generation).ToList();
            var listTitleIds = GetIdsFromString(filter.TitleIds);
            var listAccountStatusIds = GetIdsFromString(filter.AccountStatusIds);
            var listAccountEstadoIds = GetIdsFromString(filter.AccountEstadoIds);

            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, filter.PeriodId);

            if (accountRoot.LeftBower.HasValue && accountRoot.RightBower.HasValue)
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.LeftBower, accountRoot.LeftBower);
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.RightBower, accountRoot.RightBower);
            }
            else
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.AccountID, accountRoot.AccountID);
            }

            List<string> accountStatusExcluded = new List<string>() { "Terminated", "Cessada" };
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Nin(ai => ai.Activity, accountStatusExcluded);

            if (filter.PQVFrom.HasValue)
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.PQV, filter.PQVFrom);

            if (filter.PQVTo.HasValue && filter.PQVTo < 10000)
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.PQV, filter.PQVTo);

            if (filter.DQVFrom.HasValue)
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.DQV, filter.DQVFrom);

            if (filter.DQVTo.HasValue && filter.DQVTo < 5000000)
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.DQV, filter.DQVTo);

            if (listLevelIds.Count > 0)
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.In(ai => ai.LEVEL, listLevelIds);

            if (listGenerationIds.Count > 0)
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.In(ai => ai.Generation, listGenerationIds);

            if (listAccountStatusIds.Count > 0)
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.In(ai => ai.Activity, listAccountStatusIds);

            if (listAccountEstadoIds.Count > 0)
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.In(ai => ai.STATE, listAccountEstadoIds);

            if (listTitleIds.Count > 0)
            {
                switch (filter.TitleType)
                {
                    case (int)TitleTypes.Career:
                        filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.In(ai => ai.CareerTitle, listTitleIds);
                        break;
                    case (int)TitleTypes.Paid:
                        filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.In(ai => ai.PaidAsCurrentMonth, listTitleIds);
                        break;
                };
            }

            if (!String.IsNullOrEmpty(filter.StringSearch))
            {
                var filter_AccountName = Builders<AccountsInformation_Mongo>.Filter.Regex(ai => ai.AccountName, new BsonRegularExpression(filter.StringSearch, "i"));
                var filter_AccountNumber = Builders<AccountsInformation_Mongo>.Filter.Regex(ai => ai.AccountNumber, new BsonRegularExpression(filter.StringSearch, "i"));

                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Or(filter_AccountName, filter_AccountNumber);
            }

            if (filter.JoinDateFrom.HasValue)
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.JoinDate, filter.JoinDateFrom);
            }

            if (filter.JoinDateTo.HasValue)
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.JoinDate, filter.JoinDateTo);
            }

            var totalItems = (int)accountInformationCollection.Find(filterDefinition).Count();

            var orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ai.AccountName);

            if (!String.IsNullOrEmpty(filter.OrderBy))
            {
                switch (filter.OrderBy)
                {
                    case "1":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ai.AccountName);
                        break;
                    case "2":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.AccountName);
                        break;
                    case "3":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ((int)ai.LEVEL));
                        break;
                    case "4":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ((int)ai.LeftBower));
                        break;
                };
            }

            var result = accountInformationCollection
                .Aggregate(options: new AggregateOptions
                {
                    AllowDiskUse = true
                })
                .Match(filterDefinition)
                .Sort(orderDefinition)
                .Skip(filter.PageSize * (filter.PageNumber - 1))
                .Limit(filter.PageSize)
                .Lookup<AccountsInformation_Mongo, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                    accountsCollection,
                    ai => ai.AccountID,
                    a => a.AccountID,
                    r => r.Account
                )
                .Unwind(a => a.Account, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                //.Lookup<AccountsInformation_MongoWithAccountAndSponsor, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                //    accountsCollection,
                //    ai => ai.SponsorID,
                //    s => s.AccountID,
                //    r => r.Sponsor
                //)
                //.Unwind(a => a.Sponsor, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                .ToList();

            result.ForEach(a =>
            {
                a.LEVEL = a.LEVEL - accountRoot.LEVEL;
                a.Generation = a.Generation - accountRoot.Generation;
            });

            return new PagedList<AccountsInformation_MongoWithAccountAndSponsor>(result, totalItems, filter.PageNumber, filter.PageSize);
        }

        public List<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoredsThree(ReportAccountsSponsoredsSearch filter, string country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);

            filter.PeriodId = filter.PeriodId == null ? homeService.GetCurrentPeriod(country).PeriodID : filter.PeriodId;

            var accountRoot = accountInformationCollection.Find(a => a.AccountID == filter.AccountId && a.PeriodID == filter.PeriodId, null).FirstOrDefault();
            var accountRootPrincipal = accountInformationCollection.Find(a => a.AccountID == filter.SponsorNumberSearch && a.PeriodID == filter.PeriodId, null).FirstOrDefault();
            if (accountRoot == null)
            {
                return null;
            }

            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, filter.PeriodId);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.SponsorID, accountRoot.AccountID);

            List<string> accountStatusExcluded = new List<string>() { "Terminated", "Cessada" }; //"BegunEnrollment", "Terminated", "BegunEnrollment", "Cessada", "Cadastrada" 
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Nin(ai => ai.Activity, accountStatusExcluded);

            if (!String.IsNullOrEmpty(filter.StringSearch))
            {
                var filter_AccountName = Builders<AccountsInformation_Mongo>.Filter.Regex(ai => ai.AccountName, new BsonRegularExpression(filter.StringSearch, "i"));
                var filter_AccountNumber = Builders<AccountsInformation_Mongo>.Filter.Regex(ai => ai.AccountNumber, new BsonRegularExpression(filter.StringSearch, "i"));

                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Or(filter_AccountName, filter_AccountNumber);
            }

            var totalItems = (int)accountInformationCollection.Find(filterDefinition).Count();

            var orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ((int)ai.LEVEL));

            var result = new List<AccountsInformation_MongoWithAccountAndSponsor>();

            result = accountInformationCollection
                    .Aggregate()
                    .Match(filterDefinition)
                    .Sort(orderDefinition)
                    .Lookup<AccountsInformation_Mongo, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                        accountsCollection,
                        ai => ai.AccountID,
                        a => a.AccountID,
                        r => r.Account
                    )
                    .Unwind(a => a.Account, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                    .ToList();

            result.ForEach(a =>
            {
                a.LEVEL = a.LEVEL - accountRootPrincipal.LEVEL;
                a.Generation = a.Generation - accountRoot.Generation;
            });

            return result;
        }

        public IEnumerable<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsBySponsored(int sponsor, int accountID, string country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);

            var period = homeService.GetCurrentPeriod(country).PeriodID;

            var accountRoot = AccountsUtils.RecursivoShortName(accountInformationCollection, period, sponsor, accountID, accountsCollection, country).ToList();

            if (accountRoot == null)
            {
                return null;
            }
            return accountRoot;
        }

        public async Task<IEnumerable<Options_DTO>> GetReportAccountsPeriods(string country)
        {
            IMongoCollection<Periods_Mongo> periodsCollection = encoreMongo_Context.PeriodsProvider(country);
            var periodCurrent = homeService.GetCurrentPeriod(country);

            if (periodCurrent != null)
            {
                var periods = await periodsCollection.Find(p => p.PlanID == 1 && p.PeriodID <= periodCurrent.PeriodID)
                                                                       .Limit(12)
                                                                       .SortByDescending(o => o.PeriodID)
                                                                       .ToListAsync();
                if (periods != null)
                {
                    List<Options_DTO> list = new List<Options_DTO>();
                    periods.ForEach(a =>
                        list.Add(new Options_DTO() { ID = a.PeriodID, Name = a.Description })
                    );

                    return list;
                }
            }

            return null;
        }

        private List<string> GetIdsFromString(string value)
        {
            List<string> result = new List<string>();
            if (!String.IsNullOrEmpty(value))
            {
                result = value.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            }
            return result;
        }

        public AccountsInformation_MongoWithAccountAndSponsor GetConsultantDetails(int? periodId, int accountId, int accountIdCurrent, string country = null)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);

            periodId = periodId.HasValue ? periodId : homeService.GetCurrentPeriod(country).PeriodID;

            var accountRoot = accountInformationCollection.Find(a => a.AccountID == accountIdCurrent && a.PeriodID == periodId, null).FirstOrDefault();


            if (accountRoot == null)
            {
                return null;
            }

            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, periodId);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.AccountID, accountId);

            var result = accountInformationCollection
                .Aggregate()
                .Match(filterDefinition)
                .Lookup<AccountsInformation_Mongo, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                    accountsCollection,
                    ai => ai.AccountID,
                    a => a.AccountID,
                    r => r.Account
                )
                .Unwind(a => a.Account, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                .Lookup<AccountsInformation_MongoWithAccountAndSponsor, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                    accountsCollection,
                    ai => ai.SponsorID,
                    s => s.AccountID,
                    r => r.Sponsor
                )
                .Unwind(a => a.Sponsor, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                .Lookup<AccountsInformation_MongoWithAccountAndSponsor, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                    accountsCollection,
                    ai => ai.UplineLeader0,
                    l => l.AccountID,
                    r => r.Leader0
                )
                .Unwind(a => a.Leader0, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                .Lookup<AccountsInformation_MongoWithAccountAndSponsor, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                    accountsCollection,
                    ai => ai.UplineLeaderM3,
                    l => l.AccountID,
                    r => r.LeaderM3
                )
                .Unwind(a => a.LeaderM3, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                .FirstOrDefault();

            if (result == null)
            {
                return new AccountsInformation_MongoWithAccountAndSponsor();
            }

            result.LEVEL = result.LEVEL - accountRoot.LEVEL;
            result.Generation = accountRoot.Generation - result.Generation;

            if (result.SponsorID == 10)
            {
                var accountConsultedRoot = accountsCollection.Find(a => a.AccountID == 1, null).FirstOrDefault();
                result.Sponsor = accountConsultedRoot;
            }

            return result;
        }
    }
}