using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Data.Contexts;
using System.Linq;

namespace Belcorp.Encore.Repositories
{
    public class AccountInformationRepository : Repository<AccountsInformation>, IAccountInformationRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;

        public AccountInformationRepository(EncoreCommissions_Context _dbCommissions_Context) : base(_dbCommissions_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId)
        {
            var result = dbCommissions_Context.AccountsInformation.AsNoTracking().Where(ai => ai.PeriodID == periodId);
            return result;
        }

        public IEnumerable<AccountsInformation_DTO> GetListAccountInformationByPeriodIdAndAccountId(int periodId, List<int> accountIds)
        {
            var result = from ai in dbCommissions_Context.AccountsInformation.AsNoTracking()
                         where ai.PeriodID == periodId && accountIds.Contains(ai.AccountID)
                         select 
                         (  new AccountsInformation_DTO
                            {
                                AccountsInformationID = ai.AccountsInformationID,
                                PeriodID = ai.PeriodID,
                                AccountID = ai.AccountID,
                                AccountNumber = ai.AccountNumber,
                                AccountName = ai.AccountName,
                                SponsorID = ai.SponsorID,
                                SponsorName = ai.SponsorName,
                                Address = ai.Address,
                                PostalCode = ai.PostalCode,
                                City = ai.City,
                                STATE = ai.STATE,

                                JoinDate = ai.JoinDate,
                                Generation = ai.Generation,
                                LEVEL = ai.LEVEL,
                                SortPath = ai.SortPath,
                                LeftBower = ai.LeftBower,
                                RightBower = ai.RightBower
                            }
                        );

            return result.ToList();
        }
    }
}
