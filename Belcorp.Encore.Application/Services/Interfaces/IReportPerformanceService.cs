using Belcorp.Encore.Application.ViewModel;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Mongo.Extension;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IReportPerformanceService
    {
        Task<IEnumerable<ReportPerformance_DetailModel>> GetPerformance_Detail(int accountId, int periodId, string country);
        Task<AccountsInformationPerformance_Mongo> GetPerformanceByAccount(int accountId, int periodId, string country);
        Task<IEnumerable<AccountsInformation_Mongo>> GetPerformanceBySponsor(int sponsorID, int periodId, string country);
        Task<ReportAccountPerformance_DTO> GetRequirements(ReportAccountPerformance_DTO item, string country);

    }
}
