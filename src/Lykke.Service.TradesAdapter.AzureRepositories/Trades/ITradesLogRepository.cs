using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core;

namespace Lykke.Service.TradesAdapter.AzureRepository.Trades
{
    public interface ITradesLogRepository
    {
        Task AddOrMergeMultipleAsync(IEnumerable<ITrade> trades);

        Task<IEnumerable<TradeLogEntity>> GetLatestAsync(string assetPairId, int n);
    }
}
