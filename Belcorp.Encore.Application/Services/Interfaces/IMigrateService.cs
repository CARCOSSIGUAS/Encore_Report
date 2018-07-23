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
        void MigrateAccountInformationByPeriod(int? periodId, string country);
        void MigrateAccounts(string country);
        void MigratePeriods(string country);
        void MigrateTermTranslations(string country);
        void MigrateAccountKPIsDetailsByPeriod(int? periodId, string country);
        void MigrateBonusDetailsByPeriod(int? periodId, string country);
        void RequirementTitleCalculations(string country);

        IEnumerable<AccountsInformation_Mongo> GetAccountInformations(List<Titles> titles, IList<AccountsInformation> accountsInformation, Activities activity = null, int? AccountID = null);
    }
}
