using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Belcorp.Encore.Application
{
    public class AccountInformationService : IAccountInformationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAccountInformationRepository _accountInformationRepository;
        
        public AccountInformationService(IUnitOfWork unitOfWork, IAccountInformationRepository accountInformationRepository)
        {
            _accountInformationRepository = accountInformationRepository;
            _unitOfWork = unitOfWork;
            encoreMongo_Context = new EncoreMongo_Context();
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId)
        {
            return _accountInformationRepository.GetListAccountInformationByPeriodId(periodId);
        }

        public void CalcularAccountInformation(int periodId, int accountId)
        {
            using (_unitOfWork)
            {
                var rai = _accountInformationRepository.GetList(r => r.PeriodID == periodId && r.AccountID == accountId);
                _accountInformationRepository.Delete(rai);
                _unitOfWork.SaveChanges();
            }
        }

        public void SaveCollection(int periodId)
        {
            var result = GetListAccountInformationByPeriodId(periodId).ToList();
            encoreMongo_Context.AccountsInformationProvider.InsertMany(result);
        }
    }
}
