# nullable enable
using Core.AssetLoader;
using Cysharp.Threading.Tasks;
using MultiPlayerGame.Game;
using MultiPlayerGame.Network;
using Network.Protocol;
using System.Collections.Generic;
using UnityEngine;

namespace MultiPlayerGame.Procedure
{
    public class GameProcedure : MonoSingleton<GameProcedure>
    {
        private Dictionary<int, GameObject> _playerIDToInstanceMapping = new();

        private Dictionary<int, Bullet> _bulletIDToInstanceMapping = new();

        private int a;

        private int _localPlayerID;

        private void Awake() {
            var client = Services.Instance.GetService<Client>();
            client.AddMessageListener<SyncPlayerPositionRequest>(HandlePlayerSyncPositionRequest);
            client.AddMessageListener<FireRequest>(HandlePlayerFire);
        }

        protected override void OnDestroy() {
            var client = Services.Instance.GetService<Client>();
            client.RemoveMessageListener<SyncPlayerPositionRequest>(HandlePlayerSyncPositionRequest);
            client.RemoveMessageListener<FireRequest>(HandlePlayerFire);
        }


        public void GameStart(int localPlayerID, IEnumerable<int> playerIDList) {
            _localPlayerID = localPlayerID;
            var assetLoader = Services.Instance.GetService<IAssetLoader>();
            foreach (var id in playerIDList) {
                if (id != _localPlayerID) {
                    var ob = assetLoader.Load<GameObject>(AssetPath.Prefab.PlayerVisual);
                    ob.transform.position = transform.position;
                    _playerIDToInstanceMapping.Add(id, ob);
                }
                else {
                    var ob = assetLoader.Load<GameObject>(AssetPath.Prefab.Player);
                    ob.transform.position = transform.position;
                    _playerIDToInstanceMapping.Add(id, ob);
                    ob.GetComponent<PlayerController>().PlayerID = id;
                }
            }
        }

        private async void HandlePlayerSyncPositionRequest(object sender, SyncPlayerPositionRequest request) {
            await UniTask.SwitchToMainThread();
            int id = request.RequesterID;
            if (id != _localPlayerID) {
                var ob = _playerIDToInstanceMapping[id];
                ob.transform.position = request.PlayerPosition.ToUnityVector();
                ob.transform.eulerAngles = request.PlayerRotation.ToUnityVector();
            }
        }

        private async void HandlePlayerFire(object _, FireRequest request) {
            await UniTask.SwitchToMainThread();
            _playerIDToInstanceMapping[request.RequesterID].GetComponentInChildren<Gun>().Fire();
        }
    }
}
