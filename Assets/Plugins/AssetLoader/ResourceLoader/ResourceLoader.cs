#nullable enable
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace Core.AssetLoader.ResourceLoader
{
    internal class ResourceLoader : IAssetLoader
    {
        private readonly ConcurrentDictionary<string, WeakReference<UnityEngine.Object>> _assetPathToAssetMapping = new();

        public T Load<T>(string assetPath) where T : UnityEngine.Object {
            var reference = _assetPathToAssetMapping.GetOrAdd(assetPath, _ => new WeakReference<UnityEngine.Object>(null!));
            if(reference.TryGetTarget(out var asset) == false) {
                asset = Resources.Load<T>(assetPath);
                reference.SetTarget(asset);
            }

            if (asset == null) {
                throw new ArgumentException($"找不到路径为 {assetPath} 的资源，请检测名称是否正确");
            }

            return (T)(asset is GameObject ? GameObject.Instantiate(asset) : asset);
        }

        public async UniTask<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object {
            var reference = _assetPathToAssetMapping.GetOrAdd(assetPath, _ => new WeakReference<UnityEngine.Object>(null!));
            if (reference.TryGetTarget(out var asset) == false) {
                await UniTask.SwitchToMainThread();
                if (reference.TryGetTarget(out asset) == false) {
                    asset = await Resources.LoadAsync<T>(assetPath);
                    reference.SetTarget(asset);
                }
            }

            if (asset == null) {
                throw new ArgumentException($"找不到路径为 {assetPath} 的资源，请检测名称是否正确");
            }

            return (T)(asset is GameObject ? GameObject.Instantiate(asset) : asset);
        }

        public void UnLoadAsset() {
            _assetPathToAssetMapping.Clear();
            Resources.UnloadUnusedAssets();
        }

        public void UnLoadUnUsedAsset() {
            _assetPathToAssetMapping.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}