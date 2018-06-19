using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class BonusIndicator_DTO
    {
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
        public string INDICATORPAYMENT { get; set; }
        public int? PERIODIDPAYMENT { get; set; }


        public decimal? PayoutAmountLevel { get; set; }
        public decimal? PayoutAmountGeneration { get; set; }
        public decimal? PayoutAmountBonus { get; set; }
    }
}
