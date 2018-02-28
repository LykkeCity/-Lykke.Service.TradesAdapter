using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Client.Models;

namespace Lykke.Service.TradesAdapter.Client
{
    public interface ITradesAdapterClient
    {
        Task<TradesAdapterResponse> GetTradesByAssetPairIdAsync(string assetPairId, int skip, int take);
    }
}
