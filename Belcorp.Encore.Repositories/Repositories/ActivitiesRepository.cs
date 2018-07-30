using Belcorp.Encore.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Repositories.Interfaces;

namespace Belcorp.Encore.Repositories.Repositories
{
    public class ActivitiesRepository : IActivitiesRepository
    {
        private EncoreCore_Context dbCore_Context;
        private EncoreCommissions_Context dbCommissions_Context;

        public ActivitiesRepository(EncoreCommissions_Context _dbCommissions_Context, EncoreCore_Context _dbCore_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
            dbCore_Context = _dbCore_Context;
        }

        public IEnumerable<Activities> GetAccountActivStatus(int accountID, int periodID)
        {
            var result = from ac in dbCore_Context.Activities join acs in dbCore_Context.ActivityStatuses
                                    on ac.ActivityStatusID equals acs.ActivityStatusID
                                     where ac.AccountID == accountID &&
                                     ac.PeriodID ==
                                    (
                                        (
                                            from per in dbCommissions_Context.Periods
                                            where per.PeriodID < periodID
                                            orderby per.PeriodID descending
                                            select per.PeriodID
                                        ).Take(1).FirstOrDefault()
                                    )
                                    select ac;

            return result;
        }
    }
}
