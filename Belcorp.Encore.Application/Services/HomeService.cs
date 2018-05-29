using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly EncoreMongo_Context encoreMongo_Context;

        public HomeService (IConfiguration configuration)
        {
            encoreMongo_Context = new EncoreMongo_Context(configuration);
        }

        public async Task<AccountHomeHeader_DTO> GetHeader(int accountId, string country)
        {
            try
            {
                IMongoCollection<Accounts_Mongo> accountCollection = encoreMongo_Context.AccountsProvider(country);
                IMongoCollection<Periods_Mongo> periodsCollection = encoreMongo_Context.PeriodsProvider(country);
                IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);

                var account = await accountCollection.Find(a => a.AccountID == accountId, null).FirstOrDefaultAsync();

                if (account != null)
                {
                    var datetimeNow = DateTime.Now;
                    var period = periodsCollection.Find(p => datetimeNow >= p.StartDateUTC && datetimeNow <= p.EndDateUTC && p.PlanID == 1).FirstOrDefault();

                    var result = accountInformationCollection.Find(ai => ai.AccountID == account.AccountID &&
                                                                                            ai.PeriodID == period.PeriodID
                                                                                     )
                                                                                .ToList()
                                                                                .Select(ai => new AccountHomeHeader_DTO()
                                                                                {
                                                                                    LeftBower = ai.LeftBower,
                                                                                    RightBower = ai.RightBower,
                                                                                    account = account,
                                                                                    CareerTitle = ai.CareerTitle,
                                                                                    CareerTitle_Des = ai.CareerTitle_Des,
                                                                                    periodStartDateUTC = period == null ? null : period.StartDateUTC,
                                                                                    periodEndDateUTC = period == null ? null : period.EndDateUTC,
                                                                                    periodDescription = period == null ? "" : period.Description,
                                                                                    periodId = period == null ? 0 : period.PeriodID,
                                                                                    cantFinalPeriodo = TimeLimitEndPeriod(period == null ? null : period.EndDateUTC)
                                                                                });

                    return result.FirstOrDefault();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception { };
            }
        }

        private static string TimeLimitEndPeriod(DateTime? endDate)
        {
            if (endDate.HasValue)
            {
                string result = "";
                var datetimeNow = DateTime.Now;
                double diffDays = 0;
                diffDays = Math.Round((endDate - datetimeNow).Value.TotalDays);
                switch (diffDays)
                {
                    case Double i when i == 0:
                        result = "HOY";
                        break;
                    case Double i when i > 5:
                        result = endDate.Value.ToString("dd/MM/yyyy");
                        break;
                    default:
                            result = diffDays.ToString() + " días";
                            break;
                };

                return result;
            }

            return "";
        }

        public async Task<PerformanceIndicator_DTO> GetPerformanceIndicator(int accountId, string country)
        {
            var datetimeNow = DateTime.Now;
            IMongoCollection<Periods_Mongo> periodsCollection = encoreMongo_Context.PeriodsProvider(country);
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);

            var period = periodsCollection.Find(p => datetimeNow >= p.StartDateUTC && datetimeNow <= p.EndDateUTC && p.PlanID == 1).FirstOrDefault();

            var result = await accountInformationCollection.Find(ai => ai.AccountID == accountId &&
                                                                                          ai.PeriodID == period.PeriodID
                                                                                   ).ToListAsync();

            return result.Select(ai => new PerformanceIndicator_DTO
                                        {
                                            PQV = ai.PQV,
                                            DQV = ai.DQV,
                                            DQVT = ai.DQVT,
                                            CareerTitle = ai.CareerTitle,
                                            CareerTitle_Desc = ai.CareerTitle_Des,
                                            PaidTitle = ai.PaidAsCurrentMonth,
                                            PaidTitle_Desc = ai.PaidAsCurrentMonth_Des
                                        }
                                ).FirstOrDefault();
        }
    }
}
