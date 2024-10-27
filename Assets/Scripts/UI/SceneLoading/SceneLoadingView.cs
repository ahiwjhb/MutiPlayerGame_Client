# nullable enable
using Core.MVVM.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MultiPlayerGame.UI.SceneLoading
{
    public class SceneLoadingView : MonoView<SceneLoadingView, SceneLoadingViewModel>
    {
        [SerializeField] Image _processImage = null!;

        protected override void Awake() {
            base.Awake();

            Binder.BuildDataBind<float>(v => v._processImage.fillAmount).To(vm => vm.LoadingProcess);

            ViewModel = new SceneLoadingViewModel();
        }
    }
}
