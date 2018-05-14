using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Interfaces
{
    public interface IProcessOnlineMlmService
    {
        void ProcessMLMOrder(int orderId);
        void ProcessMLMLote(int loteId);
    }
}
