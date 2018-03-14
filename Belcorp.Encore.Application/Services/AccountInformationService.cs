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
using Belcorp.Encore.Application.Services;

namespace Belcorp.Encore.Application
{
    public class AccountInformationService : IAccountInformationService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> _unitOfWork;
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAccountInformationRepository _accountInformationRepository;

        public AccountInformationService(IUnitOfWork<EncoreCommissions_Context> unitOfWork, IAccountInformationRepository accountInformationRepository)
        {
            _accountInformationRepository = accountInformationRepository;
            _unitOfWork = unitOfWork;
            encoreMongo_Context = new EncoreMongo_Context();
        }

        public IEnumerable<AccountsInformation> GetListAccountInformationByPeriodId(int periodId)
        {
            return _accountInformationRepository.GetListAccountInformationByPeriodId(periodId);
        }

        public IEnumerable<Report_Downline> GetListAccountInformationByPeriodIdAndAccountId(int periodId, int accountId)
        {
            return _accountInformationRepository.GetListAccountInformationByPeriodIdAndAccountId(periodId, accountId);
        }

        public void Migrate_AccountInformationWithAccountsByPeriod(int periodId)
        {
            var total = _accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 20000, true);
            int ii = total.TotalPages;

            encoreMongo_Context.AccountsInformationProvider.DeleteMany( p => p.PeriodID == periodId);
            IRepository<Accounts> _accountsRepository = _unitOfWork.GetRepository<Accounts>();

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

                    JoinDate = result.r.JoinDate,
                    Generation = result.r.Generation,
                    LEVEL = result.r.LEVEL,
                    SortPath = result.r.SortPath,
                    LeftBower = result.r.LeftBower,
                    RightBower = result.r.RightBower,

                    accounts = result.a
                });

                encoreMongo_Context.AccountsInformationProvider.InsertMany(data);
            }
        }

        public void Migrate_AccountInformationByPeriod(int periodId)
        {
            var total = _accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 20000, true);
            int ii = total.TotalPages;

            encoreMongo_Context.AccountsInformationProvider.DeleteMany(p => p.PeriodID == periodId);

            for (int i = 0; i < ii; i++)
            {
                var accountsInformation = _accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 20000, true).Items;
                var data =
                accountsInformation.Select(result => new Report_Downline
                {
                    AccountsInformationID = result.AccountsInformationID,
                    PeriodID = result.PeriodID,
                    AccountID = result.AccountID,
                    AccountNumber = result.AccountNumber,
                    AccountName = result.AccountName,
                    SponsorID = result.SponsorID,
                    SponsorName = result.SponsorName,
                    Address = result.Address,
                    PostalCode = result.PostalCode,
                    City = result.City,
                    STATE = result.STATE,

                    JoinDate = result.JoinDate,
                    Generation = result.Generation,
                    LEVEL = result.LEVEL,
                    SortPath = result.SortPath,
                    LeftBower = result.LeftBower,
                    RightBower = result.RightBower
                });

                encoreMongo_Context.AccountsInformationProvider.InsertMany(data);
            }
        }

        public void Migrate_AccountInformationByAccountId(int periodId, int accountId)
        {
            var account = _accountInformationRepository.GetFirstOrDefault(ai => ai.PeriodID == periodId && ai.AccountID == accountId, null, null, true);
            var result =  _accountInformationRepository.GetListAccountInformationByPeriodIdAndAccountId(periodId, accountId);

            encoreMongo_Context.AccountsInformationProvider.DeleteMany( 
                                                                        ai => ai.PeriodID == periodId && 
                                                                        ai.LeftBower == account.LeftBower && 
                                                                        ai.RightBower == account.RightBower
                                                                      );

            encoreMongo_Context.AccountsInformationProvider.InsertMany(result);
        }
        
        public void ProcessMlmOnline(int orderId, int orderStatusId)
        {
            using (_unitOfWork)
            {
            }
        }
    }
}
