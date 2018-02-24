using System.Collections.Generic;

namespace Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders
{
    public class LimitOrderWithTrades
    {
        public LimitOrder Order { get; set; }

        public List<LimitTradeInfo> Trades { get; set; }
    }
}
