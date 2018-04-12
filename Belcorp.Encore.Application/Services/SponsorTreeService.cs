using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class SponsorTreeService : ISponsorTreeService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> unitOfWork;
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly ISponsorTreeRepository sponsorTreeRepository;

        public SponsorTreeService
        (
            IUnitOfWork<EncoreCommissions_Context> _unitOfWork, 
            ISponsorTreeRepository _sponsorTreeRepository, 
            IOptions<Settings> settings
        )
        {
            unitOfWork = _unitOfWork;
            encoreMongo_Context = new EncoreMongo_Context(settings);
            sponsorTreeRepository = _sponsorTreeRepository;
        }
    }
}
