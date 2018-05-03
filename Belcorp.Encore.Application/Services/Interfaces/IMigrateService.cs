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
    }
}
