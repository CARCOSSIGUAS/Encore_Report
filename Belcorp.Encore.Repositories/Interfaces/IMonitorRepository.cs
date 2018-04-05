using Belcorp.Encore.Entities.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Repositories.Interfaces
{
    public interface IMonitorRepository
    {
        IEnumerable<Monitor> GetDataForProcess();
    }
}
