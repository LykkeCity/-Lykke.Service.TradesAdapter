// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.TradesAdapter.AutorestClient.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Runtime;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines values for TradeAction.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TradeAction
    {
        [EnumMember(Value = "Buy")]
        Buy,
        [EnumMember(Value = "Sell")]
        Sell
    }
    internal static class TradeActionEnumExtension
    {
        internal static string ToSerializedValue(this TradeAction? value)
        {
            return value == null ? null : ((TradeAction)value).ToSerializedValue();
        }

        internal static string ToSerializedValue(this TradeAction value)
        {
            switch( value )
            {
                case TradeAction.Buy:
                    return "Buy";
                case TradeAction.Sell:
                    return "Sell";
            }
            return null;
        }

        internal static TradeAction? ParseTradeAction(this string value)
        {
            switch( value )
            {
                case "Buy":
                    return TradeAction.Buy;
                case "Sell":
                    return TradeAction.Sell;
            }
            return null;
        }
    }
}
