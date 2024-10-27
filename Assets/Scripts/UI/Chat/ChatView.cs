# nullable enable
using Core.MVVM.UI;
using TMPro;
using UnityEngine;

namespace MultiPlayerGame.UI.Chat
{

    public class ChatView : MonoView<ChatView, ChatViewModel>
    {
        [SerializeField] TMP_Text _chatterNameText = null!;

        [SerializeField] TMP_Text _chatText = null!;

        protected override void Awake() {
            base.Awake();

            Binder.BuildDataBind<string>(v => v._chatterNameText.text).To(vm => vm.ChatterName);
            Binder.BuildDataBind<string>(v => v._chatText.text).To(vm => vm.ChatText);
        }
    }
}
