using Lykke.Service.TradesAdapter.Core.Settings;

namespace Lykke.Service.TradesAdapter.Settings.ServiceSettings
{
    public class TradesAdapterSettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings LimitOrderTradesSettings { set; get; }
        public int CacheSize { set; get; }
    }
}
