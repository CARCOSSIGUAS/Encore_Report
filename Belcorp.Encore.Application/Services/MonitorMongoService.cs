using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
                            Migrate_AccountsById(item);
                            break;
                        case "Titles":
                            break;
                        default:
                            break;
                    }

                    item.Process = true;
                    item.DateProcess = DateTime.Now;
                    unitOfWork_Core.SaveChanges();
                }
            }
        }

        public void Migrate_AccountsById(MonitorMongo monitor)
        {
            IRepository<Accounts> accountsRepository = unitOfWork_Core.GetRepository<Accounts>();

            var account = accountsRepository.GetFirstOrDefault(a => a.AccountID == monitor.RowId, null, null, true);
            var account_Mongo = encoreMongo_Context.AccountsProvider.Find(a => a.AccountID == account.AccountID).FirstOrDefault();

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

                AccountPhones = new List<AccountPhones>()                
            };

            if (account == null)
            {
                encoreMongo_Context.AccountsProvider.DeleteOne(a => a.AccountID == monitor.RowId);
            }
            else if (account_Mongo == null)
            {
                encoreMongo_Context.AccountsProvider.InsertOne(account_DTO);
            }
            else
            {
                var updatesAttributes = Builders<Accounts_DTO>.Update
                .Set(a => a.AccountNumber, account.AccountNumber)
                .Set(a => a.AccountTypeID, account.AccountTypeID)
                .Set(a => a.FirstName, account.FirstName)
                .Set(a => a.MiddleName, account.MiddleName)
                .Set(a => a.LastName, account.LastName)
                .Set(a => a.EmailAddress, account.EmailAddress)
                .Set(a => a.SponsorID, account.SponsorID)
                .Set(a => a.EnrollerID, account.EnrollerID)
                .Set(a => a.EnrollmentDateUTC, account.EnrollmentDateUTC);

                encoreMongo_Context.AccountsProvider.UpdateOne(a => a.AccountID == account.AccountID, updatesAttributes);
            }

            if (monitor.MonitorMongoDetails != null)
            {
                foreach (var detail in monitor.MonitorMongoDetails)
                {
                    if (detail.TableNameSecundary == "AccountPhones")
                    {
                        Migrate_Phones(account, account_Mongo, detail);
                    }

                    detail.Process = true;
                    detail.DateProcess = DateTime.Now;
                }
            }
        }

        private void Migrate_Phones(Accounts account, Accounts_DTO account_Mongo, MonitorMongoDetails detail)
        {
            IRepository<AccountPhones> accountPhonesRepository = unitOfWork_Core.GetRepository<AccountPhones>();

            var phone = accountPhonesRepository.GetFirstOrDefault(ap => ap.AccountID == account.AccountID && ap.AccountPhoneID == detail.RowDetalleId, null, null, true);
            var phone_Mongo = account_Mongo.AccountPhones == null ? null : account_Mongo.AccountPhones.FirstOrDefault(p => p.AccountPhoneID == detail.RowDetalleId);

            if (phone == null)
            {
                var updatesAttributes = Builders<Accounts_DTO>.Update.PullFilter(a => a.AccountPhones, builder => builder.AccountPhoneID == detail.RowDetalleId);

                encoreMongo_Context.AccountsProvider.UpdateOne(
                              a => a.AccountID == account.AccountID && a.AccountPhones.Any(ap => ap.AccountPhoneID == phone_Mongo.AccountPhoneID),
                              updatesAttributes
                              );
            }
            else if (phone_Mongo == null)
            {
                var updatesAttributes = Builders<Accounts_DTO>.Update
                                             .Push(a => a.AccountPhones, phone);

                //encoreMongo_Context.AccountsProvider.UpdateOne(a => a.AccountID == account.AccountID, updatesAttributes, new UpdateOptions { IsUpsert = true } );
            }
            else
            {
                var updatesAttributes = Builders<Accounts_DTO>.Update.Set(a => a.AccountPhones[-1].IsDefault, phone.IsDefault)
                                             .Set(a => a.AccountPhones[-1].IsPrivate, phone.IsPrivate)
                                             .Set(a => a.AccountPhones[-1].PhoneNumber, phone.PhoneNumber)
                                             .Set(a => a.AccountPhones[-1].PhoneTypeID, phone.PhoneTypeID);

                encoreMongo_Context.AccountsProvider.UpdateOne(
                              a => a.AccountID == account.AccountID && a.AccountPhones.Any(ap => ap.AccountPhoneID == phone_Mongo.AccountPhoneID),
                              updatesAttributes
                              );
            }
        }
    }
}
