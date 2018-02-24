using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.TradesAdapter.Core.Settings
{
    public class AssetsServiceClientSettings
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
