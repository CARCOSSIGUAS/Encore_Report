using Belcorp.Encore.Entities.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IActivityStatusesService
    {
        ActivityStatuses GetActivityIDFromInternalName(string internalName);
    }
}
