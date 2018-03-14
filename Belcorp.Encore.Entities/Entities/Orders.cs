using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }

        public string OrderNumber { get; set; }
        public Int16 OrderStatusID { get; set; }
        public Int16 OrderTypeID { get; set; }
        public int AccountID { get; set; }
        public int? SiteID { get; set; }
        public int? ParentOrderID { get; set; }
        public int CurrencyID { get; set; }
        public DateTime? CompleteDateUTC { get; set; }
        public DateTime? CommissionDateUTC { get; set; }
        public decimal? HostessRewardsEarned { get; set; }
        public decimal? HostessRewardsUsed { get; set; }
        public bool? IsTaxExempt { get; set; }
        public decimal? TaxAmountTotal { get; set; }
        public decimal? TaxAmountTotalOverride { get; set; }
        public decimal? TaxableTotal { get; set; }
        public decimal? TaxAmountOrderItems { get; set; }
        public decimal? TaxAmountShipping { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? DiscountTotal { get; set; }
        public decimal? ShippingTotal { get; set; }
        public decimal? ShippingTotalOverride { get; set; }
        public decimal? HandlingTotal { get; set; }
        public decimal? GrandTotal { get; set; }
        public decimal? PaymentTotal { get; set; }
        public decimal? Balance { get; set; }
        public decimal? CommissionableTotal { get; set; }
        public int? ReturnTypeID { get; set; }
        public string StepUrl { get; set; }
        public int? ModifiedByUserID { get; set; }
        public DateTime DateCreatedUTC { get; set; }
        public int? CreatedByUserID { get; set; }
        public byte[] DataVersion { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? PartyShipmentTotal { get; set; }
        public decimal? PartyHandlingTotal { get; set; }
        public string ETLNaturalKey { get; set; }
        public string ETLHash { get; set; }
        public string ETLPhase { get; set; }
        public DateTime? ETLDate { get; set; }
        public DateTime? DateLastModifiedUTC { get; set; }
        public string IDNationalMail { get; set; }
        public int? IDSupportTicket { get; set; }
        public int? CreatedPeriodID { get; set; }
        public int? CompletedPeriodID { get; set; }
    }
}

