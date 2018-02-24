using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Service.TradesAdapter.Contract;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface ITradesPublisher
    {
        Task PublishAsync(List<Trade> trades);
    }
}
