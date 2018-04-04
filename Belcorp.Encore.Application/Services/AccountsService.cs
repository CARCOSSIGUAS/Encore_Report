using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.DTO;
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
            var total = accountsRepository.GetPagedList(null, null, null, 0, 20000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var accounts = accountsRepository.GetPagedList(null, null, a => a.Include(p => p.AccountPhones), i, 20000, true).Items;

                List<Accounts_DTO> accounts_aux = new List<Accounts_DTO>();
                foreach (var account in accounts)
                {
                    Accounts_DTO account_DTO = new Accounts_DTO();

                    account_DTO.AccountID = account.AccountID;
                    account_DTO.AccountNumber = account.AccountNumber;
                    account_DTO.AccountTypeID = account.AccountTypeID;
                    account_DTO.FirstName = account.FirstName;
                    account_DTO.MiddleName = account.MiddleName;
                    account_DTO.LastName = account.LastName;
                    account_DTO.EmailAddress = account.EmailAddress;
                    account_DTO.SponsorID = account.SponsorID;
                    account_DTO.EnrollerID = account.EnrollerID;
                    account_DTO.EnrollmentDateUTC = account.EnrollmentDateUTC;
                    account_DTO.IsEntity = account.IsEntity;
                    account_DTO.AccountStatusChangeReasonID = account.AccountStatusChangeReasonID;
                    account_DTO.AccountStatusID = account.AccountStatusID;
                    account_DTO.EntityName = account.EntityName;

                    account_DTO.BirthdayUTC = account.BirthdayUTC;
                    account_DTO.TerminatedDateUTC = account.TerminatedDateUTC;

                    account_DTO.AccountPhones = account.AccountPhones;

                    accounts_aux.Add(account_DTO);
                }

                encoreMongo_Context.AccountsProvider.InsertMany(accounts_aux);
            }
        }

        public async Task<List<Accounts_DTO>> GetListAccounts(int accountId)
        {
            var result = await encoreMongo_Context.AccountsProvider.Find(q => q.AccountID == accountId, null).Project(Builders<Accounts_DTO>.Projection.Exclude("_id")).As<Accounts_DTO>().ToListAsync();
            return result;
        }
    }
}
