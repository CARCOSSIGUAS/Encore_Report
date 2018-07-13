using System.Collections.Generic;
using System.Threading.Tasks;
using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Mongo.Extension;
using Belcorp.Encore.Entities.Entities.Search;
using Belcorp.Encore.Entities.Entities.Search.Paging;

namespace Belcorp.Encore.Application.Services
{
    public interface IReportAccountService
    {
        PagedList<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoreds(ReportAccountsSponsoredsSearch filter, string country);
        List<AccountsInformation_MongoWithAccountAndSponsor> GetReportAccountsSponsoredsThree(ReportAccountsSponsoredsSearch filter, string country);
        IEnumerable<AccountsInformation_Mongo> GetReportAccountsBySponsored(int sponsor, int accountID, string country);

        Task<IEnumerable<Options_DTO>> GetReportAccountsPeriods(string country);
        AccountsInformation_MongoWithAccountAndSponsor GetConsultantDetails(int? periodId, int accountId, int accountIdCurrent, string country = null);
        IEnumerable<Dictionary<string, string>> GetStatesByPeriods(int periodID, string country);
    }
}
