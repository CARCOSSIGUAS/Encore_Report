using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using MongoDB.Bson;

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

        public IEnumerable<AccountsInformation> GetListAccountInformationByAccountId(int accountId)
        {
            return _accountInformationRepository.GetListAccountInformationByAccountId(accountId);
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

        public void Migrate_AccountInformationByAccountId(int accountId)
        {
            var result = GetListAccountInformationByAccountId(accountId);
            encoreMongo_Context.AccountsInformationProvider.InsertMany(result);
        }

        public void Migrate_AccountInformationByPeriod(int periodId)
        {
            var total = _accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var result = _accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 10000, true).Items;
                encoreMongo_Context.AccountsInformationProvider.InsertMany(result);
            }
        }

        public void ProcessMlmOnline(int orderId, int orderStatusId)
        {
            using (_unitOfWork)
            {
            }
        }
    }
}
