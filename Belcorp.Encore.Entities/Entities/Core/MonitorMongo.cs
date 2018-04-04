using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    [Table("MonitorMongo", Schema = "Mongo")]
    public class MonitorMongo
    {
        [Key]
        public int MonitorMongoId { get; set; }

        public int RowId { get; set; }
        public string TableNamePrincipal { get; set; }
        public bool Process { get; set; }
        public DateTime? DateProcess { get; set; }

        public IEnumerable<MonitorMongoDetails> MonitorMongoDetails { get; set; }
    }
}
