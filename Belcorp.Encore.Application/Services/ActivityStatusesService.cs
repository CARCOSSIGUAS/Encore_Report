using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Entities.Entities.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Belcorp.Encore.Application.Services
{
    public class ActivityStatusesService : IActivityStatusesService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;

        public ActivityStatusesService(IUnitOfWork<EncoreCore_Context> _unitOfWork_Core)
        {
            unitOfWork_Core = _unitOfWork_Core;
        }

        public ActivityStatuses GetActivityIDFromInternalName(string internalName)
        {
            IRepository<ActivityStatuses> activityStatusesRepository = unitOfWork_Core.GetRepository<ActivityStatuses>();
            var result = activityStatusesRepository.GetFirstOrDefault(c => c.InternalName == internalName, o => o.OrderBy(a => a.ActivityStatusID), null, true);
            if (result != null)
            {
                return result;
            }

            return result;
        }
    }
}
