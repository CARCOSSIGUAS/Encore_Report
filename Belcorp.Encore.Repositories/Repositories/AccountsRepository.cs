using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;

namespace Belcorp.Encore.Repositories
{
    public class AccountsRepository : Repository<Accounts>, IAccountsRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;

        public AccountsRepository(EncoreCommissions_Context _dbCommissions_Context) : base(_dbCommissions_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
        }


    }
}
