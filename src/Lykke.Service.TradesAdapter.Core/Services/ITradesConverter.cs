using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core.IncomingMessages.LimitOrders;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface ITradesConverter
    {
        Task<List<Trade>> ConvertAsync(LimitOrders orders);
    }
}
