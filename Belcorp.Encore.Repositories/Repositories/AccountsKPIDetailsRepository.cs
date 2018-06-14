using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Commissions;

namespace Belcorp.Encore.Repositories
{
    public class AccountsKPIDetailsRepository : Repository<AccountKPIsDetails>, IAccountKPIsDetailsRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;

        public AccountsKPIDetailsRepository(EncoreCommissions_Context _dbCommissions_Context) : base(_dbCommissions_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
        }

        public IEnumerable<AccountKPIsDetails> GetAccountKPIsDetails(string country, int? periodId = null)
        {
            var result = dbCommissions_Context.AccountKPIsDetails.AsNoTracking().Where(ai => ai.PeriodID == periodId);
            return result;
        }
    }
}
