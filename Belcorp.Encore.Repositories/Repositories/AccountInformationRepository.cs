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
        protected readonly EncoreCommissions_Context _dbCommissions_Context;

        public AccountInformationRepository(EncoreCommissions_Context dbCommissions_Context) : base(dbCommissions_Context)
        {
            _dbCommissions_Context = dbCommissions_Context;
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId)
        {
            var result = _dbCommissions_Context.AccountsInformation.AsNoTracking().Where(ai => ai.PeriodID == periodId);
            return result;
        }

        public IEnumerable<Report_Downline> GetListAccountInformationByPeriodIdAndAccountId(int periodId, int accountId)
        {
            var accounts = _dbCommissions_Context.AccountsInformation.Where(ai => ai.PeriodID == periodId && ai.AccountID == accountId).FirstOrDefault();
            var accounts_downline = _dbCommissions_Context.AccountsInformation.Where(ai => ai.PeriodID == periodId && ai.LeftBower >= accounts.LeftBower && ai.RightBower <= accounts.RightBower);

            var result = from ai in accounts_downline
                         join a in _dbCommissions_Context.Accounts on ai.AccountID equals a.AccountID
                         where a.FirstName != "TempName"
                         select (new Report_Downline
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
                                 accounts = a
                             }
                        );

            return result.ToList();
        }

        //public IEnumerable<AccountsInformation> GetAccountsInformationKPIs(int periodId, int accountId)
        //{
        //    var accounts = _dbCommissions_Context.AccountsInformation.Where(ai => ai.PeriodID == periodId && ai.AccountID == accountId).FirstOrDefault();
        //    var accounts_downline = _dbCommissions_Context.AccountsInformation.Where(ai => ai.PeriodID == periodId && ai.LeftBower >= accounts.LeftBower && ai.RightBower <= accounts.RightBower);

        //    var result = from ai in _dbCommissions_Context.AccountsInformation
        //                 join a in _dbCommissions_Context.AccountKPIs on ai.AccountID equals a.AccountID
        //                 where a.FirstName != "TempName"
        //                 select (new Report_Downline
        //                 {
        //                     AccountsInformationID = ai.AccountsInformationID,
        //                     PeriodID = ai.PeriodID,
        //                     AccountID = ai.AccountID,
        //                     AccountNumber = ai.AccountNumber,
        //                     AccountName = ai.AccountName,
        //                     SponsorID = ai.SponsorID,
        //                     SponsorName = ai.SponsorName,
        //                     Address = ai.Address,
        //                     PostalCode = ai.PostalCode,
        //                     City = ai.City,
        //                     STATE = ai.STATE,
        //                     accounts = a
        //                 }
        //                );

        //    return result.ToList();
        //}
    }
}
