using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class RequirementRules
    {
        public int RuleRequirementID { get; set; }
        public int RuleTypeID { get; set; }
        public int PlanID { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Value3 { get; set; }
        public string Value4 { get; set; }
    }
}
