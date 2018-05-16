using Belcorp.Encore.Application.Services;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;
using Microsoft.Extensions.Options;
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

        public PagedList<AccountsInformation_Mongo> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter)
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
            

            var accountsThreeCompleted = from accountsSponsored in encoreMongo_Context.AccountsInformationProvider.AsQueryable()
                                         where
                                         accountsSponsored.PeriodID == filter.PeriodId &&
                                         accountsSponsored.LeftBower >= accountRoot.LeftBower &&
                                         accountsSponsored.RightBower <= accountRoot.RightBower &&

                                         (accountsSponsored.PQV >= filter.PQVFrom && accountsSponsored.PQV <= filter.PQVTo) &&
                                         (accountsSponsored.DQV >= filter.DQVFrom && accountsSponsored.DQV <= filter.DQVTo) &&
                                         (accountsSponsored.JoinDate >= filter.JoinDateFrom && accountsSponsored.JoinDate <= filter.JoinDateTo) &&

                                         (listLevelIds.Contains((int)accountsSponsored.LEVEL) || listLevelIds.Count == 0) &&
                                         (listGenerationIds.Contains((int)accountsSponsored.Generation) || listGenerationIds.Count == 0) &&
                                         (listAccountStatusIds.Contains(accountsSponsored.Activity) || listAccountStatusIds.Count == 0)
                                         select accountsSponsored;

            if (listTitleIds.Count > 0)
            {
                switch (filter.TitleType) {
                    case (int)TitleTypes.Career:
                        accountsThreeCompleted = accountsThreeCompleted.Where(a => listTitleIds.Contains(a.CareerTitle));
                        break;
                    case (int)TitleTypes.Paid:
                        accountsThreeCompleted = accountsThreeCompleted.Where(a => listTitleIds.Contains(a.PaidAsCurrentMonth));
                        break;
                };
            }

            if (filter.AccountNumberSearch.HasValue && filter.AccountNumberSearch != 0)
            {
                accountsThreeCompleted = accountsThreeCompleted.Where(a => a.AccountID == filter.AccountNumberSearch);
            }
            if (!String.IsNullOrEmpty(filter.AccountNameSearch))
            {
                accountsThreeCompleted = accountsThreeCompleted.Where(a => a.AccountName.ToUpper().Contains(filter.AccountNameSearch.ToUpper()));
            }

            if (filter.SponsorNumberSearch.HasValue && filter.SponsorNumberSearch > 0)
            {
                var accountSponsor = encoreMongo_Context.AccountsInformationProvider.Find(a => 
                                            a.AccountID == filter.SponsorNumberSearch && 
                                            a.PeriodID == filter.PeriodId, null
                                            ).FirstOrDefault();

                accountsThreeCompleted = accountsThreeCompleted.Where(a => 
                                            a.LeftBower >= accountSponsor.LeftBower && 
                                            a.RightBower <= accountSponsor.RightBower
                                            );
            }

            if (!String.IsNullOrEmpty(filter.SponsorNameSearch))
            {
                accountsThreeCompleted = accountsThreeCompleted.Where(a => a.SponsorName.ToUpper().Contains(filter.SponsorNameSearch.ToUpper()));
            }

            if (!String.IsNullOrEmpty(filter.OrderBy))
            {
                switch (filter.OrderBy)
                {
                    case "1":
                        accountsThreeCompleted = accountsThreeCompleted.OrderByDescending(a => a.CareerTitle);
                        break;
                    case "2":
                        accountsThreeCompleted = accountsThreeCompleted.OrderByDescending(a => a.PaidAsCurrentMonth);
                        break;
                    case "3":
                        accountsThreeCompleted = accountsThreeCompleted.OrderByDescending(a => a.PQV);
                        break;
                    case "4":
                        accountsThreeCompleted = accountsThreeCompleted.OrderByDescending(a => a.JoinDate);
                        break;
                };
            }
            else
                accountsThreeCompleted = accountsThreeCompleted.OrderBy(a => a.LEVEL);

            return new PagedList<AccountsInformation_Mongo>(accountsThreeCompleted, filter.PageNumber, filter.PageSize);
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
}
