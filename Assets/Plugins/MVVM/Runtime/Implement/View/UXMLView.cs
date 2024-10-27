#nullable enable
using UnityEngine;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using Core.MVVM.Extension;
using System;

namespace Core.MVVM.UI
{
    public class UXMLView<TView, TViewModel> : IView<TViewModel> where TView : class, IView<TViewModel> where TViewModel : ViewModelBase
    {
        protected VisualElement _self;

        protected ViewBindingCollection<TView, TViewModel> Binder { get; private set; }

        public VisualElement View => _self;

        public ITransform Transform => _self.transform;

        public IStyle Style => _self.style;

        public Vector2 WorldPosition => _self.worldTransform.GetPosition();

        private bool _isOpeningOrClosing = false;

        public UXMLView(VisualElement view) : base() {
            var instance = this as TView;
            ExceptionEx.ThrowIfNull<InvalidCastException>(instance, "{0} cant not case to {1}", GetType().Name, nameof(TView));
            Binder = new ViewBindingCollection<TView, TViewModel>(instance);
            _self = view;
        }

        public void InitView(string uxmlPath) {
           // _self = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath).Instantiate()[0];
        }

        public TViewModel ViewModel {
            get => Binder.BindingContext ?? throw new MissingMemberException($"the {nameof(ViewModel)} is not be set");
            set => Binder.BindingContext = value;
        }

        public bool Interactable {
            get => _self.enabledSelf;
            set => _self.SetEnabled(value);
        }
        public float Alpha {
            get => _self.style.opacity.value;
            set => _self.style.opacity = value;
        }

        public bool Visiable {
            get => _self.visible;
            set => _self.visible = value;
        }

        public VisualElement Parent {
            get => _self.parent ?? _self;
            set {
                _self.parent?.Remove(_self);
                value?.Add(_self);
            }
        }

        public Vector2 LocalPosition {
            get => Parent.WorldToLocal(WorldPosition);
            set {
                Style.position = Position.Absolute;
                Style.left = value.x;
                Style.top = value.y;
            }
        }

        public virtual async UniTask OpenAsync(float fadeTime = 0) {
            if (_isOpeningOrClosing) return;

            Binder.BindingContext?.OnStartOpen(this);
            Visiable = true;
            _isOpeningOrClosing = true;
            if (fadeTime > 0) {
                for (float ratio = 0; ratio <= 1; ratio += Time.deltaTime / fadeTime) {
                    Alpha = ratio;
                    await UniTask.Yield();
                }
            }
            Alpha = 1;
            _isOpeningOrClosing = false;
            Binder.BindingContext?.OnOpenFinished(this);
        }

        public virtual async UniTask CloseAsync(float fadeTime = 0) {
            if (_isOpeningOrClosing)

                ViewModel.OnStartColse(this);
            if (fadeTime > 0) {
                for (float ratio = 1; ratio >= 0; ratio -= Time.deltaTime / fadeTime) {
                    Alpha = ratio;
                    await UniTask.Yield();
                }
            }
            Alpha = 0;
            Visiable = false;
            ViewModel.OnCloseFinished(this);
        }

        public T Q<T>(string? name = null, string? className = null) where T : VisualElement {
            return _self.Q<T>(name, className);
        }

        public void Destory() {
            _self.RemoveFromHierarchy();
        }

        public void SetParent(object parent) {
            ((VisualElement)parent).Add(_self);
        }
    }
}
