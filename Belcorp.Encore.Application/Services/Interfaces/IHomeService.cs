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
        KpiIndicatorPivot_DTO GetkpisIndicator(int periodID, int SponsorID, int DownLineID, string country = null);
        BonusDetails_DTO GetBonusIndicator(int SponsorID, int periodID, string country = null);
    }
}
