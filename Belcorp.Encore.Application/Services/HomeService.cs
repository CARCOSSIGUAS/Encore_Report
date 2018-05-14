using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.DTO;
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

        public HomeService (IOptions<Settings> settings)
        {
            encoreMongo_Context = new EncoreMongo_Context(settings);
        }

        public async Task<AccountHomeHeader_DTO> GetHeader(int accountId)
        {
            try
            {
                var account = await encoreMongo_Context.AccountsProvider.Find(a => a.AccountID == accountId, null).FirstOrDefaultAsync();

                if (account != null)
                {
                    var datetimeNow = DateTime.Now;
                    var period = encoreMongo_Context.PeriodsProvider.Find(p => datetimeNow >= p.StartDateUTC && datetimeNow <= p.EndDateUTC && p.PlanID == 1).FirstOrDefault();

                    var result = encoreMongo_Context.AccountsInformationProvider.Find(ai => ai.AccountID == account.AccountID &&
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
                diffDays = (endDate - datetimeNow).Value.TotalDays;
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

        public async Task<PerformanceIndicator_DTO> GetPerformanceIndicator(int accountId)
        {
            var datetimeNow = DateTime.Now;
            var period = encoreMongo_Context.PeriodsProvider.Find(p => datetimeNow >= p.StartDateUTC && datetimeNow <= p.EndDateUTC && p.PlanID == 1).FirstOrDefault();

            var result = await encoreMongo_Context.AccountsInformationProvider.Find(ai => ai.AccountID == accountId &&
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
