using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.MVVM.UI;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace MultiPlayerGame.UI.MessageTip
{
    public class MessageTipView : MonoView<MessageTipView, MessageTipViewModel>
    {
        [SerializeField] TMP_Text _messageText;

        [SerializeField] Image _tipImage;

        protected override void Awake() {
            base.Awake();

            Binder.BuildDataBind<string>(v => v._messageText.text).To(vm => vm.Message);
            Binder.BuildDataBind<Sprite>(v => v._tipImage.sprite).To(vm => vm.TipIcon);

            ViewModel = new MessageTipViewModel();
        }

        protected override async UniTask OpenAsync_Internal(float fadeTime = 0) {
            var transform = (RectTransform)this.transform;
            float tagetY = transform.position.y - transform.rect.height * 1.25f;
            await UniTask.WhenAll(base.OpenAsync_Internal(fadeTime), transform.DOMoveY(tagetY, fadeTime).ToUniTask());
        }

        protected override async UniTask CloseAsync_Internal(float fadeTime = 0) {
            var transform = (RectTransform)this.transform;
            float tagetY = transform.position.y + transform.rect.height * 1.25f;
            await UniTask.WhenAll(base.CloseAsync_Internal(fadeTime * 0.5f), transform.DOMoveY(tagetY, fadeTime).ToUniTask());
        }
    }
}
