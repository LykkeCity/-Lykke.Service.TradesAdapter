using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.TradesAdapter.Client 
{
    public class TradesAdapterServiceClientSettings 
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
