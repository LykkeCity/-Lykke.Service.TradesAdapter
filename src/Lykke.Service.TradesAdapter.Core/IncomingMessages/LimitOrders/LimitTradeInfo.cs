﻿using System;

namespace Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders
{
    public class LimitTradeInfo
    {
        public string TradeId { get; set; }
        
        public string ClientId { get; set; }

        public string Asset { get; set; }

        public double Volume { get; set; }

        public double Price { get; set; }

        public DateTime Timestamp { get; set; }

        public string OppositeOrderId { get; set; }

        public string OppositeOrderExternalId { get; set; }

        public string OppositeAsset { get; set; }

        public string OppositeClientId { get; set; }

        public double OppositeVolume { get; set; }
        
        public int Index { get; set; }
    }
}
