using System;
using System.Collections.Generic;
using System.Text;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Mongo.Extension;
using MongoDB.Driver;

namespace Belcorp.Encore.Application.Utilities
{
    public class AccountsUtils
    {
        public static IEnumerable<AccountsInformation_Mongo> Recursivo(IMongoCollection<AccountsInformation_Mongo> accountInformationCollection, int period, int sponsor, int accountID)
        {
            var lista = new List<AccountsInformation_Mongo>();
            var objAccountInformation = new AccountsInformation_Mongo();

            objAccountInformation = accountInformationCollection.Find(a => a.AccountID == accountID && a.PeriodID == period, null).FirstOrDefault();

            if (objAccountInformation != null)
            {
                lista.Add(objAccountInformation);

                if (accountID != sponsor)
                {
                    lista.AddRange(Recursivo(accountInformationCollection, period, sponsor, objAccountInformation.SponsorID));
                }
            }

            return lista;
        }

        public static IEnumerable<AccountsInformation_Mongo> RecursivoWithoutSponsor(IMongoCollection<AccountsInformation_Mongo> accountInformationCollection, int accountID, int periodID)
        {
            var lista = new List<AccountsInformation_Mongo>();
            var objAccountInformation = new AccountsInformation_Mongo();

            objAccountInformation = accountInformationCollection.Find(a => a.AccountID == accountID, null).FirstOrDefault();

            if (objAccountInformation != null)
            {
                lista.Add(objAccountInformation);
                if (objAccountInformation.AccountID != objAccountInformation.SponsorID && objAccountInformation.SponsorID != 0)
                {
                    lista.AddRange(RecursivoWithoutSponsor(accountInformationCollection, objAccountInformation.SponsorID, periodID));
                }
            }

            return lista;
        }

        public static IEnumerable<AccountsInformation_MongoWithAccountAndSponsor> RecursivoShortName(IMongoCollection<AccountsInformation_Mongo> accountInformationCollection, int period, int sponsor, int accountID,IMongoCollection<Accounts_Mongo> accountsCollection, string country)
        {
            var lista = new List<AccountsInformation_MongoWithAccountAndSponsor>();

            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.AccountID, accountID);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, period);

            var item = accountInformationCollection
                    .Aggregate()
                    .Match(filterDefinition)
                    .Lookup<AccountsInformation_Mongo, Accounts_Mongo, AccountsInformation_MongoWithAccountAndSponsor>(
                        accountsCollection,
                        ai => ai.AccountID,
                        a => a.AccountID,
                        r => r.Account
                    )
                   .Unwind(a => a.Account, new AggregateUnwindOptions<AccountsInformation_MongoWithAccountAndSponsor> { PreserveNullAndEmptyArrays = true })
                .FirstOrDefault();

            if (item != null)
            {
                item.country = country;
                lista.Add(item);

                if (accountID != sponsor)
                {
                    lista.AddRange(RecursivoShortName(accountInformationCollection, period, sponsor, item.SponsorID, accountsCollection, country));
                }
            }
            return lista;
        }

    }
}
