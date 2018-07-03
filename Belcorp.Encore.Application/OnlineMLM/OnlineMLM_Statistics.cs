using Belcorp.Encore.Entities.Constants;
using Belcorp.Encore.Entities.Entities.Core;
using Belcorp.Encore.Repositories.Interfaces;

namespace Belcorp.Encore.Application.OnlineMLM
{
    public class OnlineMLM_Statistics
    {
        private readonly IProcessOnlineRepository processOnlineRepository;

        public OnlineMLM_Statistics(IProcessOnlineRepository _processOnlineRepository, Orders _order)
        {
            processOnlineRepository = _processOnlineRepository;
            Order = _order;
            QV = GetQV_ByOrder();
            CV = GetCV_ByOrder();
            RV = GetRV_ByOrder();
        }

        public Orders Order { get; set; }
        public int PeriodID { get; set; }

        public decimal? QV { get; set; }
        public decimal? CV { get; set; }
        public decimal? RV { get; set; }

        private decimal GetQV_ByOrder()
        {
            var value = processOnlineRepository.GetQV_ByOrder(Order.OrderID);
            if (Order.OrderTypeID == (short)Constants.OrderType.ReturnOrder)
            {
                return value > 0 ? value * -1 : value;
            }

            return value;
        }

        private decimal? GetCV_ByOrder()
        {
            var value = Order.CommissionableTotal == null ? 0 : Order.CommissionableTotal;
            if (Order.OrderTypeID == (short)Constants.OrderType.ReturnOrder)
            {
                return value > 0 ? value * -1 : value;
            }

            return value;
        }

        private decimal GetRV_ByOrder()
        {
            var value = Order.Subtotal == null ? 0 : (decimal)Order.Subtotal;
            if (Order.OrderTypeID == (short)Constants.OrderType.ReturnOrder)
            {
                return value > 0 ? value * -1 : value;
            }

            return value;
        }
    }
}
