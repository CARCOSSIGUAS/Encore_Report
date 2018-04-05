using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class MonitorOrders
    {
        [Key]
        public int MonnitorOrderId { get; set; }

        public int OrderId { get; set; }
        public bool Process { get; set; }
        public DateTime DateProcess { get; set; }
    }
}
