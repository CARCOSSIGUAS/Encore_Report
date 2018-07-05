using System.Collections.Generic;
using System.Threading.Tasks;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;

namespace Belcorp.Encore.Application.Services
{
    public interface IReportAccountService
    {
        PagedList<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter, string country);
        PagedList<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoredsThree(ReportAccountsSponsoredsSearch filter, string country);

        Task<IEnumerable<Options_DTO>> GetReportAccountsPeriods(string country);
        AccountsInformation_MongoWithAccountAndSponsor GetConsultantDetails(int? periodId, int accountId, string country = null);
    }
}
