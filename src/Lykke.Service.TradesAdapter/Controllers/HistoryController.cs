using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core.Services;
using Lykke.Service.TradesAdapter.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.TradesAdapter.Controllers
{
    public class HistoryController : Controller
    {
        private const string InvalidAssetPairIdMessage = "Invalid asset pair Id value provided";
        private const string InvalidSkipMessage = "Invalid skip value provided";
        private const string InvalidTakeMessage = "Invalid take value provided";

        public HistoryController(
            ICacheOfCaches cache,
            IAssetsServiceWrapperWithCache assetsServiceWrapperWithCache)
        {
            _cache = cache;
            _assetsServiceWrapperWithCache = assetsServiceWrapperWithCache;
        }
        
        private readonly ICacheOfCaches _cache;
        private readonly IAssetsServiceWrapperWithCache _assetsServiceWrapperWithCache;
        
        /// <summary>
        /// Returns latest trades based on asset pair
        /// </summary>
        /// <param name="assetPairId">ID of asset pair</param>
        /// <param name="skip">How many items to skip</param>
        /// <param name="take">How many items to take</param>
        /// <returns></returns>
        [HttpGet("assetPairId/{assetPairId}")]
        [ProducesResponseType(typeof(IEnumerable<Trade>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetTradesByAssetPairId(
            string assetPairId,
            [FromQuery]int skip,
            [FromQuery]int take)
        {
            if (await _assetsServiceWrapperWithCache.TryGetAssetPairAsync(assetPairId) == null)
            {
                return BadRequest(ErrorResponse.Create(InvalidAssetPairIdMessage));
            }

            if (skip < 0)
            {
                return BadRequest(ErrorResponse.Create(InvalidSkipMessage));
            }

            if (take < 0)
            {
                return BadRequest(ErrorResponse.Create(InvalidTakeMessage));
            }

            var data = await _cache.GetAsync(assetPairId, skip, take);
            
            return Ok(data);
        }
    }
}
