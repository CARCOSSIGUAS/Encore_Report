using Belcorp.Encore.Entities.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IHomeService
    {
        Task<AccountHomeHeader_DTO> GetHeader(int accountId, string country);
        Task<PerformanceIndicator_DTO> GetPerformanceIndicator(int accountId, string country);
        KpisIndicator_DTO GetKpisIndicator(int periodID, int SponsorID, int DownLineID, string country);
        BonusIndicator_DTO GetBonusIndicator(int periodID, int SponsorID, int DownlineID, string country);
    }
}
