using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class ActivityStatuses
    {
        [Key]
        public short ActivityStatusID { get; set; }

        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string TermName { get; set; }
        public string CampaignsWithoutOrder { get; set; }
        public short AccountStatusID { get; set; }
        public string ActivityDescripcion { get; set; }
        public int? SortIndex { get; set; }
        public int MarketID { get; set; }
        public bool IsActive { get; set; }
    }
}
