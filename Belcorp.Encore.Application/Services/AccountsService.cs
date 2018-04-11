using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        private readonly IAccountsRepository accountsRepository; 
        private readonly EncoreMongo_Context encoreMongo_Context;

        public AccountsService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm, IUnitOfWork<EncoreCore_Context> _unitOfWork_Core, IAccountsRepository _accountsRepository)
        {
            unitOfWork_Core = _unitOfWork_Core;
            unitOfWork_Comm = _unitOfWork_Comm;
            encoreMongo_Context = new EncoreMongo_Context();
            accountsRepository = _accountsRepository;
        }

        public void Migrate_Accounts()
        {
            IRepository<Accounts> accountsRepository = unitOfWork_Core.GetRepository<Accounts>();
            encoreMongo_Context.AccountsProvider.DeleteMany(new BsonDocument { });
            var total = accountsRepository.GetPagedList(null, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var accounts = accountsRepository.GetPagedList(null, null, a => a.Include(p => p.AccountPhones), i, 10000, true).Items;

                List<Accounts_Mongo> accounts_Mongo = new List<Accounts_Mongo>();
                foreach (var account in accounts)
                {
                    Accounts_Mongo account_Mongo = new Accounts_Mongo()
                    {
                        CountryID = 0,
                        AccountID = account.AccountID,

                        AccountNumber = account.AccountNumber,
                        AccountTypeID = account.AccountTypeID,
                        FirstName = account.FirstName,
                        MiddleName = account.MiddleName,
                        LastName = account.LastName,
                        EmailAddress = account.EmailAddress,
                        SponsorID = account.SponsorID,
                        EnrollerID = account.EnrollerID,
                        EnrollmentDateUTC = account.EnrollmentDateUTC,
                        IsEntity = account.IsEntity,
                        AccountStatusChangeReasonID = account.AccountStatusChangeReasonID,
                        AccountStatusID = account.AccountStatusID,
                        EntityName = account.EntityName,

                        BirthdayUTC = account.BirthdayUTC,
                        TerminatedDateUTC = account.TerminatedDateUTC,
                        
                        AccountPhones = account.AccountPhones

                    };

                    accounts_Mongo.Add(account_Mongo);
                }

                encoreMongo_Context.AccountsProvider.InsertMany(accounts_Mongo);
            }
        }

        public async Task<List<Accounts_Mongo>> GetListAccounts(int accountId)
        {
            var result = await encoreMongo_Context.AccountsProvider.Find(a => a.AccountID == accountId).Project(Builders<Accounts_Mongo>.Projection.Exclude("_id")).As<Accounts_Mongo>().ToListAsync();
            return result;
        }
    }
}
