using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using JetBrains.Annotations;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core;

namespace Lykke.Service.TradesAdapter.AzureRepository.Trades
{
    [UsedImplicitly]
    public class TradesLogRepository : ITradesLogRepository
    {
        public const string TableName = "TradesAdapted";
        private const string IndexId = "IndexId";
        private readonly INoSQLTableStorage<TradeLogEntity> _tableStorage;
        private readonly INoSQLTableStorage<AzureIndex> _idIndex;

        public TradesLogRepository(
            INoSQLTableStorage<TradeLogEntity> tableStorage,
            INoSQLTableStorage<AzureIndex> idIndex)
        {
            _tableStorage = tableStorage;
            _idIndex = idIndex;
        }
        
        public async Task AddOrMergeMultipleAsync(IEnumerable<ITrade> trades)
        {
            if (trades == null || !trades.Any())
                return;

            if (await GetAsync(trades.First().Id) != null)
                return;
            
            var entities = trades.Select(TradeLogEntity.Create).ToArray();

            await _tableStorage.InsertAsync(entities);
            await _idIndex.InsertAsync(entities.Select(t => AzureIndex.Create(IndexId, t.Id, t.PartitionKey, t.RowKey)));
        }

        public Task<IEnumerable<TradeLogEntity>> GetLatestAsync(string assetPairId, int n)
        {
            return _tableStorage.GetTopRecordsAsync(assetPairId.ToLower(), n);
        }
        
        private Task<TradeLogEntity> GetAsync(string id)
        {
            return _tableStorage.GetDataAsync(_idIndex, IndexId, id);
        }
    }
}
