using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class AccountConsistencyStatuses
    {
        [Key]
        public Int16 AccountConsistencyStatusID { get; set; }
        public string Name { get; set; }
        public string TermName { get; set; }
        public int? SortIndex { get; set; }
        public string Description { get; set; }
        public int MarketID { get; set; }
    }
}
