using Belcorp.Encore.Application.ViewModel;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Services.Report.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Services
{
    public interface IAccountInformationService
    {
        void Migrate_AccountInformationByPeriod();

		IEnumerable<ReportPerformance_HeaderModel> GetPerformance_Header(int accountId, int periodId);

        IEnumerable<ReportPerformance_DetailModel> GetPerformance_Detail(int accountId, int sponsorId, int periodId);


    }
}
