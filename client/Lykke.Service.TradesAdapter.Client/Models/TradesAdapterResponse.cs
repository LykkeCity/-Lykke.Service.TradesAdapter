using System.Collections.Generic;
using Lykke.Service.TradesAdapter.AutorestClient.Models;
using Lykke.Service.TradesAdapter.Client.Models;

namespace Lykke.Service.TradesAdapter.Client.Models
{
    public class TradesAdapterResponse
    {
        public ErrorModel Error { get; set; }
        public IList<Trade> Records { get; set; }
    }
}
