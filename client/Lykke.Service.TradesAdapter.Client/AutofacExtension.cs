using System;
using Autofac;
using Common.Log;

namespace Lykke.Service.TradesAdapter.Client
{
    public static class AutofacExtension
    {
        public static void RegisterTradesAdapterClient(this ContainerBuilder builder, string serviceUrl, ILog log)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null) throw new ArgumentNullException(nameof(serviceUrl));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (string.IsNullOrWhiteSpace(serviceUrl))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceUrl));

            builder.RegisterType<TradesAdapterClient>()
                .WithParameter("serviceUrl", serviceUrl)
                .As<ITradesAdapterClient>()
                .SingleInstance();
        }

        public static void RegisterTradesAdapterClient(this ContainerBuilder builder, TradesAdapterServiceClientSettings settings, ILog log)
        {
            builder.RegisterTradesAdapterClient(settings?.ServiceUrl, log);
        }
    }
}
