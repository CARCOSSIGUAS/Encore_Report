using Belcorp.Encore.Entities.Entities.Commissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface ISystemConfigsService
    {
        SystemConfigs GetAmountParameter(string configCode);
    }
}
