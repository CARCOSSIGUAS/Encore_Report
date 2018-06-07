using System.Collections.Generic;
using System.Threading.Tasks;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;

namespace Belcorp.Encore.Application.Services
{
    public interface IReportAccountService
    {
        PagedList<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter);
        Task<IEnumerable<Options_DTO>> GetReportAccountsPeriods();
    }
}
