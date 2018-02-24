using Lykke.Service.TradesAdapter.Core.Settings;

namespace Lykke.Service.TradesAdapter.Job.Settings.JobSettings
{
    public class TradesAdapterSettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings LimitOrderTradesSettings { set; get; }
        public RabbitMqSettings PublishQueueSetings { set; get; }
    }
}
