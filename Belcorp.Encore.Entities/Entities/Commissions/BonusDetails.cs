using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Commissions
{
    [Table("BonusDetails", Schema = "Reports")]
    public class BonusDetails
    {
        [Key]
        public int BonusDetailID { get; set; }

        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public int DownlineID { get; set; }
        public string DownlineName { get; set; }
        public int BonusTypeID { get; set; }
        public string BonusCode { get; set; }
        public int? OrderID { get; set; }
        public decimal? QV { get; set; }
        public decimal? CV { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? OriginalAmount { get; set; }
        public decimal? Adjustment { get; set; }
        public decimal? PayoutAmount { get; set; }
        public int? CurrencyTypeID { get; set; }
        public int? AccountSponsorTypeID { get; set; }
        public int? TreeLevel { get; set; }
        public int? PeriodID { get; set; }
        public int? ParentOrderID { get; set; }
        public decimal? CorpOriginalAmount { get; set; }
        public decimal? CorpAdjustment { get; set; }
        public decimal? CorpPayoutAmount { get; set; }
        public int? CorpCurrencyTypeID { get; set; }
        public DateTime? DateModified { get; set; }
        public char INDICATORPAYMENT { get; set; }
        public int? PERIODIDPAYMENT { get; set; }
    }
}
