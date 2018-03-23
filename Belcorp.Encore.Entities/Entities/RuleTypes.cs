using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class RuleTypes
    {
        [Key]
        public int RuleTypeID { get; set; }

        public string Name { get; set; }
        public string TermName { get; set; }
        public string Description { get; set; }
        public string ValueType1 { get; set; }
        public string ValueType2 { get; set; }
        public string ValueType3 { get; set; }
        public string ValueType4 { get; set; }
        public string ValueTypeTerm1 { get; set; }
        public string ValueTypeTerm2 { get; set; }
        public string ValueTypeTerm3 { get; set; }
        public string ValueTypeTerm4 { get; set; }
        public bool Active { get; set; }
        public DateTime DateModified { get; set; }
        public int SortOrder { get; set; }

        public RequirementRules RequirementRules { get; set; }

    }
}
