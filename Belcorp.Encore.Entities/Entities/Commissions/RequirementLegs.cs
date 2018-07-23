using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class RequirementLegs
    {
        public int TitleID { get; set; }
        public int PlanID { get; set; }
        public int TitleRequired { get; set; }
        public int Generation { get; set; }
        public int Level { get; set; }
        public decimal TitleQty { get; set; }
    }
}
