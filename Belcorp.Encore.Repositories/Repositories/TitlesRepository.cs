using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;

namespace Belcorp.Encore.Repositories
{
    public class TitlesRepository : Repository<Titles>, ITitlesRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;

        public TitlesRepository(EncoreCommissions_Context _dbCommissions_Context) : base(_dbCommissions_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
        }
    }
}
