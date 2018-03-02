using Belcorp.Encore.Entities;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace Belcorp.Encore.Application
{
    public class AccountInformationService : IAccountInformationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountInformationRepository _accountInformationRepository;
        
        public AccountInformationService(IUnitOfWork unitOfWork, IAccountInformationRepository accountInformationRepository)
        {
            _accountInformationRepository = accountInformationRepository;
            _unitOfWork = unitOfWork;
        }

        public IPagedList<AccountsInformation> GetListAccountInformationByPeriodId(int periodId)
        {
             return _accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 1, 100, false);
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
    }
}
