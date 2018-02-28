using System;

namespace Lykke.Service.TradesAdapter.Contract
{
    public interface ITrade : IEvent
    {
        string AssetPairId { set; get; }
        double Volume { set; get; }
        double Price { set; get; }
        TradeAction Action { set; get; }
    }

    public enum TradeAction
    {
        Buy, Sell
    }
}
