using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Core
{
    public class AccountStatuses
    {
        [Key]
        public short AccountStatusID { get; set; }
        public string Name { get; set; }
        public string TermName { get; set; }
        public string Description { get; set; }
        public bool Editable { get; set; }
        public bool Active { get; set; }
        public bool ReportAsActive { get; set; }
    }
}
