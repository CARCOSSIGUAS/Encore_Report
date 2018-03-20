using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;
using Belcorp.Encore.Entities.Entities;

namespace Belcorp.Encore.Repositories
{
    public class AccountKPIsRepository : Repository<AccountKPIs>, IAccountKPIsRepository
    {
        protected readonly EncoreCommissions_Context _dbCommissions_Context;

        public AccountKPIsRepository(EncoreCommissions_Context dbCommissions_Context) : base(dbCommissions_Context)
        {
            _dbCommissions_Context = dbCommissions_Context;
        }
    }
}