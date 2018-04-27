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

        public void Migrate_AccountInformationByPeriod(int ? periodId = null)
        {
            if (periodId == null)
            {
                IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();
                var date = DateTime.Now;
                var result = periodsRepository.GetFirstOrDefault(p => date >= p.StartDateUTC && date <= p.EndDateUTC && p.PlanID == 1, null, null, true);
                periodId = result.PeriodID;
            }

            var total = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            IRepository<Titles> titlesRepository = unitOfWork_Comm.GetRepository<Titles>();
            var titles = titlesRepository.GetAll().ToList();

            encoreMongo_Context.AccountsInformationProvider.DeleteMany(p => p.PeriodID == periodId);

            for (int i = 0; i < ii; i++)
            {
                var accountsInformation = accountInformationRepository.GetPagedList(p => p.PeriodID == periodId, null, null, i, 10000, true).Items;

                var result = from accountsInfo in accountsInformation
                             join titlesInfo_Career in titles on Int32.Parse(accountsInfo.CareerTitle) equals titlesInfo_Career.TitleID
                             join titlesInfo_Paid in titles on Int32.Parse(accountsInfo.PaidAsCurrentMonth) equals titlesInfo_Paid.TitleID
                             select new AccountsInformation_Mongo
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
                                 Region = accountsInfo.Region,
                                 NewStatus = accountsInfo.NewStatus,
                                 TimeLimitToBeDemote = accountsInfo.TimeLimitToBeDemote,
                                 CareerTitle = accountsInfo.CareerTitle,
                                 PaidAsCurrentMonth = accountsInfo.PaidAsCurrentMonth,
                                 PaidAsLastMonth = accountsInfo.PaidAsLastMonth,
                                 VolumeForCareerTitle = accountsInfo.VolumeForCareerTitle,
                                 Activity = accountsInfo.Activity,
                                 NineMonthsPQV = accountsInfo.NineMonthsPQV,
                                 PQV = accountsInfo.PQV,
                                 PCV = accountsInfo.PCV,
                                 GQV = accountsInfo.GQV,
                                 GCV = accountsInfo.GCV,
                                 DQVT = accountsInfo.DQVT,
                                 DCV = accountsInfo.DCV,
                                 DQV = accountsInfo.DQV,
                                 JoinDate = accountsInfo.JoinDate,
                                 Generation = accountsInfo.Generation,
                                 LEVEL = accountsInfo.LEVEL,
                                 SortPath = accountsInfo.SortPath,
                                 LeftBower = accountsInfo.LeftBower,
                                 RightBower = accountsInfo.RightBower,
                                 RequirementNewGeneration = accountsInfo.RequirementNewGeneration,
                                 TimeLimitForNewGeneration = accountsInfo.TimeLimitForNewGeneration,
                                 Title1Legs = accountsInfo.Title1Legs,
                                 Title2Legs = accountsInfo.Title2Legs,
                                 Title3Legs = accountsInfo.Title3Legs,
                                 Title4Legs = accountsInfo.Title4Legs,
                                 Title5Legs = accountsInfo.Title5Legs,
                                 Title6Legs = accountsInfo.Title6Legs,
                                 Title7Legs = accountsInfo.Title7Legs,
                                 Title8Legs = accountsInfo.Title8Legs,
                                 Title9Legs = accountsInfo.Title9Legs,
                                 Title10Legs = accountsInfo.Title10Legs,
                                 Title11Legs = accountsInfo.Title11Legs,
                                 Title12Legs = accountsInfo.Title12Legs,
                                 Title13Legs = accountsInfo.Title13Legs,
                                 Title14Legs = accountsInfo.Title14Legs,
                                 EmailAddress = accountsInfo.EmailAddress,
                                 CQL = accountsInfo.CQL,
                                 LastOrderDate = accountsInfo.LastOrderDate,
                                 IsCommissionQualified = accountsInfo.IsCommissionQualified,
                                 BirthdayUTC = accountsInfo.BirthdayUTC,
                                 UplineLeaderM3 = accountsInfo.UplineLeaderM3,
                                 UplineLeaderM3Name = accountsInfo.UplineLeaderM3Name,
                                 UplineLeaderL1 = accountsInfo.UplineLeaderL1,
                                 UplineLeaderL1Name = accountsInfo.UplineLeaderL1Name,
                                 TotalDownline = accountsInfo.TotalDownline,
                                 CreditAvailable = accountsInfo.CreditAvailable,
                                 DebtsToExpire = accountsInfo.DebtsToExpire,
                                 ExpiredDebts = accountsInfo.ExpiredDebts,
                                 GenerationM3 = accountsInfo.GenerationM3,
                                 ActiveDownline = accountsInfo.ActiveDownline,
                                 TitleMaintainance = accountsInfo.TitleMaintainance,
                                 SalesAverage = accountsInfo.SalesAverage,
                                 NewQualification = accountsInfo.NewQualification,
                                 NewEnrollments = accountsInfo.NewEnrollments,
                                 NineMonthsGQV = accountsInfo.NineMonthsGQV,
                                 NineMonthsDQV = accountsInfo.NineMonthsDQV,
                                 ConsultActive = accountsInfo.ConsultActive,

                                 CareerTitle_Des = titlesInfo_Career.Name,
                                 PaidAsCurrentMonth_Des = titlesInfo_Paid.Name
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
            var accountLogged = encoreMongo_Context.AccountsInformationProvider.Find(q => q.AccountID == filtrosDTO.CodConsultoraLogged && q.PeriodID == filtrosDTO.Period, null).FirstOrDefault();
            var sponsorSearch = encoreMongo_Context.AccountsInformationProvider.Find(q => q.AccountID == filtrosDTO.CodConsultoraSearched && q.PeriodID == filtrosDTO.Period, null).FirstOrDefault();
            sponsorSearch = sponsorSearch ?? new AccountsInformation_Mongo();
            if (accountLogged != null)
            {
                try
                {
                    var elements = encoreMongo_Context.AccountsInformationProvider.Find(c => c.PeriodID == filtrosDTO.Period && c.LeftBower >= accountLogged.LeftBower && c.RightBower <= accountLogged.RightBower).ToList();
                    var sponsorFilter = from reportAccountLogged in elements
                                        join reportAccountConsulted in encoreMongo_Context.AccountsInformationProvider.AsQueryable()
                                        on reportAccountLogged.AccountsInformationID equals reportAccountConsulted.AccountsInformationID into joined
                                        from consulted in joined.DefaultIfEmpty()
                                        where (consulted != null) &&
                                              (reportAccountLogged.PeriodID == filtrosDTO.Period) && 
                                              (consulted.LeftBower >= sponsorSearch.LeftBower || sponsorSearch.LeftBower == null) && 
                                              (consulted.RightBower <= sponsorSearch.RightBower || sponsorSearch.RightBower == null) &&
                                              (consulted.AccountName.ToUpper().Contains(filtrosDTO.NombreConsultora != null ? filtrosDTO.NombreConsultora.ToUpper() : ""))&&
                                              (consulted.SponsorID == filtrosDTO.CodigoPatrocinador || filtrosDTO.CodigoPatrocinador == 0) &&
                                              (consulted.SponsorName.ToUpper().Contains(filtrosDTO.NombrePatrocinador != null ? filtrosDTO.NombrePatrocinador.ToUpper() : ""))
                                        select consulted;


                    var detailWTA = sponsorFilter.ToList();
                    var cantidadRegistros = detailWTA.Count;

                    var devolverData = detailWTA.Skip((filtrosDTO.NumeroPagina >= cantidadRegistros) ? 0: filtrosDTO.NumeroPagina).Take(filtrosDTO.NumeroRegistros).ToList();

                    var totalPages = (filtrosDTO.NumeroRegistros >= cantidadRegistros) ?  1: cantidadRegistros / (filtrosDTO.NumeroRegistros);

                    return new AccountsExtended { numPage = totalPages, accountsInformationDTO = devolverData };
                }
                catch (Exception ex)
                {

                    return new AccountsExtended
                    {
                        numPage = 0,
                        accountsInformationDTO = new List<AccountsInformation_Mongo>{
                        new AccountsInformation_Mongo
                        {
                            AccountName=ex.Message
                        }
                    }
                    };
                }



            }

            return null;
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


        public AccountsInformationExtended GetPerformance_HeaderFront(int accountId, int periodId)
        {
            try
            {
                var header = encoreMongo_Context.AccountsProvider.Find(q => q.AccountID == accountId, null).ToList();

                if (header != null)
                {
                    IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();
                    var datetimeNow = DateTime.Now;
                    var period = periodsRepository.GetFirstOrDefault(p => datetimeNow >= p.StartDateUTC && datetimeNow <= p.EndDateUTC && p.PlanID == 1, null, null, true);

                    var headerByAccountInformation = from headerInitial in header
                                                     join reportAccountInitial in encoreMongo_Context.AccountsInformationProvider.Find(c => c.AccountID == accountId && c.PeriodID == periodId).ToList() on headerInitial.AccountID equals reportAccountInitial.AccountID
                                                     select new AccountsInformationExtended
                                                     {
                                                         LeftBower = reportAccountInitial.LeftBower,
                                                         RigthBower = reportAccountInitial.RightBower,
                                                         accounts_Mongo = headerInitial,
                                                         CareerTitle = reportAccountInitial.CareerTitle,
                                                         CareerTitle_Des = reportAccountInitial.CareerTitle_Des,
                                                         periodStartDateUTC = period == null ? null : period.StartDateUTC,
                                                         periodEndDateUTC = period == null ? null : period.EndDateUTC,
                                                         periodDescription = period == null ? "" : period.Description,
                                                         cantFinalPeriodo = Math.Round((period.EndDateUTC - DateTime.Now).Value.TotalDays) == 0 ? "HOY" : Math.Round((period.EndDateUTC - DateTime.Now).Value.TotalDays) > 5 ? period.EndDateUTC.Value.ToString("dd/MM/yyyy") : Math.Round((period.EndDateUTC - DateTime.Now).Value.TotalDays).ToString() + " d�as"
                                                     };

                    return headerByAccountInformation.FirstOrDefault();
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception { };
            }
        }

        //public AccountsInformationExtended GetPerformance_HeaderFront(int accountId,int periodId)
        //      {
        //	try
        //	{
        //		var header = encoreMongo_Context.AccountsProvider.Find(q => q.AccountID == accountId, null).ToList();



        //		var headerByAccountInformation = from headerInitial in header
        //										 join reportAccountInitial in encoreMongo_Context.AccountsInformationProvider.Find(c=>c.AccountID==accountId && c.PeriodID== periodId).ToList() on headerInitial.AccountID equals reportAccountInitial.AccountID
        //										 select new AccountsInformationExtended { LeftBower=reportAccountInitial.LeftBower, RigthBower=reportAccountInitial.RightBower , accounts_Mongo= headerInitial };


        //		//var headerByAccountInformation = encoreMongo_Context.AccountsInformationProvider.Find(c => c.AccountID == accountId && c.PeriodID == period).ToList();


        //		return headerByAccountInformation.FirstOrDefault();

        //          }
        //          catch (Exception ex)
        //          {
        //              throw new Exception { };
        //          }
        //      }

    }
}
