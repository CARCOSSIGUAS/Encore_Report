﻿using Belcorp.Encore.Entities.Entities.Commissions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class AccountsInformation_Mongo
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
        public string Region { get; set; }
        public string NewStatus { get; set; }
        public int? TimeLimitToBeDemote { get; set; }
        public string CareerTitle { get; set; }
        public string PaidAsCurrentMonth { get; set; }
        public string PaidAsLastMonth { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? VolumeForCareerTitle { get; set; }
        public string Activity { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? NineMonthsPQV { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? PQV { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? PCV { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? GQV { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? GCV { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? DQVT { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? DCV { get; set; }

        [BsonRepresentation(BsonType.Double)]
        public decimal? DQV { get; set; }

        public DateTime? JoinDate { get; set; }
        public int? Generation { get; set; }
        public int? LEVEL { get; set; }
        public byte[] SortPath { get; set; }
        public int? LeftBower { get; set; }
        public int? RightBower { get; set; }
        public string RequirementNewGeneration { get; set; }
        public int? TimeLimitForNewGeneration { get; set; }
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
        public string EmailAddress { get; set; }
        public decimal? CQL { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public bool? IsCommissionQualified { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime? BirthdayUTC { get; set; }
        public int BirthDayMonth
        {
            get
            {
                return BirthdayUTC.HasValue ? BirthdayUTC.Value.Month : 0;
            }
        }


        public int? UplineLeaderM3 { get; set; }
        public string UplineLeaderM3Name { get; set; }
        public int? UplineLeaderL1 { get; set; }
        public string UplineLeaderL1Name { get; set; }
        public int? TotalDownline { get; set; }
        public decimal? CreditAvailable { get; set; }
        public decimal? DebtsToExpire { get; set; }
        public decimal? ExpiredDebts { get; set; }
        public int? GenerationM3 { get; set; }
        public int? ActiveDownline { get; set; }
        public int? TitleMaintainance { get; set; }
        public decimal? SalesAverage { get; set; }
        public int? NewQualification { get; set; }
        public int? NewEnrollments { get; set; }
        public decimal? NineMonthsGQV { get; set; }
        public decimal? NineMonthsDQV { get; set; }
        public int? ConsultActive { get; set; }

        public string CareerTitle_Des { get; set; }
        public string PaidAsCurrentMonth_Des { get; set; }

        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string LastName1 { get; set; }
        public string LastName2 { get; set; }
        public string country { get; set; }
        public string SPName { get; set; }
        public string SPLastName { get; set; }
        public string HB { get; set; }
    }
}
