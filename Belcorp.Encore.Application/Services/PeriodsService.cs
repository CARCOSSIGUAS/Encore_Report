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
    }
}
