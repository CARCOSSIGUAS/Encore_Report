using Belcorp.Encore.Application.ViewModel;
using Belcorp.Encore.Entities;
using Belcorp.Encore.Entities.Entities;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Services.Report.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;

namespace Belcorp.Encore.Application.Services
{
    public interface IAccountInformationService
    {
        Task<IEnumerable<ReportPerformance_HeaderModel>> GetPerformance_Header(int accountId, int periodId);

        Task<IEnumerable<ReportPerformance_DetailModel>> GetPerformance_Detail(int accountId, int periodId);

        AccountsInformationExtended GetPerformance_HeaderFront(int accountId, int periodId);

        Task<AccountsInformation_Mongo> GetPerformance_AccountInformation(int accountId, int periodId);

        AccountsExtended GetAccounts(Filtros_DTO filtrosDTO);

        PagedList<AccountsInformation_Mongo> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter);
        Task<IEnumerable<Options_DTO>> GetReportAccountsPeriods();
    }

}
