using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Application.Services.Interfaces;
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

        public PagedList<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter, string  country)
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

            if(filter.PQVFrom.HasValue)
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
                };
            }


            var result = accountInformationCollection
                .Aggregate()
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
                .Unwind(a => a.Account, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true } )
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
            if (accountRoot == null)
            {
                return null;
            }

            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, filter.PeriodId);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.SponsorID, accountRoot.AccountID);

            List<string> accountStatusExcluded = new List<string>() { "BegunEnrollment", "Terminated", "BegunEnrollment", "Cessada", "Cadastrada" };
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
                a.LEVEL = a.LEVEL - accountRoot.LEVEL;
                a.Generation = a.Generation - accountRoot.Generation;
            });

            return result;
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
                .FirstOrDefault();

            if (result == null)
            {
                return new AccountsInformation_MongoWithAccountAndSponsor();
            }

            result.LEVEL = result.LEVEL - accountRoot.LEVEL;
            return result;
        }
    }
}