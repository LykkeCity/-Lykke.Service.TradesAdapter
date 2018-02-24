using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Common;
using Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;
using Lykke.Service.TradesAdapter.AzureRepository.Trades;
using Lykke.Service.TradesAdapter.Job.Settings.JobSettings;
using Lykke.Service.TradesAdapter.Core.Services;
using Lykke.Service.TradesAdapter.Job.RabbitPublishers;
using Lykke.Service.TradesAdapter.Job.RabbitSubscribers;
using Lykke.Service.TradesAdapter.Job.Settings;
using Lykke.Service.TradesAdapter.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.TradesAdapter.Job.Modules
{
    public class JobModule : Module
    {
        private readonly AppSettings _settings;
        private readonly IReloadingManager<DbSettings> _dbSettingsManager;
        private readonly ILog _log;
        private readonly IServiceCollection _services;

        public JobModule(AppSettings settings, IReloadingManager<DbSettings> dbSettingsManager, ILog log)
        {
            _settings = settings;
            _log = log;
            _dbSettingsManager = dbSettingsManager;

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

            LoadAzureRepositories(builder);
            
            _services.RegisterAssetsClient(
                AssetServiceSettings.Create(new Uri(_settings.AssetsServiceClient.ServiceUrl),
                    TimeSpan.FromMinutes(3)));

            LoadAssetsCacheDictionaries(builder);

            LoadServices(builder);

            LoadRabbitSubscribers(builder);

            builder.Populate(_services);
        }

        private void LoadAzureRepositories(ContainerBuilder builder)
        {
            builder.RegisterInstance(new TradesLogRepository(
                    AzureTableStorage<TradeLogEntity>.Create(
                        _dbSettingsManager.ConnectionString(x => x.DataConnString),
                        TradesLogRepository.TableName,
                        _log),
                    AzureTableStorage<AzureIndex>.Create(
                        _dbSettingsManager.ConnectionString(x => x.DataConnString),
                        TradesLogRepository.TableName,
                        _log
                )))
                .As<ITradesLogRepository>()
                .SingleInstance();
        }
        
        private void LoadRabbitSubscribers(ContainerBuilder builder)
        {
            builder.RegisterType<LimitOrdersSubscriberForDb>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("connectionString", _settings.TradesAdapterJob.LimitOrderTradesSettings.ConnectionString)
                .WithParameter("exchangeName", _settings.TradesAdapterJob.LimitOrderTradesSettings.ExchangeName);
            
            builder.RegisterType<LimitOrdersSubscriberForPublishing>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("connectionString", _settings.TradesAdapterJob.LimitOrderTradesSettings.ConnectionString)
                .WithParameter("exchangeName", _settings.TradesAdapterJob.LimitOrderTradesSettings.ExchangeName);
        }
        
        private void LoadServices(ContainerBuilder builder)
        {
            builder.RegisterType<TradesConverter>()
                .As<ITradesConverter>()
                .SingleInstance();

            builder.RegisterType<TradesPublisher>()
                .As<ITradesPublisher>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("connectionString", _settings.TradesAdapterJob.PublishQueueSetings.ConnectionString);

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
