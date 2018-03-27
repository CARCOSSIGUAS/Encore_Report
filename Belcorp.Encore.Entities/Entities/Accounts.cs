using Belcorp.Encore.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Belcorp.Encore.Entities
{
    public class Accounts
    {
        [Key]
        public int AccountID { get; set; }


        public string AccountNumber { get; set; }
        public short AccountTypeID { get; set; }
        public short AccountStatusID { get; set; }
        public int ? PreferedContactMethodID { get; set; }
        public int DefaultLanguageID { get; set; }
        public int ? UserID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string CoApplicant { get; set; }
        public string EmailAddress { get; set; }
        public int ? SponsorID { get; set; }
        public int ? EnrollerID { get; set; }
        public DateTime ? EnrollmentDateUTC { get; set; }
        public bool ? IsTaxExempt { get; set; }
        public string TaxNumber { get; set; }
        public bool IsEntity { get; set; }
        public string EntityName { get; set; }
        public short ? AccountStatusChangeReasonID { get; set; }
        public DateTime ? LastRenewalUTC { get; set; }
        public DateTime ? NextRenewalUTC { get; set; }
        public bool ReceivedApplication { get; set; }
        public bool IsTaxExemptVerified { get; set; }
        public DateTime ? DateApplicationReceivedUTC { get; set; }
        public DateTime ? BirthdayUTC { get; set; }
        public short ? GenderID { get; set; }
        public byte[] DataVersion { get; set; }
        public int ? ModifiedByUserID { get; set; }
        public DateTime DateCreatedUTC { get; set; }
        public int ? CreatedByUserID { get; set; }
        public short ? AccountSourceID { get; set; }
        public DateTime ? DateLastModifiedUTC { get; set; }
        public DateTime ? TerminatedDateUTC { get; set; }
        public string TaxGeocode { get; set; }
        public int MarketID { get; set; }
        public string ETLNaturalKey { get; set; }
        public string ETLHash { get; set; }
        public string ETLPhase { get; set; }
        public DateTime ? ETLDate { get; set; }
        public bool IsLocked { get; set; }
        public short ? AccountBlockingTypeID { get; set; }
        public short ? AccountBlockingSubTypeID { get; set; }

        public IEnumerable<AccountPhones> AccountPhones { get; set; }
    }
}
