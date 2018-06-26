using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IPersonalIndicatorLogService
    {
        PersonalIndicatorLog GetByOrderId(int orderID);
        PersonalIndicatorLog Insert(PersonalIndicatorLog personalIndicatorLog);
        PersonalIndicatorLog Update(PersonalIndicatorLog personalIndicatorLog);
    }
}
