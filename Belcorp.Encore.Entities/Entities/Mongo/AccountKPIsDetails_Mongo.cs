using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class AccountKPIsDetails_Mongo
    {
        [BsonId]
        public int AccountKPIDetailID { get; set; }
        public int PeriodID { get; set; }
        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public int DownlineID { get; set; }
        public string DownlineName { get; set; }
        public string KPICode { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? Value { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? Percentage { get; set; }

        public string DownlinePaidAsTitle { get; set; }
        public int? CurrencyTypeID { get; set; }
        public int? AccountSponsorTypeID { get; set; }
        public int? TreeLevel { get; set; }
        public DateTime DateModified { get; set; }
    }
}
