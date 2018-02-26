using System;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface IRabbitSubscriber
    {
        void Subscribe<TMessage>(string connectionString, string exchangeName, SubscriptionPurpose purpose,
            IMessageDeserializer<TMessage> deserializer, Func<TMessage, Task> handler);
    }

    public enum SubscriptionPurpose
    {
        Cache,
        DbLog,
        Publisher
    }
}
