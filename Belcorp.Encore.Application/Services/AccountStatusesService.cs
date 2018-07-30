using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class AccountStatusesService : IAccountStatusesService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;

        public AccountStatusesService(IUnitOfWork<EncoreCore_Context> _unitOfWork_Core)
        {
            unitOfWork_Core = _unitOfWork_Core;
        }

        public AccountStatuses LkpAccountStatusID(string name)
        {
            IRepository<AccountStatuses> accountStatusesRepository = unitOfWork_Core.GetRepository<AccountStatuses>();
            var result = accountStatusesRepository.GetFirstOrDefault(c => c.Name == name, null, null, true);
            if (result != null)
            {
                return result;
            }

            return result;
        }
    }
}
