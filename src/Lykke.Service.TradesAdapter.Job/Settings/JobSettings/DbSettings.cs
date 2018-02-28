using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.TradesAdapter.Job.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
        [AzureTableCheck]
        public string DataConnString { get; set; }
    }
}
