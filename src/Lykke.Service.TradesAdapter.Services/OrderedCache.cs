using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.TradesAdapter.AzureRepository.Trades;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core.Services;
using Nito.AsyncEx;

namespace Lykke.Service.TradesAdapter.Services
{
    [UsedImplicitly]
    public class OrderedCache : IOrderedCache
    {
        private readonly AsyncReaderWriterLock _asyncReaderWriterLock;
        private readonly SemaphoreSlim _initLock;
        private readonly ITradesLogRepository _tradesLogRepository;
        private readonly IComparer<Trade> _eventsComparer;
        private List<Trade> _data;
        private readonly int _cacheSize;
        private bool _dbLoaded;
        private readonly string _key;

        public OrderedCache(string key, int cacheSize, ITradesLogRepository tradesLogRepository)
        {
            _tradesLogRepository = tradesLogRepository;
            _initLock = new SemaphoreSlim(1, 1);
            _asyncReaderWriterLock = new AsyncReaderWriterLock();
            _eventsComparer = new EventsComparer<Trade>();
            _data = new List<Trade>();
            _cacheSize = cacheSize;
            _dbLoaded = false;
            _key = key;
        }
        
        private async Task InitAsync()
        {
            await _initLock.WaitAsync();

            try
            {
                if (_dbLoaded)
                    return;

                var entities = await _tradesLogRepository.GetLatestAsync(_key, _cacheSize);

                _data = entities.Select(x => new Trade
                {
                    Id = x.Id,
                    Action = x.Action,
                    AssetPairId = x.AssetPairId,
                    DateTime = x.DateTime,
                    Price = x.Price,
                    Volume = x.Volume
                }).ToList();

                _dbLoaded = true;
            }
            finally
            {
                _initLock.Release();
            }
        }
        
        public async Task AddAsync(Trade e)
        {
            using (await _asyncReaderWriterLock.WriterLockAsync())
            {
                if (!_dbLoaded)
                    await InitAsync();
                
                if (_data.Any(x => x.Id == e.Id))
                    return;
                
                var i = _data.BinarySearch(e, _eventsComparer);

                var whereToInsert = i < 0 ? ~i : i;
                    
                _data.Insert(whereToInsert, e);

                while (_data.Count > _cacheSize)
                {
                    _data.RemoveAt(_data.Count - 1);
                }
            }
        }

        public async Task<IEnumerable<Trade>> GetAsync(int skip, int take)
        {
            using (await _asyncReaderWriterLock.ReaderLockAsync())
            {
                if (!_dbLoaded)
                    await InitAsync();
                
                return _data.Skip(skip).Take(take);
            }
        }
    }

    internal class EventsComparer<T> : IComparer<T> where T :  IEvent
    {
        public int Compare(T x, T y)
        {
            if (x.DateTime != y.DateTime) return -x.DateTime.CompareTo(y.DateTime);
            
            if (x.Index.HasValue && y.Index.HasValue)
            {
                return -x.Index.Value.CompareTo(y.Index.Value);
            }

            if (x.Index.HasValue)
            {
                return 1;
            }

            if (y.Index.HasValue)
            {
                return -1;
            }

            return 0;
        }
    }
}
