using Belcorp.Encore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application
{
    public interface IAccountInformationService
    {
        IPagedList<AccountsInformation> GetListAccountInformationByPeriodId(int periodId);
        void CalcularAccountInformation(int periodId, int accountId);
    }
}
