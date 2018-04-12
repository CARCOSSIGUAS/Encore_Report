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
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Services.Report.ViewModel;
using Belcorp.Encore.Application.ViewModel;

using System.Threading.Tasks;
using Belcorp.Encore.Entities.Entities.DTO;
using Microsoft.Extensions.Options;
using Belcorp.Encore.Data;

namespace Belcorp.Encore.Application
{
    public class AccountInformationService : IAccountInformationService
    {
        private readonly IUnitOfWork<EncoreCore_Context> unitOfWork_Core;
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;

        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IAccountInformationRepository accountInformationRepository;

        public AccountInformationService
        (
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm, 
            IUnitOfWork<EncoreCore_Context> _unitOfWork_Core, 
            IAccountInformationRepository _accountInformationRepository, 
            IOptions<Settings> settings
        )
        {
            unitOfWork_Comm = _unitOfWork_Comm;
            unitOfWork_Core = _unitOfWork_Core;
            accountInformationRepository = _accountInformationRepository;
            encoreMongo_Context = new EncoreMongo_Context(settings);
        }

        public void Migrate_AccountInformationByPeriod(int periodId)
        {
            var total = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            IRepository<Titles> titlesRepository = unitOfWork_Comm.GetRepository<Titles>();
            var titles = titlesRepository.GetAll().ToList();

            encoreMongo_Context.AccountsInformationProvider.DeleteMany(p => p.PeriodID == periodId && p.CountryID == 0);

            for (int i = 0; i < ii; i++)
            {
                var accountsInformation = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 10000, true).Items;

                var result = from accountsInfo in accountsInformation
                             join titlesInfo_Career in titles on Int32.Parse(accountsInfo.CareerTitle) equals titlesInfo_Career.TitleID
                             join titlesInfo_Paid in titles on Int32.Parse(accountsInfo.PaidAsCurrentMonth) equals titlesInfo_Paid.TitleID
                             select new AccountsInformation_Mongo
                             {
                                 CountryID = 0,
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
                                 CareerTitle_Des = titlesInfo_Career.Name,
                                 PaidAsCurrentMonth_Des = titlesInfo_Paid.Name,

                                 JoinDate = accountsInfo.JoinDate,
                                 Generation = accountsInfo.Generation,
                                 LEVEL = accountsInfo.LEVEL,
                                 SortPath = accountsInfo.SortPath,
                                 LeftBower = accountsInfo.LeftBower,
                                 RightBower = accountsInfo.RightBower,
                                 Activity = accountsInfo.Activity
                             };

                encoreMongo_Context.AccountsInformationProvider.InsertMany(result);
            }
        }

        public async Task<IEnumerable<ReportPerformance_HeaderModel>> GetPerformance_Header(int accountId, int periodId)
        {
            var header = await encoreMongo_Context.AccountsInformationProvider.Find(p => p.AccountID == accountId && p.PeriodID == periodId, null).Project(Builders<AccountsInformation_Mongo>.Projection.Exclude("_id")).As<AccountsInformation_Mongo>().ToListAsync();

            var reportHeader = header.Select(qq => new ReportPerformance_HeaderModel { VP = qq.PQV, VOT = qq.DQV, VOQ = qq.DQVT, IdTituloCarrera = qq.CareerTitle, TituloCarrera = qq.CareerTitle_Des, IdTituloPago = qq.PaidAsCurrentMonth, TituloPago = qq.PaidAsCurrentMonth_Des, BrazosActivos = "" });

            return reportHeader;
        }

        public async Task<List<AccountsInformation_Mongo>> GetPerformance_AccountInformation(int accountId, int periodId)
        {
            var devolver = await encoreMongo_Context.AccountsInformationProvider.AsQueryable().Where(p => p.AccountID == accountId && p.PeriodID == periodId).ToListAsync();

            return devolver;
        }

		public AccountsExtended GetAccounts(Filtros_DTO filtrosDTO)
		{
			//Usuario Logeado
			var sponsorSearch = GetPerformance_HeaderFront(filtrosDTO.CodConsultoraSearched, filtrosDTO.Period);

			var sponsorFilter = encoreMongo_Context.AccountsInformationProvider.AsQueryable().Where(q =>
													 q.LeftBower >=filtrosDTO.LeftBower && q.RightBower <= filtrosDTO.RigthBower && q.PeriodID == filtrosDTO.Period &&  (q.PaidAsLastMonth.Contains(filtrosDTO.TituloPago ?? "") || string.IsNullOrEmpty(q.PaidAsLastMonth))
													 && (q.LeftBower<= sponsorSearch.LeftBower && q.RightBower <= sponsorSearch.RigthBower)
													 && (q.CareerTitle.Contains(filtrosDTO.TituloCarrera??"") || string.IsNullOrEmpty(q.CareerTitle))
													 && (q.STATE.Contains(filtrosDTO.Estado??"") ||( string.IsNullOrEmpty(q.STATE) ||(string.IsNullOrEmpty(filtrosDTO.Estado))) )
													 && (q.PQV == filtrosDTO.VentaPersonal || (q.PQV == 0) || (filtrosDTO.VentaPersonal == 0))
													 && (q.LEVEL == filtrosDTO.Nivel || (q.LEVEL == 0) || (filtrosDTO.Nivel == 0))
													 && (q.Generation == filtrosDTO.Generation || (q.Generation == 0) || (filtrosDTO.Generation == 0))
													 && (q.DQVT == filtrosDTO.VOT || (q.DQVT == 0) || (filtrosDTO.VOT == 0))
													 && (q.DQV == filtrosDTO.VOQ || (q.DQV == 0) || (filtrosDTO.VOQ==0))												
													).ToList();

			var devolverData = sponsorFilter.Skip(filtrosDTO.NumeroPagina).Take(filtrosDTO.NumeroRegistros).ToList();

			var totalPages = sponsorFilter.Count() / (filtrosDTO.NumeroRegistros);

			return new AccountsExtended { numPage = totalPages, accountsInformationDTO = devolverData };
			
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


		public AccountsInformationExtended GetPerformance_HeaderFront(int accountId,int period)
        {
			try
			{
				var header = encoreMongo_Context.AccountsProvider.Find(q => q.AccountID == accountId, null).ToList();



				var headerByAccountInformation = from headerInitial in header
												 join reportAccountInitial in encoreMongo_Context.AccountsInformationProvider.Find(c=>c.AccountID==accountId && c.PeriodID==period).ToList() on headerInitial.AccountID equals reportAccountInitial.AccountID
												 select new AccountsInformationExtended { LeftBower=reportAccountInitial.LeftBower, RigthBower=reportAccountInitial.RightBower , accounts_Mongo= headerInitial };
												 

				//var headerByAccountInformation = encoreMongo_Context.AccountsInformationProvider.Find(c => c.AccountID == accountId && c.PeriodID == period).ToList();

				
				return headerByAccountInformation.FirstOrDefault();
				
            }
            catch (Exception ex)
            {
                throw new Exception { };
            }
        }
	}
}
