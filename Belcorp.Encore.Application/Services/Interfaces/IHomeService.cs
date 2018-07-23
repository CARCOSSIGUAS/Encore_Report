using Belcorp.Encore.Entities.Entities.DTO;
using Belcorp.Encore.Entities.Entities.Mongo;
using Belcorp.Encore.Entities.Entities.Mongo.Extension;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IHomeService
    {
        Task<AccountHomeHeader_DTO> GetHeader(int accountId, string country);
        Task<PerformanceIndicator_DTO> GetPerformanceIndicator(int accountId, int? periodID, string country);
        Task<NewsIndicator_DTO> GetNewsIndicator(int accountID, int periodoID, string country);
        Task<BonusIndicator_DTO> GetBonusIndicator(int SponsorID, string country);
        Task<KpisIndicator_DTO> GetKpisIndicator(int periodID, int SponsorID, int DownLineID, string country);

        List<AccountsInformation_Mongo> GetConsultantSearch(string filter, int accountId, string country);
        List<AccountsInformation_MongoWithAccountAndSponsor> GetConsultantLowerPerformance(int? periodID, int accountID, string country);
        Periods_Mongo GetCurrentPeriod(string country);
        string GetLastTransaction(string country);
    }
}

