using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Belcorp.Encore.Repositories
{
    public interface IBonusDetailsRepository : IRepository<BonusDetails>
    {
        IEnumerable<BonusDetails> GetBonusDetails(string country = null, int? periodId = null);
    }
}