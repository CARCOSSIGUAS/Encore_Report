﻿using Belcorp.Encore.Data.Contexts;
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
using System.Threading.Tasks;

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

		public async Task<IEnumerable<ReportPerformance_HeaderModel>> GetPerformance_Header(int accountId, int periodId)
		{
			var header = await encoreMongo_Context.AccountsInformationProvider.Find(p => p.AccountID == accountId && p.PeriodID == periodId, null).Project(Builders<AccountsInformation_DTO>.Projection.Exclude("_id")).As<AccountsInformation_DTO>().ToListAsync();

			var reportHeader = header.Select(qq => new ReportPerformance_HeaderModel { VP = qq.PQV, VOT = qq.DQV, VOQ = qq.DQVT, IdTituloCarrera = qq.CareerTitle, TituloCarrera = qq.CareerTitle_Des, IdTituloPago = qq.PaidAsCurrentMonth, TituloPago = qq.PaidAsCurrentMonth_Des, BrazosActivos = "" });

			return reportHeader;
		}

		public async Task<List<AccountsInformation_DTO>> GetPerformance_AccountInformation(int accountId, int periodId)
		{
			var devolver = await encoreMongo_Context.AccountsInformationProvider.AsQueryable().Where(p => p.AccountID == accountId && p.PeriodID == periodId).ToListAsync();

			return devolver;
		}

		public async Task<IEnumerable<ReportPerformance_DetailModel>> GetPerformance_Detail(int accountId, int periodId)
        {

			List<ReportPerformance_DetailModel> reportPerformanceDetailModel = new List<ReportPerformance_DetailModel>();

			var detailWA = (from ai in encoreMongo_Context.AccountsInformationProvider.AsQueryable()
							join
						  a in encoreMongo_Context.AccountsProvider.AsQueryable() on
						  ai.SponsorID equals a.AccountID
							where (ai.PeriodID == periodId && (ai.SponsorID == accountId))
							select new
							{
								ai.PeriodID,
								ai.AccountID,
								ai.AccountNumber,
								ai.AccountName,
								ai.SponsorID,
								ai.SponsorName,
								ai.Address,
								ai.PostalCode,
								ai.City,
								ai.STATE,
								ai.Region,
								ai.NewStatus,
								ai.TimeLimitToBeDemote,
								ai.CareerTitle,
								ai.PaidAsCurrentMonth,
								ai.PaidAsLastMonth,
								ai.VolumeForCareerTitle,
								ai.Activity,
								ai.NineMonthsPQV,
								ai.PQV,
								ai.PCV,
								ai.GQV,
								ai.GCV,
								ai.DQVT,
								ai.DCV,
								ai.DQV,
								ai.JoinDate,
								ai.Generation,
								ai.LEVEL,
								ai.SortPath,
								ai.LeftBower,
								ai.RightBower,
								ai.RequirementNewGeneration,
								ai.TimeLimitForNewGeneration,
								ai.Title1Legs,
								ai.Title2Legs,
								ai.Title3Legs,
								ai.Title4Legs,
								ai.Title5Legs,
								ai.Title6Legs,
								ai.Title7Legs,
								ai.Title8Legs,
								ai.Title9Legs,
								ai.Title10Legs,
								ai.Title11Legs,
								ai.Title12Legs,
								ai.Title13Legs,
								ai.Title14Legs,
								ai.EmailAddress,
								ai.CQL,
								ai.LastOrderDate,
								ai.IsCommissionQualified,
								ai.BirthdayUTC,
								ai.UplineLeaderM3,
								ai.UplineLeaderM3Name,
								ai.UplineLeaderL1,
								ai.UplineLeaderL1Name,
								ai.TotalDownline,
								ai.CreditAvailable,
								ai.DebtsToExpire,
								ai.ExpiredDebts,
								ai.GenerationM3,
								ai.ActiveDownline,
								ai.TitleMaintainance,
								ai.SalesAverage,
								ai.NewQualification,
								ai.NewEnrollments,
								ai.NineMonthsGQV,
								ai.NineMonthsDQV,
								ai.ConsultActive,
								a
							});

			var detailWTA = await detailWA.AsQueryable().ToAsyncEnumerable().ToList();

			Parallel.ForEach(detailWTA, detailItem =>
			{
				reportPerformanceDetailModel.Add(new ReportPerformance_DetailModel
				{
					Nombre = detailItem.AccountName,
					Codigo = detailItem.AccountNumber,
					Cumpleanio = detailItem.BirthdayUTC,
					Estado = detailItem.STATE,
					Nivel = detailItem.LEVEL,
					Generacion = detailItem.Generation,
					Status = detailItem.Activity,
					VentaPersonal = detailItem.PQV,
					VOT = detailItem.DQV,
					VOQ = detailItem.DQVT,
					TitCarrera = detailItem.CareerTitle,
					Permanencia = "",
					TitPago = detailItem.PaidAsCurrentMonth,
					CodPatrocinador = detailItem.SponsorID,
					NombrePatrocinador = detailItem.SponsorName,
					EmailPatrocinador = (detailItem.a.AccountID == detailItem.SponsorID) ? detailItem.a.EmailAddress : "",
					TelefonoPatrocinador = (detailItem.a.AccountID == detailItem.SponsorID) ? detailItem.a.AccountPhones.Where(p => p.PhoneTypeID == 1).Select(z => z.PhoneNumber).FirstOrDefault() : "",
					CodLider = detailItem.UplineLeaderM3, //Falta Calc
					NombreLider = detailItem.UplineLeaderM3Name,
					EmailLider = (detailItem.a.AccountID == detailItem.UplineLeaderM3) ? detailItem.a.EmailAddress : "",
					TelefonoLider = (detailItem.a.AccountID == detailItem.UplineLeaderM3) ? detailItem.a.AccountPhones.Where(p => p.PhoneTypeID == 1).Select(z => z.PhoneNumber).FirstOrDefault() : "",
					ConsultoresActivos = 0,
					CantidadEmpresariosGeneracion = 0,
					BrazosActivos = ""
				});

			});

			return reportPerformanceDetailModel;

		}

		public IEnumerable<Accounts_DTO> GetPerformance_HeaderFront(int accountId)
		{
			try
			{
				var header = encoreMongo_Context.AccountsProvider.Find(q => q.AccountID == accountId, null).ToList();

				return header;

			}
			catch (Exception ex)
			{
				throw new Exception { };
			}
		}
	}
}
