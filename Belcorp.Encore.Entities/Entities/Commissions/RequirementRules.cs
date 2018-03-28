using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class RequirementRules
    {
        [Key, Column(Order = 0)]
        public int RuleRequirementID { get; set; }

        [Key, Column(Order = 0)]
        public int RuleTypeID { get; set; }

        public int PlanID { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
    }
}
