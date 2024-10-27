using System;
using UnityEngine;
using Core.MVVM.UI;
using UnityEngine.Pool;
using Core.AssetLoader;
using MultiPlayerGame;
using System.Collections.Generic;
using MultiPlayerGame.UI.MessageTip;
using MultiPlayerGame.UI.Login;
using MultiPlayerGame.UI.Register;
using MultiPlayerGame.UI.RoomHall;
using MultiPlayerGame.UI.Room;
using MultiPlayerGame.UI.Chat;
using MultiPlayerGame.UI.SceneLoading;

namespace Game.UI.WindowManager
{
    public class WindowManager : DontDestoryMonoSingleton<WindowManager>
    {
        private readonly Dictionary<Type, IObjectPool<IView>> _windowTypeToObjectPoolMapping = new();

        private readonly Dictionary<Type, string> _windowTypeToLoadPathMapping = new();

        private readonly List<(IView, OpenModel)> _openningWindowList = new();

        private IAssetLoader AssetLoader { get; set; }

        public WindowManager() {
            _windowTypeToLoadPathMapping.Add(typeof(LoginView), AssetPath.Prefab.UI.Login);
            _windowTypeToLoadPathMapping.Add(typeof(RegisterView), AssetPath.Prefab.UI.Register);
            _windowTypeToLoadPathMapping.Add(typeof(RoomHallView), AssetPath.Prefab.UI.RoomHall);
            _windowTypeToLoadPathMapping.Add(typeof(RoomView), AssetPath.Prefab.UI.Room);
            _windowTypeToLoadPathMapping.Add(typeof(SceneLoadingView), AssetPath.Prefab.UI.SceneLoading);
            _windowTypeToLoadPathMapping.Add(typeof(MessageTipView), AssetPath.Prefab.UI.MessageTip);
        }

        protected override void Init() {
            base.Init();
            AssetLoader = Services.Instance.GetService<IAssetLoader>();
            foreach(var name in Enum.GetNames(typeof(RenderLayer))){
                GameObject canvas = AssetLoader.Load<GameObject>(AssetPath.Prefab.UI.Canvas);
                canvas.name = name;
                canvas.transform.SetParent(transform);
            }
        }

        public void UnLoadCache() {
            foreach(var pool in _windowTypeToObjectPoolMapping.Values) {
                pool.Clear();
            }
            _windowTypeToObjectPoolMapping.Clear();
            for(int i = 0; i < transform.childCount; ++i) {
                Transform chlid = transform.GetChild(i);
                for (int j = 0; j < chlid.childCount; ++j) {
                    Destroy(chlid.GetChild(j).gameObject);
                }
            }
            _openningWindowList.Clear();
        }

        public void OpenTipWindow(string tip, string levelIconPath) {
            var messageTipView = OpenWindow<MessageTipView>(0.5f, OpenModel.Override, RenderLayer.Highest);

            messageTipView.ViewModel.Message.Value = tip;
            messageTipView.ViewModel.TipIcon.Value = AssetLoader.Load<Sprite>(levelIconPath);
        }

        public void CloseCurrentWindow(float fadeTime = 0f) {
            if (_openningWindowList.Count > 0) {
                CloseWindow(_openningWindowList.Count - 1, fadeTime);
            }
        }

        public void CloseWindow(IView window, float fadeTime = 0f) {
            int index = _openningWindowList.FindLastIndex(pair => pair.Item1 == window);
            if(index != -1) {
                CloseWindow(index, fadeTime);
            }
        }

        public TWindow OpenWindow<TWindow>(float fadeTime = 0, OpenModel openModel = OpenModel.ShutdownOtherWindow, RenderLayer layer = RenderLayer.Default) where TWindow : Component, IView {
            var window = GetWindow<TWindow>();
            OpenWindow(window, fadeTime, openModel, layer);
            return window;
        }

        public void OpenWindow(IView window, float fadeTime = 0, OpenModel openModel = OpenModel.ShutdownOtherWindow, RenderLayer layer = RenderLayer.Default) {
            if (openModel == OpenModel.ShutdownOtherWindow) {
                CloseAllWindow();
            }
            else if (openModel == OpenModel.PopUp && GetCurrentWindow() != null) {
                GetCurrentWindow().Interactable = false;
            }
            _openningWindowList.Add((window, openModel));
            window.SetParent(transform.GetChild((int)layer));
            window.OpenAsync(fadeTime);
        }

        public T GetWindow<T>() where T : Component, IView {
            if (_windowTypeToObjectPoolMapping.TryGetValue(typeof(T), out var pool) == false) {
                pool = new ObjectPool<IView>(
                    createFunc: () => AssetLoader.Load<GameObject>(_windowTypeToLoadPathMapping[typeof(T)]).GetComponentInChildren<T>()
                );
                _windowTypeToObjectPoolMapping.Add(typeof(T), pool);
            }
            return pool.Get() as T;
        }

        public IView GetCurrentWindow() {
            return _openningWindowList.Count >= 0 ? _openningWindowList[^1].Item1 : null;
        }

        public void CloseAllWindow(float fadeTime = 0f) {
            while(_openningWindowList.Count > 0) {
                CloseWindow(_openningWindowList.Count - 1, fadeTime);
            }
        }

        private async void CloseWindow(int index, float fadeTime = 0f) {
            var (window, openModel) = _openningWindowList[index];
            var previousWindow = index - 1 >= 0 ? _openningWindowList[index - 1].Item1 : null;

            _openningWindowList.RemoveAt(index);
            await window.CloseAsync(fadeTime);
            ReleaseWindow(window);
            if (openModel == OpenModel.PopUp && previousWindow != null) {
                previousWindow.Interactable = true;
            }
        }

        private void ReleaseWindow(IView window){
            var pool = _windowTypeToObjectPoolMapping[window.GetType()];
            pool.Release(window);
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            CloseAllWindow();
        }
    }
}
