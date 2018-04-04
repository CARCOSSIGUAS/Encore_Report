using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    [Table("Monitor", Schema = "Mongo")]
    public class Monitor
    {
        [Key]
        public int MonitorId { get; set; }

        public int RowId { get; set; }
        public int TableIdPrincipal { get; set; }
        public bool Process { get; set; }
        public DateTime? DateProcess { get; set; }

        public IEnumerable<MonitorDetails> MonitorDetails { get; set; }
    }
}
