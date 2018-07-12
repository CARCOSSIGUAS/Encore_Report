using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    public class AccountTitles
    {
        [Key, Column(Order = 0)]
        public int AccountID { get; set; }

        [Key, Column(Order = 1)]
        public int TitleID { get; set; }

        [Key, Column(Order = 2)]
        public int TitleTypeID { get; set; }

        [Key, Column(Order = 3)]
        public int PeriodID { get; set; }

        public DateTime DateModified { get; set; }
    }
}
