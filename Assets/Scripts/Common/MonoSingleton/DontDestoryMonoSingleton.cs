using Cysharp.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 不会被销毁的持久单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class DontDestoryMonoSingleton<T> : MonoSingleton<T> where T : MonoSingleton<T>
{
    protected override void Init() {
        base.Init();
        UniTask.RunOnThreadPool(async () => {
            await UniTask.SwitchToMainThread();
            transform.parent = null;
            DontDestroyOnLoad(this);
        }).Forget();
    }
}

