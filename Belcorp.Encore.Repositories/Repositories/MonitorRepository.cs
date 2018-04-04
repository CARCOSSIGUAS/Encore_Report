using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belcorp.Encore.Repositories.Repositories
{
    public class MonitorRepository : Repository<Monitor>, IMonitorRepository
    {
        protected readonly EncoreCore_Context dbCore_Context;

        public MonitorRepository(EncoreCore_Context _dbCore_Context) : base(_dbCore_Context)
        {
            dbCore_Context = _dbCore_Context;
        }

        public IEnumerable<Monitor> GetDataForProcess()
        {
            var result = dbCore_Context.Monitor.Include(c => c.MonitorDetails)
                                              .Where(m => m.Process == false || m.MonitorDetails.Any(md => md.Process == false))
                                              .Take(50)
                                              .ToList();

            return result;
        }
    }
}


