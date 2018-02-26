using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.TradesAdapter.Core;
using Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Job.RabbitSubscribers
{
    [UsedImplicitly]
    public class LimitOrdersSubscriberForPublishing : IStartable
    {
        private readonly ILog _log;
        private readonly ITradesConverter _tradesConverter;
        private readonly ITradesPublisher _tradesPublisher;
        private readonly IRabbitSubscriber _rabbitMqSubscribe;
        private readonly string _connectionString;
        private readonly string _exchangeName;

        public LimitOrdersSubscriberForPublishing(
            ILog log,
            ITradesConverter tradesConverter,
            ITradesPublisher tradesPublisher,
            IRabbitSubscriber rabbitMqSubscribe,
            string connectionString,
            string exchangeName)
        {
            _log = log;
            _tradesConverter = tradesConverter;
            _tradesPublisher = tradesPublisher;
            _rabbitMqSubscribe = rabbitMqSubscribe;
            _connectionString = connectionString;
            _exchangeName = exchangeName;
        }
        
        public void Start()
        {
            _rabbitMqSubscribe.Subscribe(
                connectionString: _connectionString,
                exchangeName: _exchangeName,
                purpose: SubscriptionPurpose.Publisher,
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

                await _tradesPublisher.PublishAsync(trades);
                
                if (DateTime.UtcNow.Subtract(start) > TimeSpan.FromSeconds(10))
                    await _log.WriteWarningAsync(
                        nameof(LimitOrdersSubscriberForPublishing),
                        nameof(ProcessMessageAsync),
                        $"Long processing: {arg.ToJson()}");
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(LimitOrdersSubscriberForPublishing), nameof(ProcessMessageAsync), ex);
                throw;
            }
        }
    }
}
