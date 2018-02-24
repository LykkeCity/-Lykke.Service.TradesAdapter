using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.TradesAdapter.Contract;
using Lykke.Service.TradesAdapter.Core.Services;
using Lykke.Service.TradesAdapter.RabbitSubscribers;
using Lykke.Service.TradesAdapter.Settings.ServiceSettings;
using Lykke.Service.TradesAdapter.Services;
using Lykke.Service.TradesAdapter.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.TradesAdapter.Modules
{
    public class ServiceModule : Module
    {
        private readonly AppSettings _settings;
        private readonly ILog _log;
        private readonly IServiceCollection _services;

        public ServiceModule(AppSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
            
            _services.RegisterAssetsClient(
                AssetServiceSettings.Create(new Uri(_settings.AssetsServiceClient.ServiceUrl),
                    TimeSpan.FromMinutes(3)));

            LoadAssetsCacheDictionaries(builder);

            LoadServices(builder);

            LoadRabbitSubscribers(builder);

            builder.Populate(_services);
        }

        private void LoadRabbitSubscribers(ContainerBuilder builder)
        {
            builder.RegisterType<LimitOrdersSubscriberForCache>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("connectionString", _settings.TradesAdapterService.LimitOrderTradesSettings.ConnectionString)
                .WithParameter("exchangeName", _settings.TradesAdapterService.LimitOrderTradesSettings.ExchangeName);
        }

        private void LoadServices(ContainerBuilder builder)
        {
            builder.RegisterType<TradesConverter>()
                .As<ITradesConverter>()
                .SingleInstance();

            builder.RegisterType<CacheOfCaches>()
                .As<ICacheOfCaches<Trade>>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(_settings.TradesAdapterService.CacheSize));

            builder.RegisterType<AssetsServiceWrapperWithCache>()
                .As<IAssetsServiceWrapperWithCache>()
                .SingleInstance();
            
            builder.RegisterType<RabbitSubscriberHelper>()
                .As<IRabbitSubscriberHelper>()
                .SingleInstance();
        }

        private void LoadAssetsCacheDictionaries(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var ctx = c.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, Asset>(
                    async () =>
                        (await ctx.Resolve<IAssetsService>().AssetGetAllAsync()).ToDictionary(itm => itm.Id));
            }).SingleInstance();

            builder.Register(c =>
            {
                var ctx = c.Resolve<IComponentContext>();
                return new CachedDataDictionary<string, AssetPair>(
                    async () =>
                        (await ctx.Resolve<IAssetsService>().AssetPairGetAllAsync())
                        .ToDictionary(itm => itm.Id));
            }).SingleInstance();
        }
    }
}
