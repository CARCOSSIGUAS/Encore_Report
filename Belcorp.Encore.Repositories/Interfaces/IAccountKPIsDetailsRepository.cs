using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Belcorp.Encore.Repositories
{
    public interface IAccountKPIsDetailsRepository : IRepository<AccountKPIsDetails>
    {
        IEnumerable<AccountKPIsDetails> GetAccountKPIsDetails(string country = null, int? periodId = null);
    }
}