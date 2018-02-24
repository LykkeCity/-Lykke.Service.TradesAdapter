using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core;
using Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Services
{
    [UsedImplicitly]
    public class TradesConverter : ITradesConverter
    {
        private readonly IAssetsServiceWrapperWithCache _assetsServiceWrapperWithCache;

        public TradesConverter(
            IAssetsServiceWrapperWithCache assetsServiceWrapperWithCache)
        {
            _assetsServiceWrapperWithCache = assetsServiceWrapperWithCache;
        }
        
        public async Task<List<Trade>> ConvertAsync(LimitOrders orders)
        {
            var result = new List<Trade>();
            
            if (orders?.Orders == null || orders.Orders.Count == 0)
                return result;
            
            foreach (var order in orders.Orders)
            {
                if(order.Trades == null || order.Trades.Count == 0)
                    continue;

                foreach (var trade in order.Trades)
                {
                    if (result.Any(x => x.Id == trade.TradeId))
                        continue;

                    var assetPair = await _assetsServiceWrapperWithCache.TryGetAssetPairAsync(trade.Asset, trade.OppositeAsset);

                    var volume =
                        assetPair != null
                            ? ( assetPair.BaseAssetId == trade.Asset
                                    ? trade.Volume
                                    : trade.OppositeVolume )
                            : 0;

                    TradeAction action;

                    if (MatchedWithMarketOrder(orders))
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
                        Volume = volume,
                        Action = action
                    });
                }
            }

            return result;
        }

        private static bool MatchedWithMarketOrder(LimitOrders order)
        {
            var result = false;
            
            foreach (var subOrder in order.Orders)
            {
                if (subOrder.Trades == null || subOrder.Trades.Count == 0)
                    continue;

                if (order.Orders.All(x => x.Trades.All(y => y.OppositeOrderId != subOrder.Order.Id)))
                    result = true;
            }

            return result;
        }
    }
}
