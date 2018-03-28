﻿using Belcorp.Encore.Entities.Entities.Commissions;
using System.Collections.Generic;

namespace Belcorp.Encore.Repositories.Interfaces
{
    public interface IProcessOnlineRepository
    {
        List<AccountKPIs> GetListAccounts_Initialize(List<int> accounts, List<CalculationTypes> calculationsTypes, int periodId);
        decimal GetQV_ByAccount_PorcentRuler(int accountId, int periodId, List<CalculationTypes> calculationTypes, decimal porcentForRuler);
        bool GetExists_OrderCalculationsOnline(int orderId);
        decimal GetQV_ByOrder(int orderId);
        decimal GetCV_ByOrder(int orderId);
        decimal GetRV_ByOrder(int orderId);
    }
}
