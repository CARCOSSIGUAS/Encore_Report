using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    [Table("MonitorDetails", Schema = "Mongo")]
    public class MonitorDetails
    {
        [Key]
        public int MonitorDetailsId { get; set; }

        public int MonitorId { get; set; }
        public int RowDetalleId { get; set; }
        public int TableIdSecundary { get; set; }
        public bool Process { get; set; }
        public DateTime? DateProcess { get; set; }
    }
}
