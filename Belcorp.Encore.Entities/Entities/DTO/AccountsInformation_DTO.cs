using Belcorp.Encore.Entities.Entities.Commissions;
using MongoDB.Bson.Serialization.Attributes;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class AccountsInformation_DTO : AccountsInformation
    {
        [BsonId]
        public override int AccountsInformationID { get; set; }

        public string CareerTitle_Des { get; set; }
        public string PaidAsCurrentMonth_Des { get; set; }

    }
}
