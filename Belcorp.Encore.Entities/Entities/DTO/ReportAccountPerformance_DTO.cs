using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class ReportAccountPerformance_DTO
    {
        public int AccountID { get; set; }

        public string AccountNumber { get; set; }
        public string AccountName { get; set; }

        public string CareerTitle { get; set; }
        public string CareerTitleDes { get; set; }
        public string PaidAsCurrentMonth { get; set; }
        public string PaidAsCurrentMonthDesc { get; set; }

        public int? LEVEL { get; set; }

        public decimal? PQV { get; set; }
        public decimal? PCV { get; set; }
        public decimal? DQVT { get; set; }
        public decimal? DQV { get; set; }
        public decimal? CQL { get; set; }

        public int? Title1Legs { get; set; }
        public int? Title2Legs { get; set; }
        public int? Title3Legs { get; set; }
        public int? Title4Legs { get; set; }
        public int? Title5Legs { get; set; }
        public int? Title6Legs { get; set; }
        public int? Title7Legs { get; set; }
        public int? Title8Legs { get; set; }
        public int? Title9Legs { get; set; }
        public int? Title10Legs { get; set; }
        public int? Title11Legs { get; set; }
        public int? Title12Legs { get; set; }
        public int? Title13Legs { get; set; }
        public int? Title14Legs { get; set; }


        public decimal? PQVRequirement { get; set; }
        public decimal? PCVRequirement { get; set; }
        public decimal? DQVTRequirement { get; set; }
        public decimal? DQVRequirement { get; set; }
        public decimal? CQLRequirement { get; set; }

        public decimal? PQVRequirementNext { get; set; }
        public decimal? PCVRequirementNext { get; set; }
        public decimal? DQVTRequirementNext { get; set; }
        public decimal? DQVRequirementNext { get; set; }
        public decimal? CQLRequirementNext { get; set; }
    }
}
