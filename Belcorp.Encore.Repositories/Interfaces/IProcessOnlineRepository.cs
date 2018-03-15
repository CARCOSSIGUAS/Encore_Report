using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Repositories.Interfaces
{
    public interface IProcessOnlineRepository
    {
        List<AccountKPIs> GetListAccounts_InitializesKPIsInGroup(List<int> accounts, List<int> calculationsTypes, int periodId);
    }
}
