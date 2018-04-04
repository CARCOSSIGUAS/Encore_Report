using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    [Table("MonitorMongoDetails", Schema = "Mongo")]
    public class MonitorMongoDetails
    {
        [Key]
        public int MonitorMongoDetailsId { get; set; }

        public int MonitorMongoId { get; set; }
        public int RowDetalleId { get; set; }
        public string TableNameSecundary { get; set; }
        public bool Process { get; set; }
        public DateTime? DateProcess { get; set; }
    }
}
