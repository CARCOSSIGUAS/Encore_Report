using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;
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
            IOptions<Settings> settings
        )
        {
            encoreMongo_Context = new EncoreMongo_Context(settings);
        }

        public PagedList<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter)
        {
            var accountRoot = encoreMongo_Context.AccountsInformationProvider.Find(a => a.AccountID == filter.AccountId && a.PeriodID == filter.PeriodId, null).FirstOrDefault();
            if (accountRoot == null)
            {
                return null;
            }

            var listLevelIds = GetIdsFromString(filter.LevelIds).Select(s => int.Parse(s)).ToList();
            var listGenerationIds = GetIdsFromString(filter.GenerationIds).Select(s => int.Parse(s)).ToList();
            var listTitleIds = GetIdsFromString(filter.TitleIds);
            var listAccountStatusIds = GetIdsFromString(filter.AccountStatusIds);

            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, filter.PeriodId);

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Gte(ai => ai.LeftBower, accountRoot.LeftBower);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Lte(ai => ai.RightBower, accountRoot.RightBower);

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Where(ai =>
                                                                                        (ai.PQV >= filter.PQVFrom && ai.PQV <= filter.PQVTo) &&
                                                                                        (ai.DQV >= filter.DQVFrom && ai.DQV <= filter.DQVTo) &&

                                                                                        (listLevelIds.Contains((int)ai.LEVEL) || listLevelIds.Count == 0) &&
                                                                                        (listGenerationIds.Contains((int)ai.Generation) || listGenerationIds.Count == 0)
                                                                                 );
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
                var accountSponsor = encoreMongo_Context.AccountsInformationProvider.Find(a =>
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

            var totalItems = (int)encoreMongo_Context.AccountsInformationProvider.Find(filterDefinition).Count();

            var orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ai.LEVEL);
            if (!String.IsNullOrEmpty(filter.OrderBy))
            {
                switch (filter.OrderBy)
                {
                    case "1":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.CareerTitle);
                        break;
                    case "2":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.PaidAsCurrentMonth);
                        break;
                    case "3":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.PQV);
                        break;
                    case "4":
                        orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Descending(ai => ai.JoinDate);
                        break;
                };
            }

            var projection = Builders<BsonDocument>.Projection.Include("AccountID")
                                                              .Include("AccountNumber")
                                                              .Include("AccountName")
                                                              .Include("JoinDate")
                                                              .Include("EmailAddress")
                                                              .Include("Generation")
                                                              .Include("LEVEL")
                                                              .Include("Activity")
                                                              .Include("PQV")
                                                              .Include("PCV")
                                                              .Include("DQVT")
                                                              .Include("DQV")
                                                              .Include("CareerTitle")
                                                              .Include("CareerTitle_Des")
                                                              .Include("PaidAsCurrentMonth")
                                                              .Include("PaidAsCurrentMonth_Des")
                                                              .Include("SponsorID")
                                                              .Include("SponsorName")
                                                              .Include("Account")
                                                              .Include("Sponsor")
                                                              .Exclude("_id");

            var result = encoreMongo_Context.AccountsInformationProvider
                .Aggregate()
                .Match(filterDefinition)
                .Sort(orderDefinition)
                .Skip(filter.PageSize * (filter.PageNumber - 1))
                .Limit(filter.PageSize)
                .Lookup("Accounts", "AccountID", "_id", "Account")
                .Unwind("Account")
                .Lookup("Accounts", "SponsorID", "_id", "Sponsor")
                .Unwind("Sponsor")
                .Project<AccountsInformation_MongoWithAccountAndSponsor>(projection)
                .ToList();

            return new PagedList<AccountsInformation_MongoWithAccountAndSponsor>(result, totalItems, filter.PageNumber, filter.PageSize);
        }

        public async Task<IEnumerable<Options_DTO>> GetReportAccountsPeriods()
        {
            var date = DateTime.Now;
            var periodCurrent = await encoreMongo_Context.PeriodsProvider.Find(p => date >= p.StartDateUTC && date <= p.EndDateUTC && p.PlanID == 1).FirstOrDefaultAsync();

            if (periodCurrent != null)
            {
                var periods = await encoreMongo_Context.PeriodsProvider.Find(p => p.PlanID == 1 && p.PeriodID <= periodCurrent.PeriodID)
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
    }

    public class AccountsInformation_MongoWithAccountAndSponsor : AccountsInformation_Mongo
    {
        public Accounts_Mongo Account { get; set; }
        public Accounts_Mongo Sponsor { get; set; }
    }
}