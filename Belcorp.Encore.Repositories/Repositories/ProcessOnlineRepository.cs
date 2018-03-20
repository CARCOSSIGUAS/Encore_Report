using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Belcorp.Encore.Repositories.Repositories
{
    public class ProcessOnlineRepository : IProcessOnlineRepository
    {
        protected readonly EncoreCommissions_Context _dbCommissions_Context;

        public ProcessOnlineRepository(EncoreCommissions_Context dbCommissions_Context)
        {
            _dbCommissions_Context = dbCommissions_Context;
        }

        public List<AccountKPIs> GetListAccounts_Initialize(List<int> accounts, List<CalculationTypes> calculationsTypes, int periodId)
        {
            var calculaTypesIds = calculationsTypes.Select(c => c.CalculationTypeID);

            var result = (
                            from a in _dbCommissions_Context.Accounts join
                                 ct in _dbCommissions_Context.CalculationTypes on 1 equals 1 join
                                 ak in _dbCommissions_Context.AccountKPIs on
                                    new { A = a.AccountID, B = periodId, C = ct.CalculationTypeID } equals new { A = ak.AccountID, B = ak.PeriodID, C = ak.CalculationTypeID }
                                 into ps from p in ps.DefaultIfEmpty()
                            where accounts.Contains(a.AccountID) && calculaTypesIds.Contains(ct.CalculationTypeID) && p == null
                            select new AccountKPIs {
                                AccountID = a.AccountID,
                                CalculationTypeID = ct.CalculationTypeID,
                                PeriodID = periodId,
                                Value = 0,
                                DateModified = DateTime.Now

                            }
                          ).ToList();

            return result;

        }


        public decimal GetDQV_Online(int accountId, int periodId, List<CalculationTypes> calculationTypes, decimal porcent)
        {
            int calculationType_PQV = calculationTypes.Where(c => c.Code == "PQV").FirstOrDefault().CalculationTypeID;
            int calculationType_DQVT = calculationTypes.Where(c => c.Code == "DQVT").FirstOrDefault().CalculationTypeID;

            var result = _dbCommissions_Context.AccountKPIs.Where(a => a.AccountID == a.AccountID && a.PeriodID == periodId && a.CalculationTypeID == calculationType_DQVT).ToList();
            var DQV_Max = result.Select(a => (a.Value * porcent)/100).FirstOrDefault();
            var DQV_Final = result.Select(a => a.Value).FirstOrDefault();

            var DQV_Excess =
                            (
                                 from a in _dbCommissions_Context.AccountKPIs
                                 where
                                 a.AccountID == accountId && a.PeriodID == periodId && a.CalculationTypeID == calculationType_PQV
                                 select new
                                 {
                                     a.AccountID,
                                     a.Value
                                 }
                            ).Concat
                            (
                                 from current_account in _dbCommissions_Context.SponsorTree
                                 join sponsored_accounts in _dbCommissions_Context.SponsorTree on
                                         current_account.AccountID equals sponsored_accounts.SponsorID
                                 join kpis in _dbCommissions_Context.AccountKPIs on
                                         sponsored_accounts.AccountID equals kpis.AccountID
                                 where
                                 current_account.AccountID == accountId &&
                                 kpis.PeriodID == periodId &&
                                 kpis.CalculationTypeID == calculationType_DQVT &&
                                 kpis.Value > 0
                                 select new
                                 {
                                     sponsored_accounts.AccountID,
                                     kpis.Value
                                 }
                             ).Where(r => r.Value > DQV_Max)
                              .Max(r => r.Value - DQV_Max);

            return DQV_Final - DQV_Excess;
        }

    }
}
