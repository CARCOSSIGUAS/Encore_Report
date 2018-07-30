﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Constants
{
    public class Constants
    {
        public enum OrderType : short
        {
            NotSet = 0,
            OnlineOrder = 1,
            WorkstationOrder = 2,
            PartyOrder = 3,
            PortalOrder = 4,
            AutoshipTemplate = 5,
            AutoshipOrder = 6,
            OverrideOrder = 7,
            ReturnOrder = 8,
            CompOrder = 9,
            ReplacementOrder = 10,
            EnrollmentOrder = 11,
            EmployeeOrder = 12,
            BusinessMaterialsOrder = 13,
            HostessRewardsOrder = 14,
            FundraiserOrder = 15,
            OnlinePartyOrder = 16,
            Employee = 17,
        }

        public enum OrderStatus : short
        {
            NotSet = 0,
            Pending = 1,
            PendingError = 2,
            Paid = 4,
            Cancelled = 5,
            PartiallyPaid = 6,
            Printed = 8,
            Shipped = 9,
            CreditCardDeclined = 11,
            CreditCardDeclinedRetry = 12,
            PartiallyShipped = 13,
            CancelledPaid = 14,
            DeferredOnlinePayment = 15,
            Suspended = 16,
            PartyOrderPending = 17,
            PendingPaid = 18,
            PendingConfirm = 19,
            Invoiced = 20,
            Delivered = 21,
            Embarked = 22,
            BillingProcessing = 23
        }

        public enum ProductPriceType : int
        {
            NotSet = 0,
            Retail = 1,
            PreferredCustomer = 2,
            ShippingFee = 10,
            HandlingFee = 11,
            CV = 18,
            HostBase = 20,
            QV = 21,
            Wholesale = 22,
        }

        public enum MonitorTables : int
        {
            NotSet = 0,
            Accounts = 1,
            AccountsPhones = 2,
            Periods = 3,
            TermTranslations = 4
        }

        public enum ResourceUriType
        {
            PreviousPage,
            NextPage
        }

        public enum TitleTypes : int
        {
            NotSet = 0,
            Career = 1, 
            Paid = 2
        }

        public enum AccountStatus : short
        {
            NotSet = 0,
            Active = 1,
            Terminated = 2,
            BegunEnrollment = 3,
            Imported = 4,
        }

        public enum Countries : int
        {
            NotSet = 0,
            USA = 1,
            BRA = 73
        }

        public enum AccountConsistencyStatus : int
        {
            BegunEnrollment = 1,
            New1 = 2,
            New2= 3,
            New3= 4,
            New4 = 5,
            Established = 6,
            New0 = 7,
            Terminated = 9
        }
    }
}
