using System.Linq;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Services
{
    [UsedImplicitly]
    public class AssetsServiceWrapperWithCache : IAssetsServiceWrapperWithCache
    {
        private readonly CachedDataDictionary<string, Asset> _assetsCache;
        private readonly CachedDataDictionary<string, AssetPair> _assetPairsCache;

        public AssetsServiceWrapperWithCache(
            CachedDataDictionary<string, Asset> assetsCache,
            CachedDataDictionary<string, AssetPair> assetPairsCache)
        {
            _assetsCache = assetsCache;
            _assetPairsCache = assetPairsCache;
        }

        public async Task<Asset> TryGetAssetAsync(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
                return null;
            
            var cachedValues = await _assetsCache.Values();

            return cachedValues.FirstOrDefault(x => x.Id == assetId);
        }

        public async Task<AssetPair> TryGetAssetPairAsync(string assetId1, string assetId2)
        {
            if (string.IsNullOrEmpty(assetId1) ||
                string.IsNullOrEmpty(assetId2))
                return null;

            var cachedValues = await _assetPairsCache.Values();

            return cachedValues
                .FirstOrDefault(
                    x =>
                        x.BaseAssetId == assetId1 && x.QuotingAssetId == assetId2 ||
                        x.BaseAssetId == assetId2 && x.QuotingAssetId == assetId1);
        }

        public async Task<AssetPair> TryGetAssetPairAsync(string assetPairId)
        {
            if (string.IsNullOrEmpty(assetPairId))
                return null;
            
            var cachedValues = await _assetPairsCache.Values();

            return cachedValues.FirstOrDefault(x => x.Id == assetPairId);
        }
    }
}
