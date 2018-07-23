using Belcorp.Encore.Entities.Entities.DTO;
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
        public IMongoCollection<AccountsInformation_Mongo> AccountsInformationProvider(string country)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

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

        public IMongoCollection<Accounts_Mongo> AccountsProvider(string country)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

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

        public IMongoCollection<Periods_Mongo> PeriodsProvider(string country)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

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
      
        public IMongoCollection<TermTranslations_Mongo> TermTranslationsProvider(string country)
        {
            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<TermTranslations_Mongo>("TermTranslations");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<AccountKPIsDetails_Mongo> AccountKPIsDetailsProvider(string country)
        {
            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<AccountKPIsDetails_Mongo>("AccountKPIsDetails");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<BonusDetails_Mongo> BonusDetailsProvider(string country)
        {
            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<BonusDetails_Mongo>("BonusDetails");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<TransactionMonitor_Mongo> TransactionMonitorProvider(string country)
        {
            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<TransactionMonitor_Mongo>("TransactionMonitor");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<PerformanceIndicatorDay_Mongo> PerformanceIndicatorDayProvider(string country)
        {
            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<PerformanceIndicatorDay_Mongo>("PerformanceIndicatorDay");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<EnrrollmentAccountsByDayTemp_Mongo> EnrrollmentAccountsByDayTempProvider(string country)
        {
            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<EnrrollmentAccountsByDayTemp_Mongo>("EnrrollmentAccountsByDayTemp");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<RequirementTitleCalculations_Mongo> RequirementTitleCalculationsProvider(string country)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<RequirementTitleCalculations_Mongo>("RequirementTitleCalculations");
            }
            else
            {
                return null;
            }

        }

        public IMongoCollection<RequirementLegs_Mongo> RequirementLegsProvider(string country)
        {

            string connectionString = _configuration.GetValue<string>("Encore_Mongo:ConnectionString" + country);
            string database = _configuration.GetValue<string>("Encore_Mongo:Database" + country);

            var client = new MongoClient(connectionString);

            IMongoDatabase _database;

            if (client != null)
            {
                _database = client.GetDatabase(database);

                return _database.GetCollection<RequirementLegs_Mongo>("RequirementLegs");
            }
            else
            {
                return null;
            }

        }


        #endregion
    }
}
