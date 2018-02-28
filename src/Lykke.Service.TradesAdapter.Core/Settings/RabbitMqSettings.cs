using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.TradesAdapter.Core.Settings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { set; get; }
        public string ExchangeName { set; get; }
    }
}
