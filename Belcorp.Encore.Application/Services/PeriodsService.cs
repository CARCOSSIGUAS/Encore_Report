using Belcorp.Encore.Application.Services.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Entities.Entities.Commissions;
using Belcorp.Encore.Entities.Entities.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class PeriodsService : IPeriodsService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork_Comm;
        private readonly EncoreMongo_Context encoreMongo_Context;

        public PeriodsService
        (
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork_Comm,
            IOptions<Settings> settings
        )
        {
            unitOfWork_Comm = _unitOfWork_Comm;
            encoreMongo_Context = new EncoreMongo_Context(settings);
        }

        public void Migrate_Periods()
        {
            IRepository<Periods> periodsRepository = unitOfWork_Comm.GetRepository<Periods>();
            encoreMongo_Context.PeriodsProvider.DeleteMany(new BsonDocument { });
            var total = periodsRepository.GetPagedList(null, null, null, 0, 10000, true);
            int ii = total.TotalPages;

            for (int i = 0; i < ii; i++)
            {
                var periods = periodsRepository.GetPagedList(null, null, null, i, 10000, true).Items;

                List<Periods_Mongo> periods_Mongo = new List<Periods_Mongo>();
                foreach (var period in periods)
                {
                    Periods_Mongo registro = new Periods_Mongo()
                    {
                        PeriodID = period.PeriodID,
                        CountryID = 0,

                        StartDate = period.StartDate,
                        EndDate = period.EndDate,
                        ClosedDate = period.ClosedDate,
                        PlanID = period.PlanID,
                        EarningsViewable = period.EarningsViewable,
                        BackOfficeDisplayStartDate = period.BackOfficeDisplayStartDate,
                        DisbursementsProcessed = period.DisbursementsProcessed,
                        Description = period.Description,
                        StartDateUTC = period.StartDateUTC,
                        EndDateUTC = period.EndDateUTC,
                        LockDate = period.LockDate
                    };

                    periods_Mongo.Add(registro);
                }

                encoreMongo_Context.PeriodsProvider.InsertMany(periods_Mongo);
            }
        }
    }
}
