using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class OrderItems
    {
        [Key]
        public int OrderItemID { get; set; }

        public int OrderCustomerID { get; set; }
        public short OrderItemTypeID { get; set; }
        public int ? HostessRewardRuleID { get; set; }
        public int ? ParentOrderItemID { get; set; }
        public int ? ProductID { get; set; }
        public int ? ProductPriceTypeID { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int ? CatalogID { get; set; }
        public int Quantity { get; set; }
        public decimal ? ItemPrice { get; set; }
        public decimal ? ShippingTotal { get; set; }
        public decimal ? ShippingTotalOverride { get; set; }
        public decimal ? HandlingTotal { get; set; }
        public decimal ? Discount { get; set; }
        public decimal ? DiscountPercent { get; set; }
        public decimal ? AdjustedPrice { get; set; }
        public decimal ? CommissionableTotal { get; set; }
        public decimal ? CommissionableTotalOverride { get; set; }
        public bool ChargeTax { get; set; }
        public bool ChargeShipping { get; set; }
        public int ? Points { get; set; }
        public decimal ? MinCustomerSubtotal { get; set; }
        public decimal ? MaxCustomerSubtotal { get; set; }
        public decimal ? TaxPercent { get; set; }
        public decimal ? TaxAmount { get; set; }
        public decimal ? TaxPercentCity { get; set; }
        public decimal ? TaxAmountCity { get; set; }
        public decimal ? TaxAmountCityLocal { get; set; }
        public decimal ? TaxPercentState { get; set; }
        public decimal ? TaxAmountState { get; set; }
        public decimal ? TaxPercentCounty { get; set; }
        public decimal ? TaxAmountCounty { get; set; }
        public decimal ? TaxAmountCountyLocal { get; set; }
        public decimal ? TaxPercentDistrict { get; set; }
        public decimal ? TaxAmountDistrict { get; set; }
        public decimal ? TaxPercentCountry { get; set; }
        public decimal ? TaxAmountCountry { get; set; }
        public decimal ? TaxableTotal { get; set; }
        public byte[] DataVersion { get; set; }
        public int ? ModifiedByUserID { get; set; }
        public int ? DynamicKitGroupID { get; set; }
        public short ? OrderItemParentTypeID { get; set; }
        public decimal? ItemPriceActual { get; set; }
        public string TaxNumber { get; set; }
        public string ETLNaturalKey { get; set; }
        public string ETLHash { get; set; }
        public string ETLPhase { get; set; }
        public DateTime ? ETLDate { get; set; }
        public DateTime DateCreatedUTC { get; set; }
        public DateTime ? DateLastModifiedUTC { get; set; }
        public bool ? Attended { get; set; }
        public int ? MaterialID { get; set; }
        public int ? OfertTypeID { get; set; }
        public decimal ? ParticipationPercentage { get; set; }

    }
}
















































































































