using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Interfaces
{
    public interface IProcessOnlineMlmService
    {
        void ProcessMLM(int orderId, int accountId, int periodId);
    }
}
