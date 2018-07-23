using System;
using System.Collections.Generic;
using System.Text;
using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;

namespace Belcorp.Encore.Repositories.Interfaces
{
    public interface IRequirementTitleCalculationsRepository : IRepository<RequirementTitleCalculations>
    {
        IEnumerable<RequirementTitleCalculations> GeRequirementTitleCalculations(string country = null);
    }
}
