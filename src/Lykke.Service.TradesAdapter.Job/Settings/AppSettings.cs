using Lykke.Service.TradesAdapter.Core.Settings;
using Lykke.Service.TradesAdapter.Job.Settings.JobSettings;
using Lykke.Service.TradesAdapter.Job.Settings.SlackNotifications;

namespace Lykke.Service.TradesAdapter.Job.Settings
{
    public class AppSettings
    {
        public TradesAdapterSettings TradesAdapterJob { get; set; }
        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
