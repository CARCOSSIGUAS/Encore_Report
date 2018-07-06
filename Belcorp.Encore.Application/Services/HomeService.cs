using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using Belcorp.Encore.Entities.Entities.Mongo.Extension;

namespace Belcorp.Encore.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly EncoreMongo_Context encoreMongo_Context;

        public HomeService(IConfiguration configuration)
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
                    if (result != null)
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

        public async Task<PerformanceIndicator_DTO> GetPerformanceIndicator(int accountId, int? periodID, string country)
        {
            IMongoCollection<Periods_Mongo> periodsCollection = encoreMongo_Context.PeriodsProvider(country);
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            Periods_Mongo period = new Periods_Mongo();
            if (periodID == 0 || periodID == null)
            {
                period = GetCurrentPeriod(country);
                periodID = period.PeriodID;
            }
               
            var result = await accountInformationCollection.Find(ai => ai.AccountID == accountId && ai.PeriodID == periodID).ToListAsync();

            var res = result.Select(ai => new PerformanceIndicator_DTO
                {
                    PQV = Math.Round(ai.PQV.Value),
                    DQV = Math.Round(ai.DQV ?? 0),
                    DQVT = Math.Round(ai.DQVT ?? 0),
                    CQL = Math.Round(ai.CQL ?? 0),
                    PaidAsCurrentMonth = ai.PaidAsCurrentMonth ,
                    CareerTitle = ai.CareerTitle,
                    CareerTitle_Desc = ai.CareerTitle_Des,
                    PaidTitle = ai.PaidAsCurrentMonth,
                    PaidTitle_Desc = ai.PaidAsCurrentMonth_Des
                }
            ).FirstOrDefault();

            return res;
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

        public BonusIndicator_DTO GetBonusIndicator(int sponsorID, string country)
        {
            var period = GetCurrentPeriod(country);

            var filter_Payments = Builders<BonusDetails_Mongo>.Filter.Empty;
            filter_Payments &= Builders<BonusDetails_Mongo>.Filter.Eq(b => b.INDICATORPAYMENT, "P");
            filter_Payments &= Builders<BonusDetails_Mongo>.Filter.Eq(b => b.PERIODIDPAYMENT, period.PeriodID);

            var filter_NoPayments = Builders<BonusDetails_Mongo>.Filter.Empty;
            filter_NoPayments &= Builders<BonusDetails_Mongo>.Filter.Eq(b => b.PeriodID, period.PeriodID);
            filter_NoPayments &= Builders<BonusDetails_Mongo>.Filter.Eq(b => b.OrderID, 0);

            var filterDefinition = Builders<BonusDetails_Mongo>.Filter.Empty;
            filterDefinition &= Builders<BonusDetails_Mongo>.Filter.Eq(b => b.SponsorID, sponsorID);
            filterDefinition &= Builders<BonusDetails_Mongo>.Filter.Or(filter_Payments, filter_NoPayments);

            IMongoCollection<BonusDetails_Mongo> bonusDetailsCollection = encoreMongo_Context.BonusDetailsProvider(country);
            var result = bonusDetailsCollection.Aggregate()
                                               .Match(filterDefinition)
                                               .ToList();

            BonusIndicator_DTO bonusDetails_DTO = new BonusIndicator_DTO();

            if (result != null)
            {
                return new BonusIndicator_DTO
                {
                    PayoutAmount = Math.Round(result.Sum(x => x.PayoutAmount) ?? 0),
                    PayoutAmountLevel = Math.Round(result.Where(x => x.BonusClass == "L").Sum(e => e.PayoutAmount) ?? 0),
                    PayoutAmountGeneration = Math.Round(result.Where(x => (x.BonusClass == "M" || x.BonusClass == "G")).Sum(e => e.PayoutAmount) ?? 0),
                    PayoutAmountBonus = Math.Round(result.Where(x => (x.BonusClass == "I" || x.BonusClass == "" || x.BonusClass == null)).Sum(e => e.PayoutAmount) ?? 0)
                };
            }
            return bonusDetails_DTO;
        }

        public List<Accounts_MongoWithAccountsInformation> GetConsultantSearch(string filter, int accountID, string country)
        {
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            var period = GetCurrentPeriod(country);

            var result = new List<Accounts_MongoWithAccountsInformation>();

            var accountRoot = accountInformationCollection.Find(a => a.AccountID == accountID && a.PeriodID == period.PeriodID, null).FirstOrDefault();
            if (accountRoot == null)
            {
                return result;
            }

            var filter_FirstName = Builders<Accounts_Mongo>.Filter.Regex(ai => ai.FirstName, new BsonRegularExpression(filter, "i"));
            var filter_LastName = Builders<Accounts_Mongo>.Filter.Regex(ai => ai.LastName, new BsonRegularExpression(filter, "i"));
            var AccountNumber = Builders<Accounts_Mongo>.Filter.Regex(ai => ai.AccountNumber, new BsonRegularExpression(filter, "i"));
            var filterDefinitionAccounts = Builders<Accounts_Mongo>.Filter.Empty;
            filterDefinitionAccounts &= Builders<Accounts_Mongo>.Filter.Or(filter_FirstName, filter_LastName, AccountNumber);
            
            List<string> accountStatusExcluded = new List<string>() { "Terminated", "Cessada" };
            var filterDefinitionAccountInformations = Builders<Accounts_MongoWithAccountsInformation>.Filter.Empty;
            filterDefinitionAccountInformations &= Builders<Accounts_MongoWithAccountsInformation>.Filter.Nin(ai => ai.AccountInformation.Activity, accountStatusExcluded);
            filterDefinitionAccountInformations &= Builders<Accounts_MongoWithAccountsInformation>.Filter.Eq(ai => ai.AccountInformation.PeriodID, period.PeriodID);
            filterDefinitionAccountInformations &= Builders<Accounts_MongoWithAccountsInformation>.Filter.Gte(ai => ai.AccountInformation.LeftBower, accountRoot.LeftBower);
            filterDefinitionAccountInformations &= Builders<Accounts_MongoWithAccountsInformation>.Filter.Lte(ai => ai.AccountInformation.RightBower, accountRoot.RightBower);
            
            var orderDefinition = Builders<Accounts_MongoWithAccountsInformation>.Sort.Ascending(ai => ai.AccountInformation.AccountName);
            var limit = 10;
            result = accountsCollection
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
                .Sort(orderDefinition)
                .Limit(limit)
                .ToList();

            return result;
        }

        public List<AccountsInformation_MongoWithAccountAndSponsor> GetConsultantLowerPerformance(int? periodID, int accountID, string country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);

            periodID = periodID == null ? GetCurrentPeriod(country).PeriodID : periodID;

            List<AccountsInformation_MongoWithAccountAndSponsor> result = new List<AccountsInformation_MongoWithAccountAndSponsor>();

            var accountRoot = accountInformationCollection.Find(a => a.AccountID == accountID && a.PeriodID == periodID, null).FirstOrDefault();
            if (accountRoot == null)
            {
                return result;
            }

            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, periodID);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.SponsorID, accountRoot.AccountID);

            List<string> accountStatusExcluded = new List<string>() { "BegunEnrollment", "Terminated", "BegunEnrollment", "Cessada", "Cadastrada" };
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Nin(ai => ai.Activity, accountStatusExcluded);

            var totalItems = (int)accountInformationCollection.Find(filterDefinition).Count();

            var orderDefinition = Builders<AccountsInformation_Mongo>.Sort.Ascending(ai => ((int)ai.DQVT));

            result = new List<AccountsInformation_MongoWithAccountAndSponsor>();

            result = accountInformationCollection
                    .Aggregate()
                    .Match(filterDefinition)
                    .Limit(1)
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

        public Periods_Mongo GetCurrentPeriod(string country)
        {
            var datetimeNow = DateTime.Now;
            IMongoCollection<Periods_Mongo> periodsCollection = encoreMongo_Context.PeriodsProvider(country);
            var period = periodsCollection.Find(p => datetimeNow >= p.StartDateUTC && datetimeNow <= p.EndDateUTC && p.PlanID == 1).FirstOrDefault();
            return period;
        }
    }
}
