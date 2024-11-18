# nullable enable
using UnityEngine;
using Core.AssetLoader;
using Cysharp.Threading.Tasks;
using MultiPlayerGame.Game;
using MultiPlayerGame.Network;
using Network.Protocol;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;

namespace MultiPlayerGame.Procedure
{
    public class GameProcedure : MonoSingleton<GameProcedure>
    {
        [SerializeField] Vector2 _spanwRange;

        private Dictionary<int, GameObject> _playerIDToInstanceMapping = new();

        private int _localPlayerID;

        private bool _hasBuffer = false;

        protected override void OnDestroy() {
            var client = Services.Instance.GetService<Client>();
            client.RemoveMessageListener<SyncPlayerPositionRequest>(HandlePlayerSyncPositionRequest);
            client.RemoveMessageListener<FireRequest>(HandlePlayerFire);
            client.RemoveMessageListener<CrossPlatformRequest>(HandlePlayerCrossPlatform);
        }

        public void GameStart(int localPlayerID, IEnumerable<int> playerIDList) {
            var client = Services.Instance.GetService<Client>();
            client.AddMessageListener<SyncPlayerPositionRequest>(HandlePlayerSyncPositionRequest);
            client.AddMessageListener<FireRequest>(HandlePlayerFire);
            client.AddMessageListener<CrossPlatformRequest>(HandlePlayerCrossPlatform);

            _localPlayerID = localPlayerID;
            var assetLoader = Services.Instance.GetService<IAssetLoader>();
            foreach (var id in playerIDList) {
                if (id == _localPlayerID) {
                    var ob = assetLoader.Load<GameObject>(AssetPath.Prefab.Player);
                    ob.transform.position = transform.position;
                    ob.transform.position += Random.Range(-_spanwRange.x / 2, _spanwRange.x / 2) * Vector3.right;
                    ob.GetComponentInChildren<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                    ob.GetComponent<PlayerController>().PlayerID = id;
                    _playerIDToInstanceMapping.Add(id, ob);
                }
                else {
                    var ob = assetLoader.Load<GameObject>(AssetPath.Prefab.PlayerVisual);
                    ob.transform.position = transform.position;
                    _playerIDToInstanceMapping.Add(id, ob);
                }
            }
        }

        private async void HandlePlayerSyncPositionRequest(object sender, SyncPlayerPositionRequest request) {
            await UniTask.SwitchToMainThread();
            var ob = _playerIDToInstanceMapping[request.RequesterID];
            ob.transform.position = request.PlayerPosition.ToUnityVector();
            ob.transform.eulerAngles = request.PlayerRotation.ToUnityVector();
        }

        private async void HandlePlayerFire(object _, FireRequest request) {
            await UniTask.SwitchToMainThread();
            _playerIDToInstanceMapping[request.RequesterID].GetComponentInChildren<Gun>().Fire();
        }

        private async void HandlePlayerCrossPlatform(object _, CrossPlatformRequest request) {
            await UniTask.SwitchToMainThread();
            _playerIDToInstanceMapping[request.RequesterID].GetComponentInChildren<PlatformCrosser>().CrossPlatform();
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, _spanwRange);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.tag == "Player" && collision.TryGetComponent<PlayerController>(out var _)) {
                collision.gameObject.transform.position = transform.position;
                collision.gameObject.transform.position += Random.Range(-_spanwRange.x / 2, _spanwRange.x / 2) * Vector3.right;
            }
        }
    }
}
