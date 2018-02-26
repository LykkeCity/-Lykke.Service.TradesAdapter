using System.Threading.Tasks;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface IAssetsServiceWrapper
    {
        Task<Asset> TryGetAssetAsync(string assetId);
        Task<AssetPair> TryGetAssetPairAsync(string assetId1, string assetId2);
        Task<AssetPair> TryGetAssetPairAsync(string assetPairId);
    }
}
