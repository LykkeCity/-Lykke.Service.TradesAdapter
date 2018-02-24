using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core;
using Lykke.Service.TradesAdapter.Core.Services;

namespace Lykke.Service.TradesAdapter.Services
{
    [UsedImplicitly]
    public class CacheOfCaches : ICacheOfCaches<Trade>
    {
        private readonly ConcurrentDictionary<string, IOrderedCache<Trade>> _cache;
        private readonly int _cacheSize;

        public CacheOfCaches(int cacheSize)
        {
            _cache = new ConcurrentDictionary<string, IOrderedCache<Trade>>();
            _cacheSize = cacheSize;
        }

        public Task AddAsync(string key, Trade item)
        {
            if (!_cache.TryGetValue(key, out var cachedCollection))
            {
                cachedCollection = new OrderedCache<Trade>(_cacheSize);
                _cache.TryAdd(key, cachedCollection);
            }

            return cachedCollection.AddAsync(item);
        }

        public Task<IEnumerable<Trade>> GetAsync(string key, int skip, int take)
        {
            if (!_cache.TryGetValue(key, out var cachedCollection))
            {
                cachedCollection = new OrderedCache<Trade>(_cacheSize);
            }

            return cachedCollection.GetAsync(skip, take);
        }
    }
}
