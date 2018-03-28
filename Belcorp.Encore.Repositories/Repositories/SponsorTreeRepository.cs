using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Repositories.Interfaces;
using Belcorp.Encore.Entities.Entities.Commissions;

namespace Belcorp.Encore.Repositories.Repositories
{
    public class SponsorTreeRepository : Repository<SponsorTree>, ISponsorTreeRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;

        public SponsorTreeRepository(EncoreCommissions_Context _dbCommissions_Context) : base(_dbCommissions_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
        }
    }
}
