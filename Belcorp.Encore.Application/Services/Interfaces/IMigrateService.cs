using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IMigrateService
    {
        void MigrateAccountInformationByPeriod(int? periodId);
        void MigrateAccounts();
        void MigratePeriods();

        IEnumerable<AccountsInformation_Mongo> GetAccountInformations(List<Titles> titles, IList<AccountsInformation> accountsInformation, Activities activity = null, int? AccountID = null);
    }
}
