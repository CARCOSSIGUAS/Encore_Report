using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;
using Belcorp.Encore.Entities.Entities;
using System.Collections;
using Belcorp.Encore.Repositories.Interfaces;

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
