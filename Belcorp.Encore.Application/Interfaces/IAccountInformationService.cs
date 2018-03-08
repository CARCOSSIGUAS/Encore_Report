using Belcorp.Encore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application
{
    public interface IAccountInformationService
    {
        IEnumerable<AccountsInformation> GetListAccountInformationByPeriodIdAndAccountId(int periodId, int accountId);
        IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId);
       
        void Migrate_AccountInformationByAccountId(int periodId, int accountId);
        void Migrate_AccountInformationByPeriod(int periodId);
    }
}
