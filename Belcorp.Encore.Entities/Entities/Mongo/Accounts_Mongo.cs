using Belcorp.Encore.Entities.Entities.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo
{
    public class Accounts_Mongo
    {
        [BsonId]
        public int AccountID { get; set; }

        public int CountryID { get; set; }
        public string AccountNumber { get; set; }
        public int AccountTypeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public int? SponsorID { get; set; }
        public int? EnrollerID { get; set; }
        public DateTime? EnrollmentDateUTC { get; set; }
        public bool IsEntity { get; set; }
        public int? AccountStatusChangeReasonID { get; set; }
        public int? AccountStatusID { get; set; }
        public string EntityName { get; set; }
        public DateTime? BirthdayUTC { get; set; }
        public DateTime? TerminatedDateUTC { get; set; }
        public DateTime? ActivationDateUTC { get; set; }
        public int? ActivationPeriodID { get; set; }

        public List<AccountPhones> AccountPhones = new List<AccountPhones>();
        
    }
}
