using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Data.Contexts;
using System.Linq;
using Belcorp.Encore.Entities.Entities.Commissions;

namespace Belcorp.Encore.Repositories
{
    public class AccountInformationRepository : Repository<AccountsInformation>, IAccountInformationRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;

        public AccountInformationRepository(EncoreCommissions_Context _dbCommissions_Context) : base(_dbCommissions_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId)
        {
            var result = dbCommissions_Context.AccountsInformation.AsNoTracking().Where(ai => ai.PeriodID == periodId);
            return result;
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodIdAndAccountId(int periodId, List<int> accountIds)
        {
            var result = from ai in dbCommissions_Context.AccountsInformation.AsNoTracking()
                         where ai.PeriodID == periodId && accountIds.Contains(ai.AccountID)
                         select ai;

            return result.ToList();
        }
    }
}
