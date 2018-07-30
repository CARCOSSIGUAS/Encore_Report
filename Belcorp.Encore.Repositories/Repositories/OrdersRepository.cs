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
    public class OrdersRepository: IOrdersRepository
    {
        protected readonly EncoreCore_Context dbCore_Context;
        List<int> orderTypesIds = new List<int>();
        List<int> orderStatusIds = new List<int>();
        

        public OrdersRepository(EncoreCore_Context _dbCore_Context)
        {
            dbCore_Context = _dbCore_Context;
        }

        public int GetExistOrderPerAnt(int accountID, int periodID)
        {
            FiltrosOrdersTypesOrdersStatus();

            var query = (from ord in dbCore_Context.Orders join acc in dbCore_Context.Accounts on ord.AccountID equals acc.AccountID
                          where ord.AccountID == accountID && ord.DateCreatedUTC >= acc.EnrollmentDateUTC &&
                          ord.CompletedPeriodID <= periodID && orderTypesIds.Contains(ord.OrderTypeID) &&
                          orderStatusIds.Contains(ord.OrderStatusID) &&
                          ord.GrandTotal + (
                         (from o_Aux in dbCore_Context.Orders
                          where
                               o_Aux.OrderTypeID == (int)Constants.OrderType.ReturnOrder &&
                               o_Aux.AccountID == ord.AccountID &&
                               o_Aux.ParentOrderID == ord.OrderID
                          select o_Aux.GrandTotal ?? 0

                        ).Sum()) != 0
                          select ord.OrderID).Count();


            int result = (query > 0) ? 1 : 0;

            return result;
        }


        public int GetExistOrderPerAct(int accountID, int periodID)
        {
            FiltrosOrdersTypesOrdersStatus();

            var query = (from ord in dbCore_Context.Orders
                         join acc in dbCore_Context.Accounts on ord.AccountID equals acc.AccountID
                         where ord.AccountID == accountID && ord.CompletedPeriodID == periodID &&
                         orderTypesIds.Contains(ord.OrderTypeID) && orderStatusIds.Contains(ord.OrderStatusID) &&
                         ord.GrandTotal + (
                        (from o_Aux in dbCore_Context.Orders
                         where
                              o_Aux.OrderTypeID == (int)Constants.OrderType.ReturnOrder &&
                              o_Aux.AccountID == ord.AccountID &&
                              o_Aux.ParentOrderID == ord.OrderID
                         select o_Aux.GrandTotal ?? 0

                       ).Sum()) != 0
                         select ord.OrderID).Count();


            int result = (query > 0) ? 1 : 0;

            return result;
        }

        public void FiltrosOrdersTypesOrdersStatus()
        {
            orderTypesIds.Add(Convert.ToInt32(Constants.OrderType.OnlineOrder));
            orderTypesIds.Add(Convert.ToInt32(Constants.OrderType.WorkstationOrder));
            orderTypesIds.Add(Convert.ToInt32(Constants.OrderType.PortalOrder));
            orderTypesIds.Add(Convert.ToInt32(Constants.OrderType.EnrollmentOrder));

            orderStatusIds.Add(Convert.ToInt32(Constants.OrderStatus.Paid));
            orderStatusIds.Add(Convert.ToInt32(Constants.OrderStatus.Printed));
            orderStatusIds.Add(Convert.ToInt32(Constants.OrderStatus.Shipped));
            orderStatusIds.Add(Convert.ToInt32(Constants.OrderStatus.CancelledPaid));
            orderStatusIds.Add(Convert.ToInt32(Constants.OrderStatus.Invoiced));
            orderStatusIds.Add(Convert.ToInt32(Constants.OrderStatus.Delivered));
            orderStatusIds.Add(Convert.ToInt32(Constants.OrderStatus.Embarked));
        }
    }
}
