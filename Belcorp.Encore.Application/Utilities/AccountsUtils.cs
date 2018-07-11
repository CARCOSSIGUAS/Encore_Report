using System;
using System.Collections.Generic;
using System.Text;
using Belcorp.Encore.Entities.Entities.Mongo;
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
    }
}
