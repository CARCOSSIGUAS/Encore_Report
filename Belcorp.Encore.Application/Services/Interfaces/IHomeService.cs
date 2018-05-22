using Belcorp.Encore.Entities.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Belcorp.Encore.Application.Services.Interfaces
{
    public interface IHomeService
    {
        Task<AccountHomeHeader_DTO> GetHeader(int accountId);
        Task<PerformanceIndicator_DTO> GetPerformanceIndicator(int accountId);
    }
}
