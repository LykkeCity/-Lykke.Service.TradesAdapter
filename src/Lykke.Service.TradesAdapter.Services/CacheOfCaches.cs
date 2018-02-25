using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.TradesAdapter.AzureRepository.Trades;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Services
{
    [UsedImplicitly]
    public class CacheOfCaches : ICacheOfCaches
    {
        private readonly ConcurrentDictionary<string, IOrderedCache> _cache;
        private readonly ITradesLogRepository _tradesLogRepository;
        private readonly int _cacheSize;

        public CacheOfCaches(int cacheSize, ITradesLogRepository tradesLogRepository)
        {
            _cache = new ConcurrentDictionary<string, IOrderedCache>();
            _tradesLogRepository = tradesLogRepository;
            _cacheSize = cacheSize;
        }

        public Task AddAsync(string key, Trade item)
        {
            if (!_cache.TryGetValue(key, out var cachedCollection))
            {
                cachedCollection = new OrderedCache(key, _cacheSize, _tradesLogRepository);
                _cache.TryAdd(key, cachedCollection);
            }

            return cachedCollection.AddAsync(item);
        }

        public Task<IEnumerable<Trade>> GetAsync(string key, int skip, int take)
        {
            if (!_cache.TryGetValue(key, out var cachedCollection))
            {
                cachedCollection = new OrderedCache(key, _cacheSize, _tradesLogRepository);
            }

            return cachedCollection.GetAsync(skip, take);
        }
    }
}
