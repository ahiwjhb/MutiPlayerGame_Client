#nullable enable
using Cysharp.Threading.Tasks;
using System.Collections;

namespace Core.MVVM.UI
{
    public interface IView 
    {
        public UniTask OpenAsync(float fadeTime = 0);

        public UniTask CloseAsync(float fadeTime = 0);

        public void SetParent(object parent);

        public bool Interactable { get; set; }

        public float Alpha { get; set; }

        public bool Visiable { get; set; }

        public void Destory();
    }
}

namespace Core.MVVM.UI
{
    public interface IView<TViewModel> : IView where TViewModel : ViewModelBase
    {
        public TViewModel ViewModel { get; set; }
    }
}
