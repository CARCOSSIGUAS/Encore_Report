using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class OrderCalculationsOnline
    {
        [Key, Column(Order = 0)]
        public int AccountID { get; set; }

        [Key, Column(Order = 1)]
        public int OrderID { get; set; }

        [Key, Column(Order = 2)]
        public int OrderCalculationTypeID { get; set; }

        public int OrderStatusID { get; set; }
        public decimal ? Value { get; set; }
        public DateTime ? CalculationDateUTC { get; set; }
        public int ? ParentOrderID { get; set; }
        public int ? AccountTypeID { get; set; }
        public int ? OrderTypeID { get; set; }
        public DateTime ? DateModifiedUTC { get; set; }
    }
}
