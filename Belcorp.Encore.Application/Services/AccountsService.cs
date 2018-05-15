using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services
{
    public class AccountsService : IAccountsService
    {
		//private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
		//private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

		//private readonly IAccountsRepository accountsRepository;
		private readonly EncoreMongo_Context encoreMongo_Context;
		//private readonly IAuthenticationService authenticationService;

		public AccountsService(IConfiguration configuration) {

			encoreMongo_Context = new EncoreMongo_Context(configuration);

		}		

		public void Migrate_Accounts()
        {

			//	Eliminar todo el contexto de Accounts Mongo.

			var hola = encoreMongo_Context.Accounts_Mongos;

			hola.DeleteMany(new BsonDocument{ });

			

				

			////	


			
            //IRepository<Accounts> accountsRepository = unitOfWork_Core.GetRepository<Accounts>();
            //encoreMongo_Context.AccountsProvider.DeleteMany(new BsonDocument { });
            //var total = accountsRepository.GetPagedList(null, null, null, 0, 10000, true);
            //int ii = total.TotalPages;

            //for (int i = 0; i < ii; i++)
            //{
            //    var accounts = accountsRepository.GetPagedList(null, null, a => a.Include(p => p.AccountPhones), i, 10000, true).Items;

            //    List<Accounts_Mongo> accounts_Mongo = new List<Accounts_Mongo>();
            //    foreach (var account in accounts)
            //    {
            //        Accounts_Mongo account_Mongo = new Accounts_Mongo();

            //        account_Mongo.CountryID = 0;
            //        account_Mongo.AccountID = account.AccountID;

            //        account_Mongo.AccountNumber = account.AccountNumber;
            //        account_Mongo.AccountTypeID = account.AccountTypeID;
            //        account_Mongo.FirstName = account.FirstName;
            //        account_Mongo.MiddleName = account.MiddleName;
            //        account_Mongo.LastName = account.LastName;
            //        account_Mongo.EmailAddress = account.EmailAddress;
            //        account_Mongo.SponsorID = account.SponsorID;
            //        account_Mongo.EnrollerID = account.EnrollerID;
            //        account_Mongo.EnrollmentDateUTC = account.EnrollmentDateUTC;
            //        account_Mongo.IsEntity = account.IsEntity;
            //        account_Mongo.AccountStatusChangeReasonID = account.AccountStatusChangeReasonID;
            //        account_Mongo.AccountStatusID = account.AccountStatusID;
            //        account_Mongo.EntityName = account.EntityName;

            //        account_Mongo.BirthdayUTC = account.BirthdayUTC;
            //        account_Mongo.TerminatedDateUTC = account.TerminatedDateUTC;

            //        account_Mongo.AccountPhones = account.AccountPhones;

            //        accounts_Mongo.Add(account_Mongo);
            //    }

            //    encoreMongo_Context.AccountsProvider.InsertMany(accounts_Mongo);
            //}
        }

        public async Task<List<Accounts_Mongo>> GetListAccounts(int accountId, string country)
        {
			try
			{
				IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.Accounts(country);

				//var documento = accountsCollection.ToJson();

				List<FilterDefinition<Accounts_Mongo>> filtros = new List<FilterDefinition<Accounts_Mongo>>();

				var IdFilter = Builders<Accounts_Mongo>.Filter.Eq(r => r.AccountTypeID, accountId);

				filtros.Add(IdFilter);

				var allFilter = Builders<Accounts_Mongo>.Filter.And(filtros);

				var lista = accountsCollection.Find(allFilter).ToList();

				return lista;


			}
			catch (Exception ex) {

				return null;
			}
			
        }


        public async Task<Accounts> GetAccountFromSingleSignOnToken(string token, TimeSpan? expiration = null)
        {
			return null;

            //try
            //{
            //    var ssoModel = new SingleSignOnModel_DTO();
            //    ssoModel.EncodedText = token;
            //    authenticationService.Decode(ssoModel);

            //    int n;
            //    bool isNumeric = int.TryParse(ssoModel.DecodedText, out n);

            //    int accountID = isNumeric == true && (expiration == null || ssoModel.TimeStamp.Add(expiration.Value) >= DateTime.Now) ? n : 0;

            //    IRepository<Accounts> accountsRepository = unitOfWork_Core.GetRepository<Accounts>();
            //    var result = await accountsRepository.GetFirstOrDefaultAsync(a => a.AccountID == accountID, null, null, true);
            //    return result;
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }
    }
}
