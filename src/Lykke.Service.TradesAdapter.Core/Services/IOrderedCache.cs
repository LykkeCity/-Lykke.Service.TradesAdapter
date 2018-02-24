using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Contract;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface IOrderedCache<T> where T : IEvent 
    {
        Task AddAsync(T e);
        Task<IEnumerable<T>> GetAsync(int skip, int take);
    }
}
