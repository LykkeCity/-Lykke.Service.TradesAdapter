using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Services
{
    [UsedImplicitly]
    public class RabbitSubscriber : IRabbitSubscriber, IDisposable
    {
        private readonly ConcurrentBag<RabbitSubscription> _subscriptions = new ConcurrentBag<RabbitSubscription>();
        private const string APPLICATION_NAME = "tradesadapter";
        private readonly ILog _log;
        private readonly IConsole _consoleWriter;

        public RabbitSubscriber(ILog log, IConsole consoleWriter)
        {
            _log = log;
            _consoleWriter = consoleWriter;
        }
        
        public void Subscribe<TMessage>(string connectionString, string exchangeName, SubscriptionPurpose purpose,
            IMessageDeserializer<TMessage> deserializer, Func<TMessage, Task> handler)
        {
            if (_subscriptions.Any(x => x.Purpose == purpose))
                _log.WriteWarningAsync(
                    nameof(RabbitSubscriber),
                    nameof(Subscribe),
                    $"More than one subscriber will read for {purpose.ToString()}")
                    .GetAwaiter()
                    .GetResult();
            
            var ns = purpose.ToString().ToLower();
            
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(connectionString, exchangeName, $"{APPLICATION_NAME}.{ns}")
                .MakeDurable();
            settings.DeadLetterExchangeName = null;

            var rabbitMqSubscriber =
                new RabbitMqSubscriber<TMessage>(
                    settings,
                    new ResilientErrorHandlingStrategy(
                        _log,
                        settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                    .SetMessageDeserializer(deserializer)
                    .SetMessageReadStrategy(new MessageReadQueueStrategy())
                    .CreateDefaultBinding()
                    .Subscribe(handler)
                    .SetLogger(_log)
                    .SetConsole(_consoleWriter)
                    .Start();

            _subscriptions.Add(
                new RabbitSubscription {
                    Purpose = purpose,
                    Subscriber = rabbitMqSubscriber
                });
        }
        
        public void Dispose()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Subscriber.Stop();
            }
        }
    }

    public class RabbitSubscription
    {
        public SubscriptionPurpose Purpose { set; get; }
        public IStopable Subscriber { set; get; }
    }
}
