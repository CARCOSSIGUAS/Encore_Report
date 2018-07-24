using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class RequirementLegs_DTO
    {
        public int TitleID { get; set; }
        public int PlanID { get; set; }
        public int TitleRequired { get; set; }
        public string TitleDescription { get; set; }
        public int Generation { get; set; }
        public int Level { get; set; }
        public decimal TitleQty { get; set; }
        public decimal TitleQtyDiff { get; set; }
    }
}
