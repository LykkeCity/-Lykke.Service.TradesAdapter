using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.RabbitSubscribers
{
    [UsedImplicitly]
    public class LimitOrdersSubscriberForCache : IStartable
    {
        private readonly ILog _log;
        private readonly ITradesConverter _tradesConverter;
        private readonly ICacheOfCaches _cache;
        private readonly IRabbitSubscriber _rabbitMqSubscribe;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        
        public LimitOrdersSubscriberForCache(
            ILog log,
            ITradesConverter tradesConverter,
            ICacheOfCaches cache,
            IRabbitSubscriber rabbitMqSubscribe,
            string connectionString,
            string exchangeName)
        {
            _log = log;
            _cache = cache;
            _tradesConverter = tradesConverter;
            _rabbitMqSubscribe = rabbitMqSubscribe;
            _connectionString = connectionString;
            _exchangeName = exchangeName;
        }
        
        public void Start()
        {
            _rabbitMqSubscribe.Subscribe(
                connectionString: _connectionString,
                exchangeName: _exchangeName,
                purpose: SubscriptionPurpose.Cache,
                deserializer: new JsonMessageDeserializer<LimitOrders>(),
                handler: ProcessMessageAsync);
        }

        private async Task ProcessMessageAsync(LimitOrders arg)
        {
            try
            {
                var start = DateTime.UtcNow;

                var trades = await _tradesConverter.ConvertAsync(arg);
                
                if (trades == null || trades.Count == 0)
                {
                    return;
                }

                foreach (var trade in trades)
                {
                    await _cache.AddAsync(trade.AssetPairId, trade);
                }
                
                if (DateTime.UtcNow.Subtract(start) > TimeSpan.FromSeconds(10))
                    await _log.WriteWarningAsync(
                        nameof(LimitOrdersSubscriberForCache),
                        nameof(ProcessMessageAsync),
                        $"Long processing: {arg.ToJson()}");
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(LimitOrdersSubscriberForCache), nameof(ProcessMessageAsync), ex);
                throw;
            }
        }
    }
}
