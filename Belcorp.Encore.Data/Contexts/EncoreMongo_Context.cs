using Belcorp.Encore.Entities.Entities.Mongo;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Belcorp.Encore.Data.Contexts
{
    public class EncoreMongo_Context
    {
        private readonly IConfiguration _configuration;
        public IMongoDatabase Database = null;

        public EncoreMongo_Context(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
      
        #region Collections
        public IMongoCollection<AccountsInformation_Mongo> AccountsInformationProvider(string pais)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + pais);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + pais);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<AccountsInformation_Mongo>("AccountsInformation");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<Accounts_Mongo> AccountsProvider(string pais)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + pais);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + pais);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<Accounts_Mongo>("Accounts");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<Periods_Mongo> PeriodsProvider(string pais)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + pais);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + pais);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<Periods_Mongo>("Periods");
            }
            else
            {
                return null;
            }

        }
      
        public IMongoCollection<TermTranslations_Mongo> TermTranslationsProvider(string pais)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + pais);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + pais);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<TermTranslationsProvider>("TermTranslations");
            }
            else
            {
                return null;
            }

        }
      
        #endregion
    }
}
