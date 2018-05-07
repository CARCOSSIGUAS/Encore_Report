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
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        private readonly IAccountsRepository accountsRepository;
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAuthenticationService authenticationService;

        public AccountsService
        (
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm,
            IUnitOfWork<EncoreCore_Context> _unitOfWork_Core,
            IAccountsRepository _accountsRepository,
            IAuthenticationService _authenticationService,
            IOptions<Settings> settings
        )
        {
            unitOfWork_Core = _unitOfWork_Core;
            unitOfWork_Comm = _unitOfWork_Comm;
            encoreMongo_Context = new EncoreMongo_Context(settings);
            accountsRepository = _accountsRepository;
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
