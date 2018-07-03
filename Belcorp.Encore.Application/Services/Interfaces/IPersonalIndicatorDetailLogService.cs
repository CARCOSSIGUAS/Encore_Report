using Belcorp.Encore.Entities.Entities.Commissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IPersonalIndicatorDetailLogService
    {
        PersonalIndicatorDetailLog Get(PersonalIndicatorDetailLog personalIndicatorDetailLog);
        PersonalIndicatorDetailLog Insert(PersonalIndicatorDetailLog personalIndicatorDetailLog);
        PersonalIndicatorDetailLog Update(PersonalIndicatorDetailLog personalIndicatorDetailLog);
        PersonalIndicatorDetailLog Create(PersonalIndicatorLog personalIndicatorLog, string codeSubProcess);
    }
}
