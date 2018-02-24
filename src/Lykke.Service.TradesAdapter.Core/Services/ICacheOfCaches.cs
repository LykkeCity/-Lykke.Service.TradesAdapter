using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Contract;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface ICacheOfCaches<T> where T : IEvent
    {
        Task AddAsync(string key, T item);
        Task<IEnumerable<T>> GetAsync(string key, int skip, int take);
    }
}
