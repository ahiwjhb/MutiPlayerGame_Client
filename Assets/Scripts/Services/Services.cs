using System;
using Logger;
using Core.AssetLoader;
using Game.UI.WindowManager;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using Network.SteamEncipherer;
using MultiPlayerGame.Network;
using Game;

namespace MultiPlayerGame
{
    [DefaultExecutionOrder(-100)]
    public class Services : DontDestoryMonoSingleton<Services>, IServiceProvider
    {
        private IServiceCollection _serviceCollection;

        private IServiceProvider _serviceProvider;

        protected override void Init() {
            base.Init();
            _serviceCollection = new ServiceCollection();

            _serviceCollection.AddSingleton<IServiceProvider>(this);
            _serviceCollection.AddSingleton<IStreamEncipherer, NonStreamEncryption>();
            _serviceCollection.AddSingleton<SceneLoader>(_ => SceneLoader.Instance);
            _serviceCollection.AddLogService();
            _serviceCollection.AddNetworkService();
            _serviceCollection.AddAssetLoadService();
            _serviceCollection.AddWindowManagementService();
            _serviceProvider = _serviceCollection.BuildServiceProvider();
        }

        public T GetService<T>() {
            return _serviceProvider.GetRequiredService<T>();
        }

        public object GetService(Type serviceType) {
            return _serviceProvider.GetService(serviceType);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            GetService<Client>().ShutdownConnect();
        }

        public void StartConfig() {
            GetService<Client>().Start();
        }
    }
}