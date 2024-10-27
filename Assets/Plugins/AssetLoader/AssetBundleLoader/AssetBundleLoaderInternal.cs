#nullable enable
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using UnityEngine;

namespace Core.AssetLoader.AssetBundleLoader
{
    /// <summary>
    /// 为单个AssetBundle提供Asset缓存的类
    /// </summary>
    internal class AssetBundleLoaderInternal
    {
        private readonly AssetBundle assetBundle;

        private readonly ConcurrentDictionary<string, WeakReference<UnityEngine.Object>> assetCache = new();

        public AssetBundleLoaderInternal(AssetBundle assetBundle) {
            this.assetBundle = assetBundle;
        }

        public void UnLoad() {
            assetBundle.Unload(false);
            assetCache.Clear();
        }

        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object {
            WeakReference<UnityEngine.Object> reference = assetCache.GetOrAdd(assetName, _ => new(null!));
            if(!reference.TryGetTarget(out var asset)) {
                asset = assetBundle.LoadAsset(assetName);
                reference.SetTarget(asset);
            }
            return (T)asset;
        }

        public async UniTask<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object {
            WeakReference<UnityEngine.Object> reference = assetCache.GetOrAdd(assetName, _ => new(null!));
            if (!reference.TryGetTarget(out var asset)) {
                await UniTask.SwitchToMainThread();
                if (!reference.TryGetTarget(out asset)) {
                    asset = await assetBundle.LoadAssetAsync(assetName);
                    reference.SetTarget(asset);
                }
            }
            return (T)asset;
        }
    }

    //对AssetBundleLoader读取子资源功能进行补充
    //internal partial class AssetBundleLoader
    //{
        //private struct SubAssectKey
        //{
        //    public string atlasName;
        //    public string subAssetName;

        //    public SubAssectKey(string a, string b) {
        //        atlasName = a;
        //        subAssetName = b;
        //    }
        //}

        //private Dictionary<SubAssectKey, UnityEngine.Object> subAssectCache = new Dictionary<SubAssectKey, UnityEngine.Object>();

        //public T LoadSubAsset<T>(string atlasName, string subAssetName) where T : UnityEngine.Object {
        //    var key = new SubAssectKey(atlasName, subAssetName);
        //    if (!subAssectCache.TryGetValue(key, out var asset)) {
        //        UnityEngine.Object[] subAssetArray = assetBundle.LoadAssetWithSubAssets(atlasName);
        //        asset = subAssetArray?.FirstOrDefault(obj => subAssetName.Equals(obj.name));
        //        if (asset != null) {
        //            subAssectCache.Add(key, asset);
        //        }
        //    }
        //    return (T)asset;
        //}

        //private void DebugIfAssetIsNull<T>(object asset, string assetName) where T : UnityEngine.Object {
        //    if (asset == null)
        //        Debug.LogError($"找不到名称为 {assetName} 的资源，请检测名称是否正确");
        //    else if (asset is not T)
        //        Debug.LogError($"资源类型转换错误， 不存在{asset.GetType().Name} 到 {typeof(T).Name}类型的转换");
        //}
    //}

}

