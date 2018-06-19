using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Constants;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
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

        public ReportAccountService
        (
            IConfiguration configuration
        )
        {
            encoreMongo_Context = new EncoreMongo_Context(configuration);
        }

        public PagedList<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter, string  country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);

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

            List<string> accountStatusExcluded = new List<string>() { "Terminated", "Cessada" };

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, filter.PeriodId);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Nin(ai => ai.Activity, accountStatusExcluded);

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Regex(ai => ai.AccountName, new BsonRegularExpression("^((?!TempName).)*$", "i"));

            if (accountRoot.LeftBower.HasValue && accountRoot.RightBower.HasValue)
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.LeftBower, accountRoot.LeftBower);
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.RightBower, accountRoot.RightBower);
            }
            else
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.AccountID, accountRoot.AccountID);
            }

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Where(ai =>
                                                                                        (ai.PQV >= filter.PQVFrom && ai.PQV <= filter.PQVTo) &&
                                                                                        (ai.DQV >= filter.DQVFrom && ai.DQV <= filter.DQVTo)

                                                                                 );
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

            if (filter.AccountNumberSearch.HasValue && filter.AccountNumberSearch != 0)
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.AccountID, filter.AccountNumberSearch);
            }
            if (!String.IsNullOrEmpty(filter.AccountNameSearch))
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Regex(ai => ai.AccountName, new BsonRegularExpression(filter.AccountNameSearch, "i"));
            }

            if (filter.SponsorNumberSearch.HasValue && filter.SponsorNumberSearch > 0)
            {
                var accountSponsor = accountInformationCollection.Find(a =>
                                            a.AccountID == filter.SponsorNumberSearch &&
                                            a.PeriodID == filter.PeriodId, null
                                            ).FirstOrDefault();

                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.LeftBower, accountSponsor.LeftBower);
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.RightBower, accountSponsor.RightBower);
            }

            if (!String.IsNullOrEmpty(filter.SponsorNameSearch))
            {
                filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Regex(ai => ai.SponsorName, new BsonRegularExpression(filter.SponsorNameSearch, "i"));
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

            var orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ((int)ai.LEVEL)).Ascending(ai => ai.AccountName);

            if (!String.IsNullOrEmpty(filter.OrderBy))
            {
                switch (filter.OrderBy)
                {
                    case "1":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.CareerTitle).Ascending(ai => ai.AccountName);
                        break;
                    case "2":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.PaidAsCurrentMonth).Ascending(ai => ai.AccountName);
                        break;
                    case "3":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.PQV).Ascending(ai => ai.AccountName);
                        break;
                    case "4":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.JoinDate).Ascending(ai => ai.AccountName);
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
                .Lookup<AccountsInformation_MongoWithAccountAndSponsor, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                    accountsCollection,
                    ai => ai.SponsorID,
                    s => s.AccountID,
                    r => r.Sponsor
                )
                .Unwind(a => a.Sponsor, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                .ToList();

            result.ForEach(a =>
            {
                a.LEVEL = a.LEVEL - accountRoot.LEVEL;
                a.Generation = a.Generation - accountRoot.Generation;
            });

            return new PagedList<AccountsInformation_MongoWithAccountAndSponsor>(result, totalItems, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<Options_DTO>> GetReportAccountsPeriods(string country)
        {
            var date = DateTime.Now;
            IMongoCollection<Periods_Mongo> periodsCollection = encoreMongo_Context.PeriodsProvider(country);


            var periodCurrent = await periodsCollection.Find(p => date >= p.StartDateUTC && date <= p.EndDateUTC && p.PlanID == 1).FirstOrDefaultAsync();

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

        public AccountsInformation_MongoWithAccountAndSponsor GetConsultantDetails(int periodId, int accountId, string country = null)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);

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
            return result;
        }
    }

    public class AccountsInformation_MongoWithAccountAndSponsor : AccountsInformation_Mongo
    {
        public Accounts_Mongo Account { get; set; }
        public Accounts_Mongo Sponsor { get; set; }
    }
}