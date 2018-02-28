using System.Threading.Tasks;
using Common;

namespace Lykke.Service.TradesAdapter.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();

        void Register(IStopable stopable);
    }
}
