using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class OrderCustomers
    {
        [Key]
        public int OrderCustomerID { get; set; }

        public short OrderCustomerTypeID { get; set; }
        public int OrderID { get; set; }
        public int AccountID { get; set; }
        public decimal ? ShippingTotal { get; set; }
        public decimal ? HandlingTotal { get; set; }
        public decimal ? DiscountAmount { get; set; }
        public decimal ? Subtotal { get; set; }
        public decimal ? PaymentTotal { get; set; }
        public decimal ? CommissionableTotal { get; set; }
        public decimal ? Balance { get; set; }
        public decimal ? Total { get; set; }
        public DateTime ? FutureBookingDateUTC { get; set; }
        public bool ? IsTaxExempt { get; set; }
        public decimal? TaxAmountTotal { get; set; }
        public decimal? TaxAmountCity { get; set; }
        public decimal? TaxAmountState { get; set; }
        public decimal? TaxAmountCounty { get; set; }
        public decimal? TaxAmountDistrict { get; set; }
        public decimal? TaxAmountOrderItems { get; set; }
        public decimal? TaxAmountShipping { get; set; }
        public decimal? TaxableTotal { get; set; }
        public decimal? TaxAmount { get; set; }
        public byte[] DataVersion { get; set; }
        public int? ModifiedByUserID { get; set; }
        public decimal? TaxAmountCountry { get; set; }
        public bool IsBookingCredit { get; set; }
        public string TaxGeocode { get; set; }
        public string SalesTaxTransactionNumber { get; set; }
        public string UseTaxTransactionNumber { get; set; }
        public string ETLNaturalKey { get; set; }
        public string ETLHash { get; set; }
        public string ETLPhase { get; set; }
        public DateTime ? ETLDate { get; set; }
        public DateTime DateCreatedUTC { get; set; }
        public DateTime DateLastModifiedUTC { get; set; }
        public int WarehouseID { get; set; }

    }
}
