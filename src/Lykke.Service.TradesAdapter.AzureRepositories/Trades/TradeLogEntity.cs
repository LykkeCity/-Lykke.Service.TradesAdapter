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
        
        private static string GenerateRowKey(DateTime dateTime)
        {
            var inverseTimeKey = DateTime
                .MaxValue
                .Subtract(dateTime)
                .TotalMilliseconds
                .ToString(CultureInfo.InvariantCulture);
            return string.Format("{0}-{1}", inverseTimeKey, Guid.NewGuid());
        }

        public static TradeLogEntity Create(ITrade src)
        {
            var entity = CreateNew(src);
            entity.PartitionKey = GeneratePartitionKey(src.AssetPairId);
            entity.RowKey = GenerateRowKey(src.DateTime);

            return entity;
        }
        
        private static TradeLogEntity CreateNew(string id, string assetPairId, DateTime dateTime, double volume,
            double price, TradeAction action)
        {
            return new TradeLogEntity
            {
                Id = id,
                AssetPairId = assetPairId,
                DateTime = dateTime,
                Volume = volume,
                Price = price,
                Action = action
            };
        }

        private static TradeLogEntity CreateNew(ITrade src)
        {
            return CreateNew(src.Id, src.AssetPairId, src.DateTime, src.Volume, src.Price, src.Action);
        }

        public string Id { get; set; }
        public string AssetPairId { get; set; }
        public DateTime DateTime { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public TradeAction Action { get; set; }
    }
}
