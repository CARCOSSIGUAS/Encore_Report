using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class RequirementTitleCalculations_Mongo
    {
        public int TitleID { get; set; }
        public int CalculationtypeID { get; set; }
        public int PlanID { get; set; }

        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public DateTime DateModified { get; set; }
    }
}
