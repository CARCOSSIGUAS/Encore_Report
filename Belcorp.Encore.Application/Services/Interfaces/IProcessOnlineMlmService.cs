using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Interfaces
{
    public interface IProcessOnlineMlmService
    {
        void ProcessMLMOrder(int orderId, string country);
        void ProcessMLMLote(int loteId, string country);
    }
}
