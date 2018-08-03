using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class RequirementLegs
    {
        [Key, Column(Order = 0)]
        public int TitleID { get; set; }
        [Key, Column(Order = 1)]
        public int PlanID { get; set; }
        [ForeignKey("Titles"), Column(Order = 2)]
        public int TitleRequired { get; set; }
        public int Generation { get; set; }
        public int Level { get; set; }
        public decimal TitleQty { get; set; }

        public Titles Titles { get; set; }
    }
}
