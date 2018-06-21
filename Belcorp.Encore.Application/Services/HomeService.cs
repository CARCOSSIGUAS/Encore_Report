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
using MongoDB.Bson;

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

                    AccountHomeHeader_DTO accountHomeHeader_DTO = new AccountHomeHeader_DTO()
                    {
                         account = account,
                         periodStartDateUTC = period == null ? null : period.StartDateUTC,
                         periodEndDateUTC = period == null ? null : period.EndDateUTC,
                         periodDescription = period == null ? "" : period.Description,
                         periodId = period == null ? 0 : period.PeriodID,
                         cantFinalPeriodo = TimeLimitEndPeriod(period == null ? null : period.EndDateUTC)
                    };

                    var result = accountInformationCollection.Find(ai => ai.PeriodID == period.PeriodID && ai.AccountID == account.AccountID).FirstOrDefault();
                    if(result != null)
                    {
                        accountHomeHeader_DTO.CareerTitle = result.CareerTitle;
                        accountHomeHeader_DTO.CareerTitle_Des = result.CareerTitle_Des;
                    }

                    return accountHomeHeader_DTO;
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

            var result = await accountInformationCollection.Find(ai => ai.AccountID == accountId && ai.PeriodID == period.PeriodID).ToListAsync();

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

        public KpisIndicator_DTO GetKpisIndicator(int periodID, int SponsorID, int DownLineID, string country)
        {
            List<string> codigos = new List<string> { "DCV", "DQV", "GCV", "GQV" };
            IMongoCollection<AccountKPIsDetails_Mongo> accountKpisDetailsCollection = encoreMongo_Context.AccountKPIsDetailsProvider(country);
            var main = accountKpisDetailsCollection.Find(a => a.PeriodID == periodID && a.SponsorID == SponsorID && a.DownlineID == DownLineID).FirstOrDefault();

            if (main != null)
            {
                KpisIndicator_DTO kpisIndicator_DTO = new KpisIndicator_DTO()
                {
                    PeriodID = main.PeriodID,
                    SponsorID = main.SponsorID,
                    SponsorName = main.SponsorName,
                    DownlineID = main.DownlineID,
                    DownlineName = main.DownlineName,
                    Percentage = main.Percentage,
                    DownlinePaidAsTitle = main.DownlinePaidAsTitle,
                    CurrencyTypeID = main.CurrencyTypeID,
                    AccountSponsorTypeID = main.AccountSponsorTypeID,
                    TreeLevel = main.TreeLevel,
                    DateModified = main.DateModified
                };

                foreach (var item in codigos)
                {
                    var result = accountKpisDetailsCollection.Find(a => a.PeriodID == periodID && a.SponsorID == SponsorID && a.DownlineID == DownLineID && a.KPICode == item).FirstOrDefault();
                    switch (item)
                    {
                        case "DCV":
                            kpisIndicator_DTO.DCV = result.Value;
                            break;
                        case "DQV":
                            kpisIndicator_DTO.DQV = result.Value;
                            break;
                        case "GCV":
                            kpisIndicator_DTO.GCV = result.Value;
                            break;
                        case "GQV":
                            kpisIndicator_DTO.GQV = result.Value;
                            break;
                    }
                }

                return kpisIndicator_DTO;
            }

            return new KpisIndicator_DTO();
        }

        public BonusIndicator_DTO GetBonusIndicator(int periodID, int SponsorID, int DownlineID, string country)
        {
            BonusIndicator_DTO bonusDetails_DTO = new BonusIndicator_DTO();
            IMongoCollection<BonusDetails_Mongo> bonusDetailsCollection = encoreMongo_Context.BonusDetailsProvider(country);
            var result = bonusDetailsCollection.Find(b => b.PeriodID == periodID && b.SponsorID == SponsorID && b.DownlineID == DownlineID).FirstOrDefault();

            if (result != null)
            {
                return new BonusIndicator_DTO
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

        public List<Accounts_MongoWithAccountsInformation> GetConsultantSearch(string filter, string country)
        {
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            var period = GetCurrentPeriod(country);

            var filterDefinitionAccounts = Builders<Accounts_Mongo>.Filter.Empty;
            filterDefinitionAccounts &= Builders<Accounts_Mongo>.Filter.Regex(ai => ai.FirstName, new BsonRegularExpression(filter, "i"));

            var filterDefinitionAccountInformations = Builders<Accounts_MongoWithAccountsInformation>.Filter.Empty;
            filterDefinitionAccountInformations &= Builders<Accounts_MongoWithAccountsInformation>.Filter.Eq(ai => ai.AccountInformation.PeriodID, period.PeriodID);

            var result = accountsCollection
                .Aggregate()
                .Match(filterDefinitionAccounts)
                .Lookup<Accounts_Mongo, AccountsInformation_Mongo, Accounts_MongoWithAccountsInformation>(
                    accountInformationCollection,
                    a => a.AccountID,
                    ai => ai.AccountID,
                    r => r.AccountInformation
                )
                .Unwind(a => a.AccountInformation, new AggregateUnwindOptions<Accounts_MongoWithAccountsInformation> { PreserveNullAndEmptyArrays = true })
                .Match(filterDefinitionAccountInformations)
                .ToList();

            if (result == null)
            {
                return new List<Accounts_MongoWithAccountsInformation>();
            }
            return result;
        }

        private Periods_Mongo GetCurrentPeriod(string country)
        {
            var datetimeNow = DateTime.Now;
            IMongoCollection<Periods_Mongo> periodsCollection = encoreMongo_Context.PeriodsProvider(country);
            var period = periodsCollection.Find(p => datetimeNow >= p.StartDateUTC && datetimeNow <= p.EndDateUTC && p.PlanID == 1).FirstOrDefault();
            return period;
        }

        public class Accounts_MongoWithAccountsInformation : Accounts_Mongo
        {
            public AccountsInformation_Mongo AccountInformation { get; set; }
        }
    }
}
