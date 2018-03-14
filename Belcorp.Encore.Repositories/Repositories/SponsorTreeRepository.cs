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
	public class SponsorTreeRepository : Repository<SponsorTree>,ISponsorTreeRepository
    {
        protected readonly EncoreCommissions_Context _dbCommissions_Context;

        public SponsorTreeRepository(EncoreCommissions_Context dbCommissions_Context) : base(dbCommissions_Context)
        {
            _dbCommissions_Context = dbCommissions_Context;
        }


		public void DevuelveValor(int accountId)
        {
			Func<byte[], byte[], bool> isGreater =
				(xs, ys) =>
				xs
					.Zip(ys, (x, y) => new { x, y })
					.Where(z => z.x != z.y)
					.Take(1)
					.Where(z => z.x > z.y)
					.Any();

			var sponsor = (from stFirst in _dbCommissions_Context.SponsorTree
						  from stSecond in _dbCommissions_Context.SponsorTree
						  where isGreater(stFirst.SortPath,stSecond.SortPath) && stSecond.HLevel <= stFirst.HLevel && stSecond.HGen == stFirst.HGen && stFirst.AccountID == accountId
						  group new { stFirst, stSecond } by new { stSecond.HLevel } into level
						  select new { MaxSortPath = level.Select(x => x.stSecond).Max()}).ToList();


		}

    }
}
