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

        public KpiIndicatorPivot_DTO GetkpisIndicator(int periodID, int SponsorID, int DownLineID, string country = null)
        {
        //    KpiIndicator_DTO kpiIndicator_DTO = new KpiIndicator_DTO();
            List<string> codigos = new List<string> {"DCV","DQV","GCV","GQV" };
            KpiIndicatorPivot_DTO kpiIndicatorPivot_DTO = new KpiIndicatorPivot_DTO();
            foreach (var item in codigos)
            {
                IMongoCollection<AccountKPIsDetails_Mongo> accountKpisDetailsCollection = encoreMongo_Context.AccountKPIsDetailsProvider(country);
                var result = accountKpisDetailsCollection.Find(a => a.PeriodID == periodID && a.SponsorID == SponsorID && a.DownlineID == DownLineID && a.KPICode == item).FirstOrDefault();
                switch (item)
                {
                    case "DCV":
                        kpiIndicatorPivot_DTO.PeriodID = result.PeriodID;
                        kpiIndicatorPivot_DTO.SponsorID = result.SponsorID;
                        kpiIndicatorPivot_DTO.SponsorName = result.SponsorName;
                        kpiIndicatorPivot_DTO.DownlineID = result.DownlineID;
                        kpiIndicatorPivot_DTO.DownlineName = result.DownlineName;
                        kpiIndicatorPivot_DTO.DCV = result.Value;
                        kpiIndicatorPivot_DTO.Percentage = result.Percentage;
                        kpiIndicatorPivot_DTO.DownlinePaidAsTitle = result.DownlinePaidAsTitle;
                        kpiIndicatorPivot_DTO.CurrencyTypeID = result.CurrencyTypeID;
                        kpiIndicatorPivot_DTO.AccountSponsorTypeID = result.AccountSponsorTypeID;
                        kpiIndicatorPivot_DTO.TreeLevel = result.TreeLevel;
                        kpiIndicatorPivot_DTO.DateModified = result.DateModified;
                        break;
                    case "DQV":
                        kpiIndicatorPivot_DTO.DQV = result.Value;
                        break;
                    case "GCV":
                        kpiIndicatorPivot_DTO.GCV = result.Value;
                        break;
                    case "GQV":
                        kpiIndicatorPivot_DTO.GQV = result.Value;
                        break;
                }
            }
            return kpiIndicatorPivot_DTO;
        }
            

        public BonusDetails_DTO GetBonusIndicator(int SponsorID, int periodID, string country)
        {
            BonusDetails_DTO bonusDetails_DTO = new BonusDetails_DTO();
            IMongoCollection<BonusDetails_Mongo> accountKpisDetailsCollection = encoreMongo_Context.BonusDetailsProvider(country);
            var result = accountKpisDetailsCollection.Find(a => a.SponsorID == SponsorID && a.PeriodID == periodID).FirstOrDefault();
            if (result != null)
            {
                return new BonusDetails_DTO
                {
                    BonusDetailID = result.BonusDetailID,
                    SponsorID = result.SponsorID,
                    SponsorName = result.SponsorName,
                    DownlineID = result.DownlineID,
                    DownlineName = result.DownlineName,
                    BonusTypeID = result.BonusTypeID,
                    BonusCode = result.BonusCode,
                    OrderID = result.OrderID,
                    QV = result.QV,
                    CV = result.CV,
                    Percentage = result.Percentage,
                    OriginalAmount = result.OriginalAmount,
                    Adjustment = result.Adjustment,
                    PayoutAmount = result.PayoutAmount,
                    CurrencyTypeID = result.CurrencyTypeID,
                    AccountSponsorTypeID = result.AccountSponsorTypeID,
                    TreeLevel = result.TreeLevel,
                    PeriodID = result.PeriodID,
                    ParentOrderID = result.ParentOrderID,
                    CorpOriginalAmount = result.CorpOriginalAmount,
                    CorpAdjustment = result.CorpAdjustment,
                    CorpPayoutAmount = result.CorpPayoutAmount,
                    CorpCurrencyTypeID = result.CorpCurrencyTypeID,
                    DateModified = result.DateModified,
                    INDICATORPAYMENT = result.INDICATORPAYMENT,
                    PERIODIDPAYMENT = result.PERIODIDPAYMENT
                };
            }
            return bonusDetails_DTO;
        }
    }
}
