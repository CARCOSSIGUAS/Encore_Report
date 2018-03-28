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
using Belcorp.Encore.Services.Report.ViewModel;
using Belcorp.Encore.Application.ViewModel;
using Belcorp.Encore.Entities.Entities.MetaData_Mongo;

namespace Belcorp.Encore.Application
{
	public class AccountInformationService : IAccountInformationService
	{
		private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
		private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

		private readonly EncoreMongo_Context encoreMongo_Context;
		private readonly IAccountInformationRepository accountInformationRepository;

		public AccountInformationService(IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm, IUnitOfWork<EncoreCore_Context> _unitOfWork_Core, IAccountInformationRepository _accountInformationRepository)
		{
			unitOfWork_Comm = _unitOfWork_Comm;
			unitOfWork_Core = _unitOfWork_Core;
			accountInformationRepository = _accountInformationRepository;
			encoreMongo_Context = new EncoreMongo_Context();
		}

		[Obsolete]
		public void Migrate_AccountInformationWithAccountsByPeriod()
		{
			int periodId = 201703;

			var total = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 20000, true);
			int ii = total.TotalPages;

			encoreMongo_Context.AccountsInformationProvider.DeleteMany(p => p.PeriodID == periodId);
			IRepository<Accounts> accountsRepository = unitOfWork_Core.GetRepository<Accounts>();

			var accounts = accountsRepository.GetAll().ToList();
			for (int i = 0; i < ii; i++)
			{
				var accountsInformation = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 20000, true).Items;
				var data =
				accountsInformation.Join(accounts, r => r.AccountID, a => a.AccountID, (r, a) => new { r, a }).Select(result => new AccountsInformation_DTO
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
					RightBower = result.r.RightBower

				});

				encoreMongo_Context.AccountsInformationProvider.InsertMany(data);
			}
		}

		public void Migrate_AccountInformationByPeriod()
		{
			int periodId = 201803;

			var total = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 20000, true);
			int ii = total.TotalPages;

			encoreMongo_Context.AccountsInformationProvider.DeleteMany(p => p.PeriodID == periodId);

			for (int i = 0; i < ii; i++)
			{
				var accountsInformation = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 20000, true).Items;
				var data =
				accountsInformation.Select(result => new AccountsInformation_DTO
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

					PQV = result.PQV,
					DQV = result.DQV,
					DQVT = result.DQVT,

					CareerTitle = result.CareerTitle,
					PaidAsCurrentMonth = result.PaidAsCurrentMonth,
					CareerTitle_Des = "",
					PaidAsCurrentMonth_Des = "",

					JoinDate = result.JoinDate,
					Generation = result.Generation,
					LEVEL = result.LEVEL,
					SortPath = result.SortPath,
					LeftBower = result.LeftBower,
					RightBower = result.RightBower,
					Activity = result.Activity
				});

				encoreMongo_Context.AccountsInformationProvider.InsertMany(data);
			}
		}

		public IEnumerable<ReportPerformance_HeaderModel> GetPerformance_Header(int accountId, int periodId)
		{
			var header = encoreMongo_Context.AccountsInformationProvider.Find(p => p.AccountID == accountId && p.PeriodID == periodId, null).Project(qq => new ReportPerformance_HeaderModel { VP = qq.PQV, VOT = qq.DQV, VOQ = qq.DQVT, IdTituloCarrera = qq.CareerTitle, TituloCarrera = qq.CareerTitle_Des, IdTituloPago = qq.PaidAsCurrentMonth, TituloPago = qq.PaidAsCurrentMonth_Des, BrazosActivos = "" }).ToList();

			return header;
		}

		public IEnumerable<ReportPerformance_DetailModel> GetPerformance_Detail(int accountId, int sponsorId, int periodId)
		{

			var accountInformationCollection = encoreMongo_Context.AccountsInformationProvider.Find(q => q.AccountID == accountId && q.SponsorID == sponsorId && q.PeriodID == periodId);

			var accountCollection = encoreMongo_Context.AccountsProvider.Find(t => t.AccountID == accountId | t.AccountID == sponsorId);
							
			//var detail = encoreMongo_Context.AccountsInformationProvider.Find(new BsonDocument).ToList
			//				.Aggregate()
			//				.Lookup<AccountsInformation_DTO,Accounts_DTO,AccountsInformationbyAccount_DTO>(
			//				encoreMongo_Context.Database.GetCollection<Accounts_DTO>("Accounts"),
			//					p => p.AccountID,
			//					q=>q.AccountID,
			//					r=>r.Accounts_DTOs						
			//				);

			//detail = detail.Project(qq => new ReportPerformance_DetailModel { Nombre=qq. }).ToList();



		}
	}
}
