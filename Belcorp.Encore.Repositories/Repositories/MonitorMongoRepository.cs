﻿using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belcorp.Encore.Repositories.Repositories
{
    public class MonitorMongoRepository : Repository<MonitorMongo>, IMonitorMongoRepository
    {
        protected readonly EncoreCore_Context dbCore_Context;

        public MonitorMongoRepository(EncoreCore_Context _dbCore_Context) : base(_dbCore_Context)
        {
            dbCore_Context = _dbCore_Context;
        }

        public IEnumerable<MonitorMongo> GetDataForProcess()
        {
            var result = dbCore_Context.MonitorMongo.Include(c => c.MonitorMongoDetails)
                                              .Where(m => m.Process == false || m.MonitorMongoDetails.Any(md => md.Process == false))
                                              .Take(50)
                                              .ToList();

            return result;
        }
    }
}


