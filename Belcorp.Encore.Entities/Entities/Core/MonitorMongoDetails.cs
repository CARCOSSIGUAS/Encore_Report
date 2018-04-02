using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class MonitorMongoDetails
    {
        [Key]
        public int Id { get; set; }
        public int RowId { get; set; }
        public int RowDetalleId { get; set; }
        public string TableNameSecundary { get; set; }
        public bool Process { get; set; }
        public DateTime DateProcess { get; set; }

        public virtual MonitorMongo MonitorMongo { get; set; }
        
    }
}
