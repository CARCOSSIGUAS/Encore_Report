using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Application.ViewModel;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Mongo.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services
{
    public class ReportPerformanceService : IReportPerformanceService
    {
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly IHomeService homeService;

        public ReportPerformanceService
        (
            IConfiguration configuration,
            IHomeService _homeService
        )
        {
            homeService = _homeService;
            encoreMongo_Context = new EncoreMongo_Context(configuration);
        }


        public async Task<AccountsInformationPerformance_Mongo> GetPerformanceByAccount(int accountId, int periodId, string country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountsCollection = encoreMongo_Context.AccountsProvider(country);
            var filterDefinition = Builders<AccountsInformation_Mongo>.Filter.Empty;

            periodId = periodId == 0 ? homeService.GetCurrentPeriod(country).PeriodID : periodId;

            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.PeriodID, periodId);
            filterDefinition &= Builders<AccountsInformation_Mongo>.Filter.Eq(ai => ai.AccountID, accountId);



            var result = await accountInformationCollection
                .Aggregate()
                .Match(filterDefinition)
                .Lookup<AccountsInformation_Mongo, Accounts_Mongo, AccountsInformationPerformance_Mongo>(
                    accountsCollection,
                    ai => ai.AccountID,
                    a => a.AccountID,
                    r => r.Account
                )
                .Unwind(a => a.Account, new AggregateUnwindOptions<AccountsInformationPerformance_Mongo> { PreserveNullAndEmptyArrays = true }).FirstOrDefaultAsync();


            //var result = await accountInformationCollection.Find(p => p.AccountID == accountId && p.PeriodID == periodId).FirstOrDefaultAsync();

            return result;
        }


        public async Task<ReportAccountPerformance_DTO> GetRequirements(ReportAccountPerformance_DTO item, string country)
        {
            IMongoCollection<RequirementLegs_Mongo> requirementLegsCollection = encoreMongo_Context.RequirementLegsProvider(country);
            IMongoCollection<RequirementTitleCalculations_Mongo> requirementTitleCalculationsCollection = encoreMongo_Context.RequirementTitleCalculationsProvider(country);
            List<RequirementLegs_DTO> items = new List<RequirementLegs_DTO>();
            List<RequirementLegs_DTO> itemsRN = new List<RequirementLegs_DTO>();


            var title = int.Parse(string.IsNullOrEmpty(item.CareerTitle) ? "0" : item.CareerTitle);
            var requirementTitleCalculations = await requirementTitleCalculationsCollection.Find(p => p.TitleID == title).ToListAsync();
            var requirementTitleCalculationsNext = await requirementTitleCalculationsCollection.Find(p => p.TitleID == title + 1).ToListAsync();
           
            var requirementLegs = await requirementLegsCollection.Find(p => p.TitleID == title).ToListAsync();
            var requirementLegsNext = await requirementLegsCollection.Find(p => p.TitleID == title + 1).ToListAsync();

            item.PQVRequirement = ValidateNullValueRequirementTitleCalculations(requirementTitleCalculations.FirstOrDefault(x => x.CalculationtypeID == 1));
            item.DQVRequirement = ValidateNullValueRequirementTitleCalculations(requirementTitleCalculations.FirstOrDefault(x => x.CalculationtypeID == 6));
            item.CQLRequirement = ValidateNullValueRequirementTitleCalculations(requirementTitleCalculations.FirstOrDefault(x => x.CalculationtypeID == 8));
            item.PQVRequirementNext = ValidateNullValueRequirementTitleCalculations(requirementTitleCalculationsNext.FirstOrDefault(x => x.CalculationtypeID == 1));
            item.DQVRequirementNext = ValidateNullValueRequirementTitleCalculations(requirementTitleCalculationsNext.FirstOrDefault(x => x.CalculationtypeID == 6));
            item.CQLRequirementNext = ValidateNullValueRequirementTitleCalculations(requirementTitleCalculationsNext.FirstOrDefault(x => x.CalculationtypeID == 8));


            requirementLegs.ForEach(x=> {
                items.Add(GetRequirementLegs(x, item ));
            });

            requirementLegsNext.ForEach(x => {
                itemsRN.Add(GetRequirementLegs(x, item));
            });

            item.RequirementLegs = items;
            item.RequirementLegsNext = itemsRN;

            return item;
        }

        public RequirementLegs_DTO GetRequirementLegs(RequirementLegs_Mongo item, ReportAccountPerformance_DTO reportAccountPerformance)
        {
            RequirementLegs_DTO retorno;
            decimal titleQtyDiff = 0;

            switch (item.TitleRequired)
            {
                case 1: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title1Legs;break;
                case 2: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title2Legs; break;
                case 3: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title3Legs; break;
                case 4: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title4Legs; break;
                case 5: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title5Legs; break;
                case 6: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title6Legs; break;
                case 7: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title7Legs; break;
                case 8: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title8Legs; break;
                case 9: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title9Legs; break;
                case 10: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title10Legs; break;
                case 11: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title11Legs; break;
                case 12: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title12Legs; break;
                case 13: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title13Legs; break;
                case 14: titleQtyDiff = item.TitleQty - (decimal)reportAccountPerformance.Title14Legs; break;
            }

            retorno = new RequirementLegs_DTO
            {
                TitleID = item.TitleID,
                PlanID = item.PlanID,
                TitleRequired = item.TitleRequired,
                TitleDescription = item.TitleDescription,
                Generation = item.Generation,
                Level = item.Level,
                TitleQty = item.TitleQty,
                TitleQtyDiff = titleQtyDiff
            };

            return retorno;
        }


        public decimal ValidateNullValueRequirementTitleCalculations(RequirementTitleCalculations_Mongo item)
        {
            decimal retorno = 0;
            if (item!=null)
                retorno = item.MinValue;
            return retorno;
        }


        public async Task<IEnumerable<AccountsInformation_Mongo>> GetPerformanceBySponsor(int sponsorID, int periodId, string country)
        {
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);

            periodId = periodId == 0 ? homeService.GetCurrentPeriod(country).PeriodID : periodId;

            var sponsor = await accountInformationCollection.Find(p => p.AccountID == sponsorID && p.PeriodID == periodId).FirstOrDefaultAsync();
            if (sponsor!=null)
            {
                sponsor.DQVT = sponsor.PQV;
            }

            var result = await accountInformationCollection.Find(p => p.SponsorID == sponsorID && p.PeriodID == periodId && p.DQVT > 0).ToListAsync();

            if (result != null)
                result.Insert(0, sponsor);

            return result;
        }



        public async Task<IEnumerable<ReportPerformance_DetailModel>> GetPerformance_Detail(int accountId, int periodId, string country)
        {

            List<ReportPerformance_DetailModel> reportPerformanceDetailModel = new List<ReportPerformance_DetailModel>();
            IMongoCollection<AccountsInformation_Mongo> accountInformationCollection = encoreMongo_Context.AccountsInformationProvider(country);
            IMongoCollection<Accounts_Mongo> accountCollection = encoreMongo_Context.AccountsProvider(country);

            var detailWA = (from ai in accountInformationCollection.AsQueryable()
                            join
                            a in accountCollection.AsQueryable() on
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

    }
}
