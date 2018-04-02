using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.MetaData_Mongo
{
    public class AccountInformationByAccounts_DTO : AccountsInformation_DTO
    {
		public Accounts_DTO[] Accounts_DTOs { get; set; }
	}
}
