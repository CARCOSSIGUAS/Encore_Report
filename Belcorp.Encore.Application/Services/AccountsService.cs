using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
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
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAuthenticationService authenticationService;

        public AccountsService
        (
            IAuthenticationService _authenticationService,
            IOptions<Settings> settings
        )
        {
            encoreMongo_Context = new EncoreMongo_Context(settings);
            authenticationService = _authenticationService;
        }

        public async Task<List<Accounts_Mongo>> GetListAccounts(int accountId)
        {
            var result = await encoreMongo_Context.AccountsProvider.Find(a => a.AccountID == accountId).Project(Builders<Accounts_Mongo>.Projection.Exclude("_id")).As<Accounts_Mongo>().ToListAsync();
            return result;
        }


        public async Task<Accounts_Mongo> GetAccountFromSingleSignOnToken(string token, TimeSpan? expiration = null)
        {
            try
            {
                var ssoModel = new SingleSignOnModel_DTO();
                ssoModel.EncodedText = token;
                authenticationService.Decode(ssoModel);

                int n;
                bool isNumeric = int.TryParse(ssoModel.DecodedText, out n);

                int accountID = isNumeric == true && (expiration == null || ssoModel.TimeStamp.Add(expiration.Value) >= DateTime.Now) ? n : 0;

                var result = await encoreMongo_Context.AccountsProvider.Find(a => a.AccountID == accountID).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
