using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class MonitorMongo
    {
        [Key]
        public int Id { get; set; }
        public int RowId { get; set; }
        public string TableNamePrincipal { get; set; }
        public bool Process { get; set; }
        public DateTime DateProcess { get; set; }

    }
}
