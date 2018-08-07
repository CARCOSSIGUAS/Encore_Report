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
        void RequirementLegs(string country);
        void Performance(string country, int? periodId);
        void PerformanceByPeriod(int? periodId, string country);

        IEnumerable<AccountsInformation_Mongo> GetAccountInformations(List<Titles> titles, IList<AccountsInformation> accountsInformation, Activities activityPrevious = null, Activities activityCurrent = null, int? AccountID = null);
    }
}
