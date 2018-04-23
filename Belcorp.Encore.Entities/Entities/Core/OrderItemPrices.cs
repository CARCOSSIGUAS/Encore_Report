using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class OrderItemPrices
    {
        [Key]
        public int OrderItemPriceID { get; set; }

        public int OrderItemID { get; set; }
        public decimal ? OriginalUnitPrice { get; set; }
        public int ProductPriceTypeID { get; set; }
        public decimal UnitPrice { get; set; }
        public string ETLNaturalKey { get; set; }
        public string ETLHash { get; set; }
        public string ETLPhase { get; set; }
        public DateTime ? ETLDate { get; set; }
        public DateTime DateCreatedUTC { get; set; }
        public DateTime ? DateLastModifiedUTC { get; set; }

    }
}