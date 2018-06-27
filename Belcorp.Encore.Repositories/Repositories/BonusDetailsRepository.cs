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
    public class BonusDetailsRepository : Repository<BonusDetails>, IBonusDetailsRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;

        public BonusDetailsRepository(EncoreCommissions_Context _dbCommissions_Context) : base(_dbCommissions_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
        }

        public IEnumerable<BonusDetails> GetBonusDetails(string country = null, int? periodId = null)
        {
            var result = dbCommissions_Context.BonusDetails.AsNoTracking().Where(ai => ai.PeriodID == periodId);
            return result;
        }
    }
}
