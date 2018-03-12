using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;

namespace Belcorp.Encore.Repositories
{
    public class TitlesRepository : Repository<Titles>, ITitlesRepository
    {
        protected readonly EncoreCommissions_Context _dbCommissions_Context;

        public TitlesRepository(EncoreCommissions_Context dbCommissions_Context) : base(dbCommissions_Context)
        {
            _dbCommissions_Context = dbCommissions_Context;
        }
    }
}
