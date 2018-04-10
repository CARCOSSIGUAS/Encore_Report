using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Application.Interfaces
{
    public interface IProcessOnlineMlmService
    {
        void ProcessMLM_Order(int orderId);
        void ProcessMLM_Lote(int loteId);
    }
}
