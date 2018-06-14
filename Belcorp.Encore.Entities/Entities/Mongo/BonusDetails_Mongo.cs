using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class BonusDetails_Mongo
    {
        [BsonId]
        public int BonusDetailID { get; set; }

        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public int DownlineID { get; set; }
        public string DownlineName { get; set; }
        public int BonusTypeID { get; set; }
        public string BonusCode { get; set; }
        public int? OrderID { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? QV { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? CV { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? Percentage { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? OriginalAmount { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? Adjustment { get; set; }

        [BsonRepresentation(BsonType.Double)]
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
    }
}
