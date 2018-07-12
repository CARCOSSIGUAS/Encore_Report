using Belcorp.Encore.Entities.Entities.Commissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface ICalculationTypesService
    {
        List<CalculationTypes> GetCalculationTypesByCodes(List<string> codes);
        int GetCalculationTypeIdByCode(string code);
    }
}
