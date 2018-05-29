using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAuthenticationService authenticationService;

        public AccountsService
        (
            IAuthenticationService _authenticationService,
            IConfiguration configuration
        )
        {
            encoreMongo_Context = new EncoreMongo_Context(configuration);
            authenticationService = _authenticationService;
        }

        public async Task<List<Accounts_Mongo>> GetListAccounts(int accountId, string country)
        {
            IMongoCollection<Accounts_Mongo> accountColletion = encoreMongo_Context.AccountsProvider(country);

            var result = await accountColletion.Find(a => a.AccountID == accountId).Project(Builders<Accounts_Mongo>.Projection.Exclude("_id")).As<Accounts_Mongo>().ToListAsync();
            return result;
        }


        public async Task<Accounts_Mongo> GetAccountFromSingleSignOnToken(string token, string country, TimeSpan? expiration = null)
        {
            try
            {
                var ssoModel = new SingleSignOnModel_DTO();
                ssoModel.EncodedText = token;
                authenticationService.Decode(ssoModel);

                int n;
                bool isNumeric = int.TryParse(ssoModel.DecodedText, out n);

                int accountID = isNumeric == true && (expiration == null || ssoModel.TimeStamp.Add(expiration.Value) >= DateTime.Now) ? n : 0;

                IMongoCollection<Accounts_Mongo> accountColletion = encoreMongo_Context.AccountsProvider(country);

                var result = await accountColletion.Find(a => a.AccountID == accountID).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
