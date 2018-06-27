using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Belcorp.Encore.Entities.Constants;
using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace Belcorp.Encore.Repositories.Repositories
{
    public class ProcessOnlineRepository : IProcessOnlineRepository
    {
        protected readonly EncoreCommissions_Context dbCommissions_Context;
        protected readonly EncoreCore_Context dbCore_Context;

        public ProcessOnlineRepository(EncoreCommissions_Context _dbCommissions_Context, EncoreCore_Context _dbCore_Context)
        {
            dbCommissions_Context = _dbCommissions_Context;
            dbCore_Context = _dbCore_Context;
        }

        public List<AccountKPIs> GetListAccounts_Initialize(List<int> accounts, List<CalculationTypes> calculationsTypes, int periodId)
        {
            var calculaTypesIds = calculationsTypes.Select(c => c.CalculationTypeID);

            var result = (
                            from a in dbCommissions_Context.Accounts join
                                 ct in dbCommissions_Context.CalculationTypes on 1 equals 1 join
                                 ak in dbCommissions_Context.AccountKPIs on
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

        public bool GetExists_OrderCalculationsOnline(int orderId)
        {
            var result = dbCommissions_Context.OrderCalculationsOnline.Any(oco => oco.OrderID == orderId);    
            return result;
        }


        public decimal GetQV_ByAccount_PorcentRuler(int accountId, int periodId, List<CalculationTypes> calculationTypes, decimal porcentForRuler)
        {
            int calculationType_PQV = calculationTypes.Where(c => c.Code == "PQV").FirstOrDefault().CalculationTypeID;
            int calculationType_DQVT = calculationTypes.Where(c => c.Code == "DQVT").FirstOrDefault().CalculationTypeID;

            var result = dbCommissions_Context.AccountKPIs.Where(a => a.AccountID == accountId && a.PeriodID == periodId && a.CalculationTypeID == calculationType_DQVT).ToList();
            var DQV_Max = result.Select(a => (a.Value * porcentForRuler)/100).FirstOrDefault();
            var DQV_Final = result.Select(a => a.Value).FirstOrDefault();

            var DQV_Excess =
                            (
                                 from a in dbCommissions_Context.AccountKPIs
                                 where
                                 a.AccountID == accountId && a.PeriodID == periodId && a.CalculationTypeID == calculationType_PQV
                                 select new
                                 {
                                     a.AccountID,
                                     a.Value
                                 }
                            ).Concat
                            (
                                 from current_account in dbCommissions_Context.SponsorTree
                                 join sponsored_accounts in dbCommissions_Context.SponsorTree on
                                         current_account.AccountID equals sponsored_accounts.SponsorID
                                 join kpis in dbCommissions_Context.AccountKPIs on
                                         new { A = sponsored_accounts.AccountID, B = periodId, C = calculationType_DQVT } equals new { A = kpis.AccountID, B = kpis.PeriodID, C = kpis.CalculationTypeID }
                                 where
                                 current_account.AccountID == accountId &&
                                 kpis.Value > 0
                                 select new
                                 {
                                     sponsored_accounts.AccountID,
                                     kpis.Value
                                 }
                             )
                             .Where(r => r.Value > DQV_Max)
                             .Select(r => r.Value)
                             .DefaultIfEmpty(0)
                             .Max();

            DQV_Excess = DQV_Excess == 0 ? 0 : DQV_Excess - DQV_Max;

            return DQV_Final - DQV_Excess;
        }

        public decimal GetQV_ByOrder(int orderId)
        {
            var result = (from oip in dbCore_Context.OrderItemPrices join
                              oi  in dbCore_Context.OrderItems on oip.OrderItemID equals oi.OrderItemID join
                              oc  in dbCore_Context.OrderCustomers on oi.OrderCustomerID equals oc.OrderCustomerID
                         where oc.OrderID == orderId && oip.ProductPriceTypeID == (int)Constants.ProductPriceType.QV
                         group new { oi, oip } by oc.OrderID into data
                         select new
                         {
                             QV = data.Sum(d => d.oip.UnitPrice * d.oi.Quantity)
                         }).ToList();

            return result == null || result.Count == 0 ? 0 : (decimal)result.FirstOrDefault().QV;
        }

        public decimal GetRV_ByOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public decimal GetCV_ByOrder(int orderId)
        {
            throw new NotImplementedException();
        }

        public void Execute_Activities(int orderId)
        {
            try
            {
                var pOrderId = new SqlParameter("@OrderID", orderId);
                dbCommissions_Context.Database.ExecuteSqlCommand("Exec SPOnLine_Activities @OrderID", pOrderId);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
