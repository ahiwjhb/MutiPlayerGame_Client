#nullable enable
#nullable enable
using System;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.IO;

/*                              ABManager使用指南
* 该类可以用来加载存储在ABAssets文件夹下的资源
* 设置好AB标签后 AutoGenerateABPackage类会将所有名称与标签名相同的文件夹里的内容自动打上标签
* 同时生成AssetName类记录的资源名的引用
* 自动生成功能请在Unity主窗口 ABGenrate 中选择开启
* 举例：若要加载标签是prefab名称为Tool的GameObject类型资源, 需要在目标文件夹下建立Prefab文件夹然后将资源放入即可
*       var a = ABManager.LoadAsset<GameObject>(AssetBundleLable.prefab, AssetName.prefab.Tool);
*/
namespace Core.AssetLoader.AssetBundleLoader
{
    /// <summary>
    /// AB包资源管理器
    /// </summary>
    internal partial class AssetBundleLoader : IAssetLoader
    {
        //AB包依赖项集合
        private static Lazy<AssetBundleManifest> manifest;

        /// <summary>
        /// AB包缓存字典 AssetBundleName To Loader
        /// </summary>
        private static readonly Dictionary<string, AssetBundleLoaderInternal> assetLoaderCache = new();

        static AssetBundleLoader() {
            manifest = new Lazy<AssetBundleManifest>(() => LoadAssetBundle(PathTool.AssetBundleSaveFolderName).LoadAsset<AssetBundleManifest>("AssetBundleManifest"));
        }

        public void UnLoadAsset() {
            foreach (AssetBundleLoaderInternal loader in assetLoaderCache.Values) {
                loader.UnLoad();
            }
        }

        public void UnLoadUnUsedAsset() {
            UnLoadAsset();
            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 加载AB包资源
        /// </summary>
        /// <typeparam name="T">Asset的类型</typeparam>
        /// <param name="assetName">所属AB包的标签</param>
        /// <returns>返回lable标签的AB包下类型为T 名称为assetName的资源</returns>
        /// <exception cref="ArgumentException">无法加载AB包对应的标签时将抛出</exception>
        public T Load<T>(string assetPath) where T : UnityEngine.Object {
            string assetBundlePath = Path.GetDirectoryName(assetPath);
            string assetName = Path.GetFileNameWithoutExtension(assetPath);
            T asset = GetOrAddLoader(assetBundlePath).LoadAsset<T>(assetName);
            if(asset == null) {
                throw new ArgumentException($"找不到路径为 {assetPath} 的资源，请检测名称是否正确");
            }
            return asset is GameObject ? GameObject.Instantiate<T>(asset) : asset;
        }


        /// <summary>
        /// 通过AssetBundleName获取对应的AB包加载器
        /// </summary>
        /// <param name="lable"></param>
        /// <returns></returns>
        private AssetBundleLoaderInternal GetOrAddLoader(string assetBundlePath) {
            if(!assetLoaderCache.TryGetValue(assetBundlePath, out var loader)) {
                loader = CreatLoader(assetBundlePath);
                assetLoaderCache.Add(assetBundlePath, loader);
            }
            return loader;
        }

        private AssetBundleLoaderInternal CreatLoader(string assetBundlePath) {
            if (!assetLoaderCache.TryGetValue(assetBundlePath, out var loader)) {
                //递归加载目标AB包的依赖项
                foreach (string dependencePath in manifest.Value.GetAllDependencies(assetBundlePath)) {
                    assetLoaderCache.Add(dependencePath, CreatLoader(dependencePath));
                }
                AssetBundle assetBundle = LoadAssetBundle(assetBundlePath);
                loader = new AssetBundleLoaderInternal(assetBundle);
            }
            return loader;
        }

        private static AssetBundle LoadAssetBundle(string assetBundlePath) {
            string path = Path.Combine(PathTool.StreamingAssetsWWWPath, PathTool.AssetBundleSaveFolderName, assetBundlePath);
            UnityWebRequestAsyncOperation asyncOp = UnityWebRequestAssetBundle.GetAssetBundle(path).SendWebRequest();

            while (!asyncOp.isDone) continue;

            return DownloadHandlerAssetBundle.GetContent(asyncOp.webRequest) ?? throw new ArgumentException($"无法加载名称为 {assetBundlePath} 的AB包, 请检测文件名或路径是否正确");
        }
    }

    /// <summary>
    /// 提供异步版本
    /// </summary>
    internal partial class AssetBundleLoader  
    {
        public async UniTask<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object {
            string assetBundlePath = Path.GetDirectoryName(assetPath);
            string assetName = Path.GetFileNameWithoutExtension(assetPath);

            var loader = await GetOrAddLoaderAsync(assetBundlePath);
            T asset = await loader.LoadAssetAsync<T>(assetName);

            if (asset == null) {
                throw new ArgumentException($"找不到路径为 {assetPath} 的资源，请检测名称是否正确");
            }

            await UniTask.SwitchToMainThread();
            return asset is GameObject ? GameObject.Instantiate<T>(asset) : asset;
        }

        private async UniTask<AssetBundleLoaderInternal> GetOrAddLoaderAsync(string assetBundlePath) {
            if (!assetLoaderCache.TryGetValue(assetBundlePath, out var loader)) {
                await UniTask.SwitchToMainThread();
                if (!assetLoaderCache.TryGetValue(assetBundlePath, out loader)) {
                    loader = await CreatLoaderAsync(assetBundlePath);
                    assetLoaderCache.Add(assetBundlePath, loader);
                }
            }
            return loader;
        }

        private async UniTask<AssetBundleLoaderInternal> CreatLoaderAsync(string assetBundlePath) {
            if (!assetLoaderCache.TryGetValue(assetBundlePath, out var loader)) {
                //递归加载目标AB包的依赖项
                foreach (string dependencePath in manifest.Value.GetAllDependencies(assetBundlePath)) {
                    assetLoaderCache.Add(dependencePath, await CreatLoaderAsync(assetBundlePath));
                }
                AssetBundle assetBundle = await LoadAssetBundleAsync(assetBundlePath);
                loader = new AssetBundleLoaderInternal(assetBundle);
            }
            return loader;
        }

        private static async UniTask<AssetBundle> LoadAssetBundleAsync(string assetBundlePath) {
            string path = Path.Combine(PathTool.StreamingAssetsWWWPath, PathTool.AssetBundleSaveFolderName, assetBundlePath);

            UnityWebRequest webRequest = await UnityWebRequestAssetBundle.GetAssetBundle(path).SendWebRequest();

            return DownloadHandlerAssetBundle.GetContent(webRequest) ?? throw new ArgumentException($"无法加载名称为 {assetBundlePath} 的AB包, 请检测文件名或路径是否正确");
        }
    }
}
