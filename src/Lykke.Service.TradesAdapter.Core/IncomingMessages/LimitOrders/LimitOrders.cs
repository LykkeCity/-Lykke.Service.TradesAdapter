using System.Collections.Generic;

namespace Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders
{
    public class LimitOrders
    {
        public List<LimitOrderWithTrades> Orders { get; set; }
    }
}
