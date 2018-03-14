﻿using Belcorp.Encore.Application.Interfaces;
using Belcorp.Encore.Data.Contexts;
using Belcorp.Encore.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public class SponsorTreeService : ISponsorTreeService
    {
        private readonly IUnitOfWork<EncoreCommissions_Context> _unitOfWork;
        private readonly EncoreMongo_Context encoreMongo_Context;
        private readonly ISponsorTreeRepository _sponsorTreeRepository;

        public SponsorTreeService(IUnitOfWork<EncoreCommissions_Context> unitOfWork, ISponsorTreeRepository sponsorTreeRepository)
        {
            _unitOfWork = unitOfWork;
            encoreMongo_Context = new EncoreMongo_Context();
            _sponsorTreeRepository = sponsorTreeRepository;
        }
    }
}
