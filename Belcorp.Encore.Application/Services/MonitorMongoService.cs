using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class MonitorMongoService : IMonitorMongoService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IMonitorMongoRepository monitorMongoRepository;

        private readonly IAccountsService accountsService;

        public MonitorMongoService(IUnitOfWork<EncoreCore_Context> _unitOfWork_Core, IMonitorMongoRepository _monitorMongoRepository, IAccountsService _accountsService)
        {
            unitOfWork_Core = _unitOfWork_Core;
            encoreMongo_Context = new EncoreMongo_Context();
            monitorMongoRepository = _monitorMongoRepository;
            accountsService = _accountsService;
        }

        public void Migrate()
        {
            using (unitOfWork_Core)
            {
                var items = monitorMongoRepository.GetDataForProcess();

                foreach (var item in items)
                {
                    switch (item.TableNamePrincipal)
                    {
                        case "Accounts":
                            Migrate_AccountsById(item.RowId);
                            break;
                        case "Titles":
                            break;
                        default:
                            break;
                    }

                    item.Process = true;
                    unitOfWork_Core.SaveChanges();
                }
            }
        }

        public void Migrate_AccountsById(int accountId)
        {
            IRepository<Accounts> accountsRepository = unitOfWork_Core.GetRepository<Accounts>();
            var account = accountsRepository.GetFirstOrDefault(a => a.AccountID == accountId, null, null, true);

            Accounts_DTO account_DTO = new Accounts_DTO()
            {
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

                AccountPhones = account.AccountPhones,
            };
        }
    }
}
