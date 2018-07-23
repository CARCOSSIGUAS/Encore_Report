using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo.Extension
{
    public class AccountsInformation_MongoWithAccountAndSponsor : AccountsInformation_Mongo
    {
        public Accounts_Mongo Account { get; set; }
        public Accounts_Mongo Sponsor { get; set; }
        public Accounts_Mongo Leader0 { get; set; }
        public Accounts_Mongo LeaderM3 { get; set; }
    }
}
