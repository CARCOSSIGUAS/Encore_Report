using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class ReportAccount_DTO
    {
        public AccountsInformation_Mongo accountsInformation { get; set; }
        public Accounts_Mongo account { get; set; }
        public Accounts_Mongo accountSponsor { get; set; }
    }
}
