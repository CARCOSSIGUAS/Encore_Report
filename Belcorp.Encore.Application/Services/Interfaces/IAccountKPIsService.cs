using Belcorp.Encore.Entities.Entities.Commissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IAccountKPIsService
    {
        AccountKPIs GetAmountAccountKPI(int accountID, int periodID, int calculationType);
    }
}
