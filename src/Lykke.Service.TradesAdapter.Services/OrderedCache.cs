using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core;
using Lykke.Service.TradesAdapter.Core.Services;
using Nito.AsyncEx;

namespace Lykke.Service.TradesAdapter.Services
{
    [UsedImplicitly]
    public class OrderedCache<T> : IOrderedCache<T> where T: IEvent
    {
        private readonly AsyncReaderWriterLock _asyncReaderWriterLock;
        private readonly IComparer<T> _eventsComparer;
        private readonly List<T> _data;
        private readonly int _cacheSize;

        public OrderedCache(int cacheSize)
        {
            _asyncReaderWriterLock = new AsyncReaderWriterLock();
            _eventsComparer = new EventsComparer<T>();
            _data = new List<T>();
            _cacheSize = cacheSize;
        }
        
        public async Task AddAsync(T e)
        {
            using (await _asyncReaderWriterLock.WriterLockAsync())
            {
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

        public async Task<IEnumerable<T>> GetAsync(int skip, int take)
        {
            using (await _asyncReaderWriterLock.ReaderLockAsync())
            {
                return _data.Skip(skip).Take(take);
            }
        }
    }

    internal class EventsComparer<T> : IComparer<T> where T :  IEvent
    {
        public int Compare(T x, T y)
        {
            return -x.DateTime.CompareTo(y.DateTime);
        }
    }
}
