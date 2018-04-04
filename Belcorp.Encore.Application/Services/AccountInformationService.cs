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
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.DTO;

namespace Belcorp.Encore.Application
{
    public class AccountInformationService : IAccountInformationService 
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAccountInformationRepository accountInformationRepository;
		private readonly ITitlesRepository titlesRepository;

        public AccountInformationService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm, IUnitOfWork<EncoreCore_Context> _unitOfWork_Core, IAccountInformationRepository _accountInformationRepository,ITitlesRepository _titlesRepository)
        {
            unitOfWork_Comm = _unitOfWork_Comm;
            unitOfWork_Core = _unitOfWork_Core;
            accountInformationRepository = _accountInformationRepository;
			titlesRepository = _titlesRepository;
            encoreMongo_Context = new EncoreMongo_Context();
        }

        public void Migrate_AccountInformationByPeriod(int periodId)
        {
            var total = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;
			var titles = titlesRepository.GetAll().AsQueryable().ToList();


			encoreMongo_Context.AccountsInformationProvider.DeleteMany(p => p.PeriodID == periodId);

			for (int i = 0; i < ii; i++)
			{
				var accountsInformation = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 10000, true).Items;

				var x = from accountsInfo in accountsInformation
						join titlesInfo in titles on Int32.Parse(accountsInfo.CareerTitle) equals titlesInfo.TitleID
						join titlesInfo2 in titles on Int32.Parse(accountsInfo.PaidAsCurrentMonth) equals titlesInfo2.TitleID
						select new AccountsInformation_DTO
						{
							AccountsInformationID = accountsInfo.AccountsInformationID,
							PeriodID = accountsInfo.PeriodID,
							AccountID = accountsInfo.AccountID,
							AccountNumber = accountsInfo.AccountNumber,
							AccountName = accountsInfo.AccountName,
							SponsorID = accountsInfo.SponsorID,
							SponsorName = accountsInfo.SponsorName,
							Address = accountsInfo.Address,
							PostalCode = accountsInfo.PostalCode,
							City = accountsInfo.City,
							STATE = accountsInfo.STATE,

							PQV = accountsInfo.PQV,
							DQV = accountsInfo.DQV,
							DQVT = accountsInfo.DQVT,

							CareerTitle = accountsInfo.CareerTitle,
							PaidAsCurrentMonth = accountsInfo.PaidAsCurrentMonth,
							CareerTitle_Des = titlesInfo.Name,
							PaidAsCurrentMonth_Des = titlesInfo2.Name,

							JoinDate = accountsInfo.JoinDate,
							Generation = accountsInfo.Generation,
							LEVEL = accountsInfo.LEVEL,
							SortPath = accountsInfo.SortPath,
							LeftBower = accountsInfo.LeftBower,
							RightBower = accountsInfo.RightBower,
							Activity = accountsInfo.Activity
						};

				encoreMongo_Context.AccountsInformationProvider.InsertMany(x);
			}

		}
    }
}
