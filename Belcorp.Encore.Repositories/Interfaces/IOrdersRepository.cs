using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Repositories.Interfaces
{
    public interface IOrdersRepository
    {
        int GetExistOrderPerAnt(int accountID, int periodID);
        int GetExistOrderPerAct(int accountID, int periodID);
    }
}
