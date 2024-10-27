using Core.MVVM.UI;
using Core.MVVM.Utility;
using Cysharp.Threading.Tasks;
using Game.UI.WindowManager;
using UnityEngine;

namespace MultiPlayerGame.UI.MessageTip
{ 
    public class MessageTipViewModel : ViewModelBase
    {
        public ObservableValue<string> Message { get; } = new();

        public ObservableValue<Sprite> TipIcon { get; } = new();

        public override async void OnOpenFinished(IView view) {
            base.OnOpenFinished(view);
            const float showTime = 2f;
            await UniTask.Delay((int)(showTime * 1000));
            Services.Instance.GetService<WindowManager>().CloseWindow(view, fadeTime: 0.5f);
        }
    }
}
