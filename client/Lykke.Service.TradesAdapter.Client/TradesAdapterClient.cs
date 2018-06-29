using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Service.TradesAdapter.AutorestClient;
using Lykke.Service.TradesAdapter.AutorestClient.Models;
using Lykke.Service.TradesAdapter.Client.Models;
using Microsoft.Rest;

namespace Lykke.Service.TradesAdapter.Client
{
    public class TradesAdapterClient : ITradesAdapterClient, IDisposable
    {
        private readonly ILog _log;
        private TradesAdapterAPI _apiClient;

        public TradesAdapterClient(string serviceUrl, ILog log)
        {
            _log = log;
            _apiClient = new TradesAdapterAPI(new Uri(serviceUrl), new HttpClient());
        }

        private TradesAdapterResponse PrepareResponseMultiple(HttpOperationResponse<object> serviceResponse)
        {
            var error = serviceResponse.Body as ErrorResponse;
            var result = serviceResponse.Body as IList<Trade>;

            if (error != null)
            {
                return new TradesAdapterResponse
                {
                    Error = new ErrorModel
                    {
                        Message = error.ErrorMessage
                    }
                };
            }

            if (result != null)
            {
                return new TradesAdapterResponse
                {
                    Records = result
                };
            }

            throw new ArgumentException("Unknown response object");
        }

        public async Task<TradesAdapterResponse> GetTradesByAssetPairIdAsync(string assetPairId, int skip, int take)
        {
            var response =
                await _apiClient.AssetPairIdByAssetPairIdGetWithHttpMessagesAsync(assetPairId, skip, take);

            return PrepareResponseMultiple(response);
        }

        public void Dispose()
        {
            if (_apiClient == null)
                return;
            _apiClient.Dispose();
            _apiClient = null;
        }
    }
}
