using Belcorp.Encore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application
{
    public interface IAccountInformationService
    {
        IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId);
        IEnumerable<AccountsInformation> GetListAccountInformationByAccountId(int accountId);
        void CalcularAccountInformation(int periodId, int accountId);
        void Migrate_AccountInformationByAccountId(int accountId);
    }
}
