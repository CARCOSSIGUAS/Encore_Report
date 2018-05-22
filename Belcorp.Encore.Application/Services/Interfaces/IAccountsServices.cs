using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services
{
    public interface IAccountsService
    {
        Task<List<Accounts_Mongo>> GetListAccounts(int accountId);
        Task<Accounts_Mongo> GetAccountFromSingleSignOnToken(string token, TimeSpan? expiration = null);
    }
}
