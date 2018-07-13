using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class ReportAccount_DTO
    {
        public int AccountID { get; set; }

        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string JoinDate { get; set; }

        public string EmailAddress { get; set; }

        public int? Generation { get; set; }
        public int? LEVEL { get; set; }
        public string Activity { get; set; }

        public decimal? PQV { get; set; }
        public decimal? PCV { get; set; }
        public decimal? DQVT { get; set; }
        public decimal? DQV { get; set; }

        public string CareerTitle { get; set; }

        public string PaidAsCurrentMonth { get; set; }

        public string MainAddress { get; set; }
        public string Phones { get; set; }

        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public string SponsorEmailAddress { get; set; }
        public string SponsorPhones { get; set; }
        public int? ActiveDownline { get; set; }
        public int? ConsultActive { get; set; }
        public string Birthday { get; set; }

        public string PostalCode { get; set; }
    }
}
