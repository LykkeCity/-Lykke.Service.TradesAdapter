using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Contract;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface IOrderedCache
    {
        Task AddAsync(Trade e);
        Task<IEnumerable<Trade>> GetAsync(int skip, int take);
    }
}
