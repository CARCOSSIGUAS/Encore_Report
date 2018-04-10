using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.MetaData_Mongo
{
    public class AccountInformationByAccounts_DTO : AccountsInformation_Mongo
    {
		public Accounts_Mongo[] Accounts_DTOs { get; set; }
	}
}
