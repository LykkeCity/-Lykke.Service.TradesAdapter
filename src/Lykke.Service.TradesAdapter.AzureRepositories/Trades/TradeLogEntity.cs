using System;
using System.Globalization;
using Lykke.Service.TradesAdapter.Contract;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.TradesAdapter.AzureRepository.Trades
{
    public class TradeLogEntity : TableEntity, ITrade
    {
        private static string GeneratePartitionKey(string assetPairId)
        {
            return assetPairId.ToLower();
        }
        
        private static string GenerateRowKey(DateTime dateTime, int? index)
        {
            var inverseTimeKey = DateTime
                .MaxValue
                .Subtract(dateTime)
                .TotalMilliseconds
                .ToString(CultureInfo.InvariantCulture);
            var secondaryOrder = index.HasValue
                ? string.Format("{0:D10}", int.MaxValue - index.Value)
                : Guid.NewGuid().ToString();
            return string.Format("{0}-{1}", inverseTimeKey, secondaryOrder);
        }

        public static TradeLogEntity Create(ITrade src)
        {
            var entity = CreateNew(src);
            entity.PartitionKey = GeneratePartitionKey(src.AssetPairId);
            entity.RowKey = GenerateRowKey(src.DateTime, src.Index);

            return entity;
        }
        
        private static TradeLogEntity CreateNew(string id, string assetPairId, DateTime dateTime, double volume,
            double price, TradeAction action, int? index)
        {
            return new TradeLogEntity
            {
                Id = id,
                AssetPairId = assetPairId,
                DateTime = dateTime,
                Volume = volume,
                Price = price,
                Action = action,
                Index = index
            };
        }

        private static TradeLogEntity CreateNew(ITrade src)
        {
            return CreateNew(src.Id, src.AssetPairId, src.DateTime, src.Volume, src.Price, src.Action, src.Index);
        }

        public string Id { get; set; }
        public string AssetPairId { get; set; }
        public DateTime DateTime { get; set; }
        public int? Index { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public TradeAction Action { get; set; }
    }
}
