using System;

namespace Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders
{
    public class LimitOrder
    {
        public string Id { get; set; }

        public string ExternalId { get; set; }

        public string AssetPairId { get; set; }

        public string ClientId { get; set; }

        public double Volume { get; set; }

        public double Price { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime Registered { get; set; }

        public DateTime? LastMatchTime { get; set; }

        public double RemainingVolume { get; set; }

        public bool Straight { get; set; } = true;
    }
}
