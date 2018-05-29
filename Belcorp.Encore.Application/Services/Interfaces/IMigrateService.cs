using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IMigrateService
    {
        void MigrateAccountInformationByPeriod(string country, int? periodId);
        void MigrateAccounts(string country);
        void MigratePeriods(string country);
    }
}
