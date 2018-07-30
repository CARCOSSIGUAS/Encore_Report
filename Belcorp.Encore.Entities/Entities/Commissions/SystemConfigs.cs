using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class SystemConfigs
    {
        [Key]
        public int SystemConfigID { get; set; }

        public string ConfigValue { get; set; }
        public string ConfigCode { get; set; }
        public string ConfigDescription { get; set; }
        public bool Editable { get; set; }
        public DateTime DateModified { get; set; }
    }
}
