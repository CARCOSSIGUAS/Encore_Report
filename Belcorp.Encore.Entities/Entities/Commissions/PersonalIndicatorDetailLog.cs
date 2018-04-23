using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class PersonalIndicatorDetailLog
    {
        [Key]
        public int PersonalIndicatorDetailLogID { get; set; }

        public int? PersonalIndicatorLogID { get; set; }
        public string CodeSubProcess { get; set; }
        public string TermName { get; set; }
        public DateTime? StarTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? StatusProcessMonthlyClosureID { get; set; }
        public string MessageToShow { get; set; }
        public string RealError { get; set; }
    }
}
