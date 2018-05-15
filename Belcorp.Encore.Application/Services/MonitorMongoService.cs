using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Constants;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class MonitorService : IMonitorMongoService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IMonitorRepository monitorMongoRepository;

        private readonly IAccountsService accountsService;

        public MonitorService
        (
            IUnitOfWork<EncoreCore_Context> _unitOfWork_Core, 
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm,
            IMonitorRepository _monitorMongoRepository, 
            IAccountsService _accountsService, 
            IConfiguration configuration
        )
        {
            unitOfWork_Core = _unitOfWork_Core;
            unitOfWork_Comm = _unitOfWork_Comm;
            encoreMongo_Context = new EncoreMongo_Context(configuration);
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
                    switch (item.TableIdPrincipal)
                    {
                        case (int)Constants.MonitorTables.Accounts:
                            Migrate_AccountsById(item);
                            break;
                        case (int)Constants.MonitorTables.Periods:
                            Migrate_PeriodsbyId(item);
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

        #region Accounts
        public void Migrate_AccountsById(Monitor monitor)
        {
            //IRepository<Entities.Entities.Core.Accounts> accountsRepository = unitOfWork_Core.GetRepository<Entities.Entities.Core.Accounts>();

            //var account = accountsRepository.GetFirstOrDefault(a => a.AccountID == monitor.RowId, null, a => a.Include(p => p.AccountPhones), true);
            //var account_Mongo = encoreMongo_Context.AccountsProvider.Find(a => a.CountryID == 0 && a.AccountID == account.AccountID).FirstOrDefault();

            //Accounts_Mongo registro = new Accounts_Mongo()
            //{
            //    CountryID = 0,
            //    AccountID = account.AccountID,

            //    AccountNumber = account.AccountNumber,
            //    AccountTypeID = account.AccountTypeID,
            //    FirstName = account.FirstName,
            //    MiddleName = account.MiddleName,
            //    LastName = account.LastName,
            //    EmailAddress = account.EmailAddress,
            //    SponsorID = account.SponsorID,
            //    EnrollerID = account.EnrollerID,
            //    EnrollmentDateUTC = account.EnrollmentDateUTC,
            //    IsEntity = account.IsEntity,
            //    AccountStatusChangeReasonID = account.AccountStatusChangeReasonID,
            //    AccountStatusID = account.AccountStatusID,
            //    EntityName = account.EntityName,

            //    BirthdayUTC = account.BirthdayUTC,
            //    TerminatedDateUTC = account.TerminatedDateUTC,

            //    AccountPhones = account.AccountPhones
            //};

            //if (account == null)
            //{
            //    encoreMongo_Context.AccountsProvider.DeleteOne(a => a.CountryID == 0 && a.AccountID == monitor.RowId);
            //}
            //else if (account_Mongo == null)
            //{
            //    encoreMongo_Context.AccountsProvider.InsertOne(registro);
            //}
            //else
            //{
            //    var updatesAttributes = Builders<Accounts_Mongo>.Update
            //    .Set(a => a.AccountNumber, account.AccountNumber)
            //    .Set(a => a.AccountTypeID, account.AccountTypeID)
            //    .Set(a => a.FirstName, account.FirstName)
            //    .Set(a => a.MiddleName, account.MiddleName)
            //    .Set(a => a.LastName, account.LastName)
            //    .Set(a => a.EmailAddress, account.EmailAddress)
            //    .Set(a => a.SponsorID, account.SponsorID)
            //    .Set(a => a.EnrollerID, account.EnrollerID)
            //    .Set(a => a.EnrollmentDateUTC, account.EnrollmentDateUTC);

            //    encoreMongo_Context.AccountsProvider.UpdateOne(a => a.CountryID == 0 && a.AccountID == account.AccountID, updatesAttributes);
            //}

            //if (monitor.MonitorDetails != null && monitor.MonitorDetails.Any(md => md.Process == false))
            //{
            //    foreach (var detail in monitor.MonitorDetails)
            //    {
            //        if (detail.Process == false)
            //        {
            //            if (detail.TableIdSecundary == (int)Constants.MonitorTables.AccountsPhones)
            //            {
            //                Migrate_Phones(account, detail);
            //            }

            //            detail.Process = true;
            //            detail.DateProcess = DateTime.Now;
            //        }
            //    }
            //}
        }

        private void Migrate_Phones(Entities.Entities.Core.Accounts account, MonitorDetails detail)
        {
            //IRepository<AccountPhones> accountPhonesRepository = unitOfWork_Core.GetRepository<AccountPhones>();
            //var account_Mongo = encoreMongo_Context.AccountsProvider.Find(a => a.CountryID == 0 && a.AccountID == account.AccountID).FirstOrDefault();

            //var phone = accountPhonesRepository.GetFirstOrDefault(ap => ap.AccountID == account.AccountID && ap.AccountPhoneID == detail.RowDetalleId, null, null, true);
            //var phone_Mongo = account_Mongo.AccountPhones == null ? null : account_Mongo.AccountPhones.FirstOrDefault(p => p.AccountPhoneID == detail.RowDetalleId);

            //UpdateDefinition<Accounts_Mongo> updatesAttributes = null;

            ////No existe en Encore y si existe en Mongo
            //if (phone == null && phone_Mongo != null)
            //{
            //    updatesAttributes = Builders<Accounts_Mongo>.Update.PullFilter(a => a.AccountPhones, builder => builder.AccountPhoneID == detail.RowDetalleId);
            //    encoreMongo_Context.AccountsProvider.UpdateOne(a => a.CountryID == 0 && a.AccountID == account.AccountID && a.AccountPhones.Any(ap => ap.AccountPhoneID == phone_Mongo.AccountPhoneID), updatesAttributes);
            //}
            ////Si existe en Encore y no existe en Mongo
            //else if (phone != null && phone_Mongo == null)
            //{
            //    if (account_Mongo.AccountPhones == null)
            //    {
            //        updatesAttributes = Builders<Accounts_Mongo>.Update.Set(a => a.AccountPhones, new List<AccountPhones> { phone } );
            //    }
            //    else
            //    {
            //        updatesAttributes = Builders<Accounts_Mongo>.Update.Push(a => a.AccountPhones, phone);
            //    }

            //    encoreMongo_Context.AccountsProvider.UpdateOne(a => a.CountryID == 0 && a.AccountID == account.AccountID, updatesAttributes, new UpdateOptions { IsUpsert = true } );
            //}
            ////Si existe en Encore y si existe en Mongo
            //else if (phone != null && phone_Mongo != null)
            //{
            //    updatesAttributes = Builders<Accounts_Mongo>.Update.Set(a => a.AccountPhones[-1].IsDefault, phone.IsDefault)
            //                                 .Set(a => a.AccountPhones[-1].IsPrivate, phone.IsPrivate)
            //                                 .Set(a => a.AccountPhones[-1].PhoneNumber, phone.PhoneNumber)
            //                                 .Set(a => a.AccountPhones[-1].PhoneTypeID, phone.PhoneTypeID);

            //    encoreMongo_Context.AccountsProvider.UpdateOne(a => a.CountryID == 0 && a.AccountID == account.AccountID && a.AccountPhones.Any(ap => ap.AccountPhoneID == phone_Mongo.AccountPhoneID), updatesAttributes);
            //}
        }
        #endregion

        #region Periods
        private void Migrate_PeriodsbyId(Monitor monitor)
        {
            //IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();

            //var period = periodsRepository.GetFirstOrDefault(p => p.PeriodID == monitor.RowId, null, null, true);
            //var period_Mongo = encoreMongo_Context.PeriodsProvider.Find(p => p.PeriodID == period.PeriodID).FirstOrDefault();

            //Periods_Mongo registro = new Periods_Mongo()
            //{
            //    PeriodID = period.PeriodID,

            //    StartDate = period.StartDate,
            //    EndDate = period.EndDate,
            //    ClosedDate = period.ClosedDate,
            //    PlanID = period.PlanID,
            //    EarningsViewable = period.EarningsViewable,
            //    BackOfficeDisplayStartDate = period.BackOfficeDisplayStartDate,
            //    DisbursementsProcessed = period.DisbursementsProcessed,
            //    Description = period.Description,
            //    StartDateUTC = period.StartDateUTC,
            //    EndDateUTC = period.EndDateUTC,
            //    LockDate = period.LockDate
            //};

            //if (period == null)
            //{
            //    encoreMongo_Context.PeriodsProvider.DeleteOne(p => p.PeriodID == monitor.RowId);
            //}
            //else if (period_Mongo == null)
            //{
            //    encoreMongo_Context.PeriodsProvider.InsertOne(registro);
            //}
            //else
            //{
            //    var updatesAttributes = Builders<Periods_Mongo>.Update
            //    .Set(p => p.StartDate, period.StartDate)
            //    .Set(p => p.EndDate, period.EndDate)
            //    .Set(p => p.ClosedDate, period.ClosedDate)
            //    .Set(p => p.PlanID, period.PlanID)
            //    .Set(p => p.EarningsViewable, period.EarningsViewable)
            //    .Set(p => p.BackOfficeDisplayStartDate, period.BackOfficeDisplayStartDate)
            //    .Set(p => p.DisbursementsProcessed, period.DisbursementsProcessed)
            //    .Set(p => p.Description, period.Description)
            //    .Set(p => p.StartDateUTC, period.StartDateUTC)
            //    .Set(p => p.EndDateUTC, period.EndDateUTC)
            //    .Set(p => p.LockDate, period.LockDate);

            //    encoreMongo_Context.PeriodsProvider.UpdateOne(p => p.PeriodID == period.PeriodID, updatesAttributes);
            //}

        }
        #endregion
    }
}
