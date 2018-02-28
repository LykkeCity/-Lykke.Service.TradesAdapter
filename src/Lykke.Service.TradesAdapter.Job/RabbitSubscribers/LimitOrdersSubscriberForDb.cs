using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.TradesAdapter.AzureRepository.Trades;
using Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Job.RabbitSubscribers
{
    [UsedImplicitly]
    public class LimitOrdersSubscriberForDb : IStartable
    {
        private readonly ILog _log;
        private readonly ITradesConverter _tradesConverter;
        private readonly ITradesLogRepository _tradesLogRepository;
        private readonly IRabbitSubscriber _rabbitMqSubscribe;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        
        public LimitOrdersSubscriberForDb(
            ILog log,
            ITradesConverter tradesConverter,
            ITradesLogRepository tradesLogRepository,
            IRabbitSubscriber rabbitMqSubscribe,
            string connectionString,
            string exchangeName)
        {
            _log = log;
            _tradesConverter = tradesConverter;
            _tradesLogRepository = tradesLogRepository;
            _rabbitMqSubscribe = rabbitMqSubscribe;
            _connectionString = connectionString;
            _exchangeName = exchangeName;
        }
        
        public void Start()
        {
            _rabbitMqSubscribe.Subscribe(
                connectionString: _connectionString,
                exchangeName: _exchangeName,
                purpose: SubscriptionPurpose.DbLog,
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
                
                await _tradesLogRepository.AddIfMissingMultipleAsync(trades);
                
                if (DateTime.UtcNow.Subtract(start) > TimeSpan.FromSeconds(10))
                    await _log.WriteWarningAsync(
                        nameof(LimitOrdersSubscriberForDb),
                        nameof(ProcessMessageAsync),
                        $"Long processing: {arg.ToJson()}");
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(LimitOrdersSubscriberForDb), nameof(ProcessMessageAsync), ex);
                throw;
            }
        }
    }
}
