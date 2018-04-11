using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    [Table("AccountKPIsDetails", Schema = "Reports")]
    public class AccountKPIsDetails
    {
        [Key]
        public int AccountKPIDetailID { get; set; }

        public int PeriodID { get; set; }
        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public int DownlineID { get; set; }
        public string DownlineName { get; set; }
        public string KPICode { get; set; }
        public decimal ? Value { get; set; }
        public decimal ? Percentage { get; set; }
        public string DownlinePaidAsTitle { get; set; }
        public int ? CurrencyTypeID { get; set; }
        public int ? AccountSponsorTypeID { get; set; }
        public int ? TreeLevel { get; set; }
        public DateTime DateModified { get; set; }
    }
}
