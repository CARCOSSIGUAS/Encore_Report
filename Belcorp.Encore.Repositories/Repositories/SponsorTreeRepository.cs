using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;
using Belcorp.Encore.Entities.Entities;

namespace Belcorp.Encore.Repositories.Repositories
{
    public class SponsorTreeRepository : Repository<SponsorTree>
    {
        protected readonly EncoreCommissions_Context _dbCommissions_Context;

        public SponsorTreeRepository(EncoreCommissions_Context dbCommissions_Context) : base(dbCommissions_Context)
        {
            _dbCommissions_Context = dbCommissions_Context;
        }

        public void lista()
        {
            var sponsor = from st in _dbCommissions_Context.SponsorTree join
                          stm in _dbCommissions_Context.SponsorTree on
                          st.SortPath equals stm.SortPath
                          select st;
        }

    }
}
