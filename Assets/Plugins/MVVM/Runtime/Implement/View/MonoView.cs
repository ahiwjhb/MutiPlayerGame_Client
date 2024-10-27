#nullable enable
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace Core.MVVM.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class MonoView<TView, TViewModel> : MonoBehaviour, IView<TViewModel> where TView : MonoView<TView, TViewModel> where TViewModel : ViewModelBase
    {
        private CanvasGroup _canvasGroup = null!;

        private bool _isOpeningOrClosing = false;

        protected ViewBindingCollection<TView, TViewModel> Binder { get; private set; } = null!;

        protected virtual void Awake() {
            _canvasGroup = GetComponent<CanvasGroup>();
            Binder = new ViewBindingCollection<TView, TViewModel>((TView)this);
            Binder.OnBindingContextChanged += OnViewModelChange;
        }

        public TViewModel ViewModel {
            get => Binder.BindingContext ?? throw new MissingMemberException($"the {nameof(ViewModel)} is not be set");
            set => Binder.BindingContext = value;
        }

        public bool Interactable {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        public float Alpha {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value; 
        }

        public bool Visiable { 
            get => gameObject.activeInHierarchy; 
            set => gameObject.SetActive(value); 
        }

        public async UniTask OpenAsync(float fadeTime = 0) {
            if (_isOpeningOrClosing) return;

            ViewModel.OnStartOpen(this);
            Visiable = true;
            _isOpeningOrClosing = true;
            await OpenAsync_Internal(fadeTime);
            _isOpeningOrClosing = false;
            ViewModel.OnOpenFinished(this);
        }

        public async UniTask CloseAsync(float fadeTime = 0) {
            if (_isOpeningOrClosing) return;

            ViewModel.OnStartColse(this);
            await CloseAsync_Internal(fadeTime);
            Visiable = false;
            ViewModel.OnCloseFinished(this);
        }

        protected virtual async UniTask OpenAsync_Internal(float fadeTime = 0) {
            if (fadeTime > 0) {
                for (float ratio = 0; ratio <= 1; ratio += Time.deltaTime / fadeTime) {
                    Alpha = ratio;
                    await UniTask.Yield();
                }
            }
            Alpha = 1;
        }

        protected virtual async UniTask CloseAsync_Internal(float fadeTime = 0) {
            if (fadeTime > 0) {
                for (float ratio = 1; ratio >= 0; ratio -= Time.deltaTime / fadeTime) {
                    Alpha = ratio;
                    await UniTask.Yield();
                }
            }
            Alpha = 0;
        }

        protected virtual void OnViewModelChange(TViewModel? oldViewModel, TViewModel? newViewModel) { }

        void IView.Destory() {
            Destroy(gameObject);
        }

        public void SetParent(object parent) {
            transform.SetParent((Transform)parent, false);
        }
    }
}
