using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class PersonalIndicatorLog
    {
        [Key]
        public int PersonalIndicatorLogID { get; set; }

        public int ? OrderID { get; set; }
        public int ? OrderStatusID { get; set; }
        public string TermName { get; set; }
        public DateTime ? StarTime { get; set; }
        public DateTime ? EndTime { get; set; }
        public int ? StatusProcessMonthlyClosureID { get; set; }

        public List<PersonalIndicatorDetailLog> PersonalIndicatorDetailLog { get; set; } = new List<PersonalIndicatorDetailLog>();
    }
}
