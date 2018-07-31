using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.Mongo.Extension
{
    public class AccountsInformationPerformance_Mongo : AccountsInformation_Mongo
    {
        public Accounts_Mongo Account { get; set; }
    }
}
