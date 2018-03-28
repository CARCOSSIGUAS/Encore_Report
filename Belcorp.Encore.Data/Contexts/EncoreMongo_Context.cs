using System;
using System.Collections.Generic;
using System.Text;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Entities.Entities.DTO;
using MongoDB.Driver;

namespace Belcorp.Encore.Data.Contexts
{
    public class EncoreMongo_Context
    {
        public IMongoDatabase Database;
        public EncoreMongo_Context()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            Database = client.GetDatabase("Encore");
        }

        #region Collections
        public IMongoCollection<AccountsInformation_DTO> AccountsInformationProvider => Database.GetCollection<AccountsInformation_DTO>("AccountsInformation");
        public IMongoCollection<Accounts_DTO> AccountsProvider => Database.GetCollection<Accounts_DTO>("Accounts");
        #endregion
    }
}
