using System;
using System.Collections.Generic;
using System.Text;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Entities.Entities.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Belcorp.Encore.Data.Contexts
{
    public class EncoreMongo_Context
    {
        public IMongoDatabase Database = null;
        public EncoreMongo_Context(IOptions<Settings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            if (client != null)
                Database = client.GetDatabase(settings.Value.Database);
        }

        #region Collections
        public IMongoCollection<AccountsInformation_Mongo> AccountsInformationProvider => Database.GetCollection<AccountsInformation_Mongo>("AccountsInformation");
        public IMongoCollection<Accounts_Mongo> AccountsProvider => Database.GetCollection<Accounts_Mongo>("Accounts");
        public IMongoCollection<Periods_Mongo> PeriodsProvider => Database.GetCollection<Periods_Mongo>("Periods");
        #endregion
    }
}
