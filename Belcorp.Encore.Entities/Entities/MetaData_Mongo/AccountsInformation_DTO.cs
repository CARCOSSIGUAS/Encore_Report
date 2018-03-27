using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities
{
    public class AccountsInformation_DTO : AccountsInformation
    {
        [BsonId]
        public override int AccountsInformationID { get; set; }

        public string CareerTitle_Des { get; set; }
        public string PaidAsCurrentMonth_Des { get; set; }

    }
}
