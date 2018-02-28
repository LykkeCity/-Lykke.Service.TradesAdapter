using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Services
{
    [UsedImplicitly]
    public class TradesConverter : ITradesConverter
    {
        private readonly IAssetsServiceWrapper _assetsServiceWrapper;

        public TradesConverter(
            IAssetsServiceWrapper assetsServiceWrapper)
        {
            _assetsServiceWrapper = assetsServiceWrapper;
        }
        
        public async Task<List<Trade>> ConvertAsync(LimitOrders orders)
        {
            var result = new List<Trade>();
            
            if (orders?.Orders == null || orders.Orders.Count == 0)
                return result;

            var matchedWithMarketOrder = MatchedWithMarketOrder(orders);
            
            foreach (var order in orders.Orders)
            {
                if(order.Trades == null || order.Trades.Count == 0)
                    continue;

                foreach (var trade in order.Trades)
                {
                    if (result.Any(x => x.Id == trade.TradeId))
                        continue;

                    var assetPair = await _assetsServiceWrapper.TryGetAssetPairAsync(trade.Asset, trade.OppositeAsset);
                    var baseAsset = await _assetsServiceWrapper.TryGetAssetAsync(assetPair.BaseAssetId);

                    var volume =
                        assetPair != null
                            ? ( assetPair.BaseAssetId == trade.Asset
                                    ? trade.Volume
                                    : trade.OppositeVolume )
                            : 0;

                    TradeAction action;

                    if (matchedWithMarketOrder)
                    {
                        action = order.Order.Volume > 0 ? TradeAction.Sell : TradeAction.Buy;
                    }
                    else
                    {
                        var oppositeOrder = orders.Orders.FirstOrDefault(x => x.Order.Id == trade.OppositeOrderId);

                        var latestOrder = oppositeOrder.Order.CreatedAt <= order.Order.CreatedAt
                            ? order.Order
                            : oppositeOrder.Order;
                        
                        action = latestOrder.Volume > 0 ? TradeAction.Buy : TradeAction.Sell;
                    }
                    
                    result.Add(new Trade
                    {
                        Id = trade.TradeId,
                        DateTime = trade.Timestamp,
                        Price = trade.Price,
                        AssetPairId = assetPair?.Id,
                        Volume = volume.Normalize(baseAsset),
                        Action = action
                    });
                }
            }

            return result;
        }
        
        private static bool MatchedWithMarketOrder(LimitOrders orders)
        {
            var result = false;
            
            foreach (var order in orders.Orders)
            {
                if (order.Trades == null || order.Trades.Count == 0)
                    continue;

                if (orders.Orders.All(
                    x =>
                        x.Trades == null ||
                        x.Trades.Count == 0 ||
                        x.Trades.All(y => y.OppositeOrderId != order.Order.Id)))
                    result = true;
            }

            return result;
        }
    }

    public static class AmountNormalizer
    {
        public static double Normalize(this double amount, Asset asset)
        {
            int assetDisplayAccuracy;

            if (asset != null)
            {
                assetDisplayAccuracy = asset.DisplayAccuracy ?? asset.Accuracy;
            }
            else
            {
                assetDisplayAccuracy = 2;
            }
            
            return amount.TruncateDecimalPlaces(assetDisplayAccuracy);
        }
    }
}
