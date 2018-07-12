using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class RequirementTitleCalculations
    {
        [Key, Column(Order = 0)]
        public int TitleID { get; set; }

        [Key, Column(Order = 1)]
        public int CalculationtypeID { get; set; }

        [Key, Column(Order = 2)]
        public int PlanID { get; set; }

        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public DateTime DateModified { get; set; }

        public CalculationTypes calculationTypes { get; set; }
    }
}
