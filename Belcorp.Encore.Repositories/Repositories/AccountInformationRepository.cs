using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;

namespace Belcorp.Encore.Repositories
{
    public class AccountInformationRepository : Repository<AccountsInformation>, IAccountInformationRepository
    {
        protected readonly EncoreCommissions_Context _dbCommissions_Context;

        public AccountInformationRepository(EncoreCommissions_Context dbCommissions_Context) : base(dbCommissions_Context)
        {
            _dbCommissions_Context = dbCommissions_Context;
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId)
        {
            var result = _dbCommissions_Context.AccountInformation.AsNoTracking().Where(ai => ai.PeriodID == periodId);
            return result;
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByAccountId(int accountId)
        {
            var result = _dbCommissions_Context.AccountInformation.AsNoTracking().Where(ai => ai.AccountID == accountId);
            return result;
        }
    }
}
