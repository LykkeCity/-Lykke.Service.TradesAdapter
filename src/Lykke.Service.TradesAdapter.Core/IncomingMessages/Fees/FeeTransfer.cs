using System;

namespace Lykke.Service.TradesAdapter.Core.IncomingMessages.Fees
{
    public class FeeTransfer
    {
        public string ExternalId { get; set; }

        public string FromClientId { get; set; }

        public string ToClientId { get; set; }

        public DateTime DateTime { get; set; }

        public double Volume { get; set; }

        public string Asset { get; set; }
    }
}
