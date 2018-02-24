using Lykke.Service.TradesAdapter.Core.Settings;
using Lykke.Service.TradesAdapter.Settings.ServiceSettings;
using Lykke.Service.TradesAdapter.Settings.SlackNotifications;

namespace Lykke.Service.TradesAdapter.Settings
{
    public class AppSettings
    {
        public TradesAdapterSettings TradesAdapterService { get; set; }
        public AssetsServiceClientSettings AssetsServiceClient { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
