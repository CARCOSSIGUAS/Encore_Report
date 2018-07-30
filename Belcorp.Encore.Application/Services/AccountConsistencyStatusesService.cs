using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Belcorp.Encore.Application.Services
{
    public class AccountConsistencyStatusesService : IAccountConsistencyStatusesService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;

        public AccountConsistencyStatusesService(IUnitOfWork<EncoreCore_Context> _unitOfWork_Core)
        {
            unitOfWork_Core = _unitOfWork_Core;
        }

        public AccountConsistencyStatuses GetAccountConsistencyStatusID(int sortIndex)
        {
            IRepository<AccountConsistencyStatuses> accountConsistencyStatusesRepository = unitOfWork_Core.GetRepository<AccountConsistencyStatuses>();
            var result = accountConsistencyStatusesRepository.GetFirstOrDefault(c => c.SortIndex == sortIndex, o => o.OrderBy(a => a.AccountConsistencyStatusID), null, true);
            if (result != null)
            {
                return result;
            }

            return result;
        }
    }
}
