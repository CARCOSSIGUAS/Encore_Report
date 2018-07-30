using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Entities.Entities.Commissions;

namespace Belcorp.Encore.Repositories
{
    public class AccountKPIsRepository : Repository<AccountKPIs>, IAccountKPIsRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;

        public AccountKPIsRepository(EncoreCommissions_Context _dbCommissions_Context) : base(_dbCommissions_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
        }

        //public IEnumerable<AccountKPIs> GetListAccountInformationByPeriodIdAndAccountId(int periodId, List<int> accountIds)
        //{
        //    var result = from ai in dbCommissions_Context.AccountsInformation.AsNoTracking()
        //                 where ai.PeriodID == periodId && accountIds.Contains(ai.AccountID)
        //                 select ai;

        //    return result.ToList();
        //}
    }
}