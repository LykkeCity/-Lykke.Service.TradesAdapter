using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Job.RabbitPublishers
{
    [UsedImplicitly]
    public class TradesPublisher : ITradesPublisher, IStartable, IStopable
    {
        private readonly ILog _log;
        private readonly IConsole _console;
        private readonly string _connectionString;
        private RabbitMqPublisher<List<Trade>> _publisher;
        
        public TradesPublisher(
            ILog log,
            IConsole console,
            string connectionString)
        {
            _log = log;
            _console = console;
            _connectionString = connectionString;
        }
        
        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForPublisher(_connectionString, "tradesadapter")
                .MakeDurable();
            
            _publisher = new RabbitMqPublisher<List<Trade>>(settings)
                .SetSerializer(new MessagePackMessageSerializer<List<Trade>>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .PublishSynchronously()
                .SetLogger(_log)
                .SetConsole(_console)
                .Start();
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public Task PublishAsync(List<Trade> trades)
        {
            return _publisher.ProduceAsync(trades);
        }
    }
}
