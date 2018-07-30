using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class SystemConfigsService : ISystemConfigsService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        public SystemConfigsService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm)
        {
            unitOfWork_Comm = _unitOfWork_Comm;
        }

        public SystemConfigs GetAmountParameter(string configCode)
        {
            IRepository<SystemConfigs> systemConfigsRepository = unitOfWork_Comm.GetRepository<SystemConfigs>();
            var result = systemConfigsRepository.GetFirstOrDefault(c => c.ConfigCode == configCode, null, null, true);
            if (result != null)
            {
                return result;
            }

            return result;
        }
    }
}
