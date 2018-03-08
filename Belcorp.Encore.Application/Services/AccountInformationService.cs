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
using MongoDB.Driver;
using Belcorp.Encore.Entities.Entities;

namespace Belcorp.Encore.Application
{
    public class AccountInformationService : IAccountInformationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAccountInformationRepository _accountInformationRepository;
        private readonly IAccountsRepository _accountsRepository;

        public AccountInformationService(IUnitOfWork unitOfWork, IAccountInformationRepository accountInformationRepository, IAccountsRepository accountsRepository)
        {
            _accountInformationRepository = accountInformationRepository;
            _accountsRepository = accountsRepository;
            _unitOfWork = unitOfWork;
            encoreMongo_Context = new EncoreMongo_Context();
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId)
        {
            return _accountInformationRepository.GetListAccountInformationByPeriodId(periodId);
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodIdAndAccountId(int periodId, int accountId)
        {
            return _accountInformationRepository.GetListAccountInformationByPeriodIdAndAccountId(periodId, accountId);
        }

        public void Migrate_AccountInformationByPeriod(int periodId)
        {
            var total = _accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 20000, true);
            int ii = total.TotalPages;
            
            FilterDefinition<Report_Downline> filter = $"{{ PeriodID:  { periodId } }}";
            encoreMongo_Context.AccountsInformationProvider.DeleteMany(filter);

            var accounts = _accountsRepository.GetAll().ToList();
            for (int i = 0; i < ii; i++)
            {
                var accountsInformation = _accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 20000, true).Items;
                var data =
                accountsInformation.Join(accounts, r => r.AccountID, a => a.AccountID, (r, a) => new { r, a }).Select(result => new Report_Downline
                {
                    AccountsInformationID = result.r.AccountsInformationID,
                    PeriodID = result.r.PeriodID,
                    AccountID = result.r.AccountID,
                    AccountNumber = result.r.AccountNumber,
                    AccountName = result.r.AccountName,
                    SponsorID = result.r.SponsorID,
                    SponsorName = result.r.SponsorName,
                    Address = result.r.Address,
                    PostalCode = result.r.PostalCode,
                    City = result.r.City,
                    STATE = result.r.STATE,
                    accounts = result.a
                });

                encoreMongo_Context.AccountsInformationProvider.InsertMany(data);
            }
        }

        public void Migrate_AccountInformationByAccountId(int periodId, int accountId)
        {  
            var accountInformation = _accountInformationRepository.GetFirstOrDefault(x => x.AccountID == accountId, null, null, true);
            //var allTitles = 

            _accountInformationRepository.GetListAccountInformationByPeriodIdAndAccountId(periodId, accountId);
        }
        
        public void ProcessMlmOnline(int orderId, int orderStatusId)
        {
            using (_unitOfWork)
            {
            }
        }
    }
}
