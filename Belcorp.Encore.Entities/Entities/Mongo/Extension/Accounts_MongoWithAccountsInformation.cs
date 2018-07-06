using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo.Extension
{
    public class Accounts_MongoWithAccountsInformation : Accounts_Mongo
    {
        public AccountsInformation_Mongo AccountInformation { get; set; }
    }
}
