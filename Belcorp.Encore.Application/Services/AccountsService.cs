using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Repositories;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;


namespace Belcorp.Encore.Application.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork;
        private readonly IAccountsRepository accountsRepository; 
        private readonly EncoreMongo_Context encoreMongo_Context;

        public AccountsService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork, IAccountsRepository _accountsRepository)
        {
            unitOfWork = _unitOfWork;
            encoreMongo_Context = new EncoreMongo_Context();
            accountsRepository = _accountsRepository;
        }

        public void Migrate_Accounts()
        {
            IRepository<Accounts> accountsRepository = unitOfWork.GetRepository<Accounts>();
            encoreMongo_Context.AccountsProvider.DeleteMany(new BsonDocument { });
            var total = accountsRepository.GetPagedList(null, null, null, 0, 20000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var data = accountsRepository.GetPagedList(null, null, null, i, 20000, true).Items;
                encoreMongo_Context.AccountsProvider.InsertMany(data);
            }
        }
    }
}
