using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Belcorp.Encore.Repositories.Repositories
{
    public class ProcessOnlineRepository : IProcessOnlineRepository
    {
        protected readonly EncoreCommissions_Context _dbCommissions_Context;

        public ProcessOnlineRepository(EncoreCommissions_Context dbCommissions_Context)
        {
            _dbCommissions_Context = dbCommissions_Context;
        }

        public List<AccountKPIs> GetListAccounts_InitializesKPIsInGroup(List<int> accounts, List<int> calculationsTypes, int periodId)
        {

            var result = (
                            from a in _dbCommissions_Context.Accounts join
                                 c in _dbCommissions_Context.CalculationTypes on 1 equals 1 join
                                 ak in _dbCommissions_Context.AccountKPIs on
                                    new { A = a.AccountID,  B = periodId, C = c.CalculationTypeID } equals new { A = ak.AccountID, B = ak.PeriodID, C = ak.CalculationTypeID }
                                 into ps from p in ps.DefaultIfEmpty()
                            where accounts.Contains(a.AccountID) && calculationsTypes.Contains(c.CalculationTypeID) && p == null
                            select new AccountKPIs {
                                    AccountID = a.AccountID,
                                    CalculationTypeID = c.CalculationTypeID,
                                    PeriodID = periodId,
                                    Value = 0,
                                    DateModified = DateTime.Now
                                    
                            }
                          ).ToList();

            return result;

        }
    }
}
