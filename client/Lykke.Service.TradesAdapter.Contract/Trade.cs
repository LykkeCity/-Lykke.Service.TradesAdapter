using System;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.TradesAdapter.Contract
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class Trade : ITrade
    {
        public string Id { set; get; }
        public string AssetPairId { set; get; }
        public DateTime DateTime { set; get; }
        public double Volume { set; get; }
        public double Price { set; get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeAction Action { set; get; }
    }
}
