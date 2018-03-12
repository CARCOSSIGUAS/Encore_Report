using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public interface IAccountInformationService
    {
        IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId);
        IEnumerable<Report_Downline> GetListAccountInformationByPeriodIdAndAccountId(int periodId, int accountId);

        void Migrate_AccountInformationByAccountId(int periodId, int accountId);
        void Migrate_AccountInformationByPeriod(int periodId);
    }
}
