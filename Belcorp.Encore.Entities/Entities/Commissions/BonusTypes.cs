using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class BonusTypes
    {
        [Key]
        public int BonusTypeID { get; set; }

        public string BonusCode { get; set; }
        public string Name { get; set; }
        public string TermName { get; set; }
        public bool Enabled { get; set; }
        public bool Editable { get; set; }
        public int? PlanID { get; set; }
        public int? EarningsTypeID { get; set; }
        public string ClientName { get; set; }
        public string ClientCode { get; set; }
        public bool? IsCommission { get; set; }
        public int? LevelGenNumber { get; set; }
        public string BonusClass { get; set; }
    }
}


     
     
     
     
     
     
     
     
     
     
     
     
     