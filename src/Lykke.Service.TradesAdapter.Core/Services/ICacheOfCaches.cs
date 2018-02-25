using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Contract;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface ICacheOfCaches
    {
        Task AddAsync(string key, Trade item);
        Task<IEnumerable<Trade>> GetAsync(string key, int skip, int take);
    }
}
