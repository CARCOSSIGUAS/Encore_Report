using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    [Table("MonitorLotes", Schema = "Mongo")]
    public class MonitorLotes
    {
        [Key]
        public int LoteId { get; set; }

        public DateTime ? DateProcess { get; set; }
        public bool Process { get; set; }

        public IEnumerable<MonitorOrders> MonitorOrders { get; set; }

    }
}
