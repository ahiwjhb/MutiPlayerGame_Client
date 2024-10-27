# nullable enable
using Core.AssetLoader;
using Core.MVVM.UI;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiPlayerGame.UI.RoomHall
{ 
    public class RoomHallView : MonoView<RoomHallView, RoomHallViewModel>
    {
        [SerializeField] Transform _roomListContainer = null!;

        [SerializeField] Button _logoutButton = null!;

        [SerializeField] TMP_InputField _roomNameInputBox = null!;

        [SerializeField] Button _searchRoomButton = null!;

        [SerializeField] Button _createRoomButton = null!;

        private readonly List<DisplayRoomView> _displayRoomViews = new();

        protected override void Awake() {
            base.Awake();

            _logoutButton.GetComponentInChildren<TMP_Text>().text = "注销登录";
            _roomNameInputBox.placeholder.GetComponent<TMP_Text>().text = "Room Name";
            _searchRoomButton.GetComponentInChildren<TMP_Text>().text = "搜索房间";
            _createRoomButton.GetComponentInChildren<TMP_Text>().text = "创建房间";

            Binder.BuildInvDataBind(vm => vm.InputRoomName).To(v => v._roomNameInputBox.text, v => v._roomNameInputBox.onEndEdit);
            Binder.BuildInvCommandBind(vm => vm.SearchRoom).To(v => v._searchRoomButton.onClick);
            Binder.BuildInvCommandBind(vm => vm.CreateRoom).To(v => v._createRoomButton.onClick);

            ViewModel = new RoomHallViewModel();
        }

        protected override void OnViewModelChange(RoomHallViewModel? oldViewModel, RoomHallViewModel? newViewModel) {
            base.OnViewModelChange(oldViewModel, newViewModel);
            if(oldViewModel != null) {
                oldViewModel.DisplayRoomViewModels.ItemBeAdded -= AddDisplayRoomView_OnRoomBeAdded;
                oldViewModel.DisplayRoomViewModels.ItemBeRemoved -= RemoveDisplayRoomView_OnRoomBeRemoved;
            }

            if(newViewModel != null) {
                foreach(IView roomView in _displayRoomViews) {
                    roomView.Destory();
                }
                _displayRoomViews.Clear();

                for(int i = 0; i < newViewModel.DisplayRoomViewModels.Count; ++i) {
                    DisplayRoomViewModel viewModel = newViewModel.DisplayRoomViewModels[i];
                    AddDisplayRoomView_OnRoomBeAdded(i, viewModel);
                }
                newViewModel.DisplayRoomViewModels.ItemBeAdded += AddDisplayRoomView_OnRoomBeAdded;
                newViewModel.DisplayRoomViewModels.ItemBeRemoved += RemoveDisplayRoomView_OnRoomBeRemoved;
            }
        }

        private async void AddDisplayRoomView_OnRoomBeAdded(int index, DisplayRoomViewModel RoomOuterViewModel) {
            var asssetLoader = Services.Instance.GetService<IAssetLoader>();
            var ob = await asssetLoader.LoadAsync<GameObject>(AssetPath.Prefab.UI.DisplayRoomBox);
            var view = ob.GetComponent<DisplayRoomView>();
            view.SetParent(_roomListContainer);
            view.ViewModel = RoomOuterViewModel;
            view.OpenAsync().Forget();
            _displayRoomViews.Add(view);
        }

        private void RemoveDisplayRoomView_OnRoomBeRemoved(int index, DisplayRoomViewModel _) {
            (_displayRoomViews[index] as IView).Destory();
            _displayRoomViews.RemoveAt(index);
        }
    }
}
