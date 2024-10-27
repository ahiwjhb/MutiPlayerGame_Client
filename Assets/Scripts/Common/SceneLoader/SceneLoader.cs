# nullable enable
using Cysharp.Threading.Tasks;
using Game.UI.WindowManager;
using MultiPlayerGame;
using MultiPlayerGame.UI.SceneLoading;
using System;
using UnityEngine.SceneManagement;

namespace Game
{

    public class SceneLoader : DontDestoryMonoSingleton<SceneLoader>
    {
        public event Action<string>? OnSceneLoading;

        public event Action<string>? OnSceneLoadCompleted;

        public async UniTask LoadSenceAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single) {
            var windowManager = Services.Instance.GetService<WindowManager>();
            var view = windowManager.OpenWindow<SceneLoadingView>();
            view.ViewModel.Report(0);

            OnSceneLoading?.Invoke(sceneName);
            await SceneManager.LoadSceneAsync(sceneName, loadSceneMode).ToUniTask(view.ViewModel);
            OnSceneLoadCompleted?.Invoke(sceneName);

            windowManager.CloseWindow(view);
        }
    }
}
