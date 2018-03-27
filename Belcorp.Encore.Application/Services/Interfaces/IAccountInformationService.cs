using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public interface IAccountInformationService
    {
        void Migrate_AccountInformationByPeriod();
    }
}
