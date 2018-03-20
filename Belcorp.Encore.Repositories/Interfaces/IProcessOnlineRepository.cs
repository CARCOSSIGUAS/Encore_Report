using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Repositories.Interfaces
{
    public interface IProcessOnlineRepository
    {
        List<AccountKPIs> GetListAccounts_Initialize(List<int> accounts, List<CalculationTypes> calculationsTypes, int periodId);

    }
}
