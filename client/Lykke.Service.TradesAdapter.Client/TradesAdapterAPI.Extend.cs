using System;
using System.Net.Http;

// ReSharper disable once CheckNamespace
namespace Lykke.Service.TradesAdapter.AutorestClient
{
    public partial class TradesAdapterAPI
    {
        /// <inheritdoc />
        /// <summary>
        /// Should be used to prevent memory leak in RetryPolicy
        /// </summary>
        public TradesAdapterAPI(Uri baseUri, HttpClient client) : base(client)
        {
            Initialize();

            BaseUri = baseUri ?? throw new ArgumentNullException("baseUri");
        }
    }
}
