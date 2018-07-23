using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Belcorp.Encore.Entities.Entities.Commissions
{
    [Table("AccountsInformation", Schema = "Reports")]
    public class AccountsInformation
    {
        [Key]
        public virtual int AccountsInformationID { get; set; }

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
        public decimal? VolumeForCareerTitle { get; set; }
        public string Activity { get; set; }
        public decimal? NineMonthsPQV { get; set; }
        public decimal? PQV { get; set; }
        public decimal? PCV { get; set; }
        public decimal? GQV { get; set; }
        public decimal? GCV { get; set; }
        public decimal? DQVT { get; set; }
        public decimal? DCV { get; set; }
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
        public DateTime? BirthdayUTC { get; set; }
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

        public decimal? NCWP { get; set; }
        public int? UplineLeader0 { get; set; }
        public bool? IsQualified { get; set; }
        public bool? HasContinuity { get; set; }
    }
}
