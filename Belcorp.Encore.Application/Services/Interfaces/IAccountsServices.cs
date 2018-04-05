using Belcorp.Encore.Entities.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services
{
    public interface IAccountsService
    {
        void Migrate_Accounts();
        Task<List<Accounts_DTO>> GetListAccounts(int accountId);
    }
}
