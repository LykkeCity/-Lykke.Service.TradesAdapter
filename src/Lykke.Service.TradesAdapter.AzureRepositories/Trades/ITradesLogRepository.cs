using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Contract;

namespace Lykke.Service.TradesAdapter.AzureRepository.Trades
{
    public interface ITradesLogRepository
    {
        Task AddIfMissingMultipleAsync(IEnumerable<ITrade> trades);

        Task<IEnumerable<TradeLogEntity>> GetLatestAsync(string assetPairId, int n);
    }
}
