using UnityEngine;
using Core.AssetLoader;
using Game.UI.WindowManager;
using MultiPlayerGame.UI.Login;
using System.Collections.Generic;
using Game;
using Cysharp.Threading.Tasks;
using System;
using MultiPlayerGame.Network;

namespace MultiPlayerGame.Procedure
{
    public class MainProcedure : DontDestoryMonoSingleton<MainProcedure>
    {
        private void Start() {
            Services.Instance.StartConfig();

            var assetLoader = Services.Instance.GetService<IAssetLoader>();
            WindowManager windowManager = Services.Instance.GetService<WindowManager>();
            windowManager.OpenWindow<LoginView>();
        }

        public async void StartGame(int localPlayerID, IEnumerable<int> playerIDList) {
            try {
                await UniTask.SwitchToMainThread();
                var sceneLoader = Services.Instance.GetService<SceneLoader>();
                await sceneLoader.LoadSenceAsync("GameScene");
                GameProcedure procedure = GameObject.FindObjectOfType<GameProcedure>();
                procedure.GameStart(localPlayerID, playerIDList);
            }
            catch(Exception e) {
                Debug.LogError(e);
            }
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            var client = Services.Instance.GetService<Client>();
            //client.SendMessageAsync()
        }
    }
}
