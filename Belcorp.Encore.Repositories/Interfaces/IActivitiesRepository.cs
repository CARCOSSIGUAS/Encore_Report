using Belcorp.Encore.Entities.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Repositories.Interfaces
{
    public interface IActivitiesRepository
    {
        IEnumerable<Activities> GetAccountActivStatus(int accountID, int periodID);
    }
}
