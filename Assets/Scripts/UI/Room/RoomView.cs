# nullable enable
using Core.AssetLoader;
using Core.MVVM.UI;
using Cysharp.Threading.Tasks;
using MultiPlayerGame.UI.Chat;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiPlayerGame.UI.Room
{
    public class RoomView : MonoView<RoomView, RoomViewModel>
    {
        [SerializeField] Button _exitRoomButton = null!;

        [SerializeField] Transform _playerListContainer = null!;

        [SerializeField] Transform _chatListContainer = null!;

        [SerializeField] TMP_InputField _chatInputBox = null!;

        [SerializeField] Button _sendChatButton = null!;

        [SerializeField] Button _PrepareOrStartGameButton = null!;

        private List<DisplayPlayerView> _displayPlayerViews = new();

        private List<ChatView> _chatViews = new();

        protected override void Awake() {
            base.Awake();

            _exitRoomButton.GetComponentInChildren<TMP_Text>().text = "退出房间";
            _sendChatButton.GetComponentInChildren<TMP_Text>().text = "发送";

            Binder.BuildDataBind<string>(v => v._PrepareOrStartGameButton.GetComponentInChildren<TMP_Text>().text).To(vm => vm.PrepareOrStartGameButtonText);
            Binder.BuildDataBind<bool>(v => v._sendChatButton.interactable).To(vm => vm.SendChatButtonEnable);
            Binder.BuildInvDataBind<string>(vm => vm.InputChatText).To(v => v._chatInputBox.text, v => v._chatInputBox.onValueChanged).TwoWay();
            Binder.BuildInvCommandBind(vm => vm.PrepareOrStartGame).To(v => v._PrepareOrStartGameButton.onClick);
            Binder.BuildInvCommandBind(vm => vm.SendChat).To(v => v._sendChatButton.onClick);
            Binder.BuildInvCommandBind(vm => vm.ExitRoom).To(v => v._exitRoomButton.onClick);
        }

        protected override void OnViewModelChange(RoomViewModel? oldViewModel, RoomViewModel? newViewModel) {
            base.OnViewModelChange(oldViewModel, newViewModel);
            if(oldViewModel != null) {
                for(int i = _displayPlayerViews.Count - 1; i >= 0; --i) {
                    RemoveDisplayPlayerView(i, _displayPlayerViews[i].ViewModel);
                }
                for(int i = _chatViews.Count - 1; i >= 0; --i) {
                    RemoveChatView(i, _chatViews[i].ViewModel);
                }

                oldViewModel.DispalyPlayerViewModels.ItemBeAdded -= AddDisplayPlayerView;
                oldViewModel.DispalyPlayerViewModels.ItemBeRemoved -= RemoveDisplayPlayerView;
                oldViewModel.ChatViewModels.ItemBeAdded -= AddChatView;
                oldViewModel.ChatViewModels.ItemBeRemoved -= RemoveChatView;
            }

            if(newViewModel != null) {
                for (int i = 0; i < newViewModel.DispalyPlayerViewModels.Count; ++i) {
                    AddDisplayPlayerView(i, newViewModel.DispalyPlayerViewModels[i]);
                }
                for (int i = 0; i < newViewModel.ChatViewModels.Count; ++i) {
                    AddChatView(i, newViewModel.ChatViewModels[i]);
                }

                newViewModel.DispalyPlayerViewModels.ItemBeAdded += AddDisplayPlayerView;
                newViewModel.DispalyPlayerViewModels.ItemBeRemoved += RemoveDisplayPlayerView;
                newViewModel.ChatViewModels.ItemBeAdded += AddChatView;
                newViewModel.ChatViewModels.ItemBeRemoved += RemoveChatView;
            }
        }

        private async void AddDisplayPlayerView(int index, DispalyPlayerViewModel viewModel) {
            await UniTask.SwitchToMainThread();

            var loader = Services.Instance.GetService<IAssetLoader>();
            var view = (await loader.LoadAsync<GameObject>(AssetPath.Prefab.UI.DisplayPlayerBox)).GetComponent<DisplayPlayerView>();
            view.SetParent(_playerListContainer);
            view.ViewModel = viewModel;
            view.OpenAsync().Forget();
            _displayPlayerViews.Add(view);
        }

        private async void RemoveDisplayPlayerView(int index, DispalyPlayerViewModel viewModel) {
            await UniTask.SwitchToMainThread();

            IView view = _displayPlayerViews[index];
            _displayPlayerViews.RemoveAt(index);
            view.Destory();
        }

        private async void AddChatView(int index, ChatViewModel viewModel) {
            await UniTask.SwitchToMainThread();

            var loader = Services.Instance.GetService<IAssetLoader>();
            var view = (await loader.LoadAsync<GameObject>(AssetPath.Prefab.UI.Chat)).GetComponent<ChatView>();
            view.SetParent(_chatListContainer);
            view.ViewModel = viewModel;
            view.OpenAsync().Forget();
            _chatViews.Add(view);
        }

        private async void RemoveChatView(int index, ChatViewModel viewModel) {
            await UniTask.SwitchToMainThread();

            IView view = _chatViews[index];
            _chatViews.RemoveAt(index);
            view.Destory();
        }
    }
}
