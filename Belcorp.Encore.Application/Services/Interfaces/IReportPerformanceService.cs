using Belcorp.Encore.Application.ViewModel;
using Belcorp.Encore.Entities.Entities.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IReportPerformanceService
    {
        Task<IEnumerable<ReportPerformance_DetailModel>> GetPerformance_Detail(int accountId, int periodId, string country);
        Task<AccountsInformation_Mongo> GetPerformanceByAccount(int accountId, int periodId, string country);
        Task<IEnumerable<AccountsInformation_Mongo>> GetPerformanceBySponsor(int sponsorID, int periodId, string country);

    }
}
