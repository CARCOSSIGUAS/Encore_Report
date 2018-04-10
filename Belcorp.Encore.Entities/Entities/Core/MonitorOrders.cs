using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    [Table("MonitorOrders", Schema = "Mongo")]
    public class MonitorOrders
    {
        [Key]
        public int MonitorOrderId { get; set; }

        public int LoteId { get; set; }
        public int OrderId { get; set; }
        public bool Process { get; set; }
        public DateTime ? DateProcess { get; set; }
    }
}
