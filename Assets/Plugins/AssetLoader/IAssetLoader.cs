#nullable enable
#nullable enable
using Cysharp.Threading.Tasks;
using System;

namespace Core.AssetLoader 
{
    public interface IAssetLoader 
    {
        /// <summary>
        /// 加载路径资产
        /// </summary>
        /// <exception cref="ArgumentException">无法加载路径资源时将抛出</exception>
        public T Load<T>(string assetPath) where T : UnityEngine.Object;

        /// <summary>
        /// 异步加载路径资产
        /// </summary>
        /// <exception cref="ArgumentException">无法加载路径资源时将抛出</exception>
        public UniTask<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object;

        public void UnLoadAsset();

        public void UnLoadUnUsedAsset();
    }    
}
