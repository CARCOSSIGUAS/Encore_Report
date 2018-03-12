using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class Report_Downline
    {
        [BsonId]
        public int AccountsInformationID { get; set; }

        public int PeriodID { get; set; }
        public int AccountID { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string STATE { get; set; }

        public DateTime? JoinDate { get; set; }
        public int? Generation { get; set; }
        public int? LEVEL { get; set; }
        public byte[] SortPath { get; set; }
        public int? LeftBower { get; set; }
        public int? RightBower { get; set; }

        [BsonIgnore]
        public Accounts accounts { get; set; }
    }
}
