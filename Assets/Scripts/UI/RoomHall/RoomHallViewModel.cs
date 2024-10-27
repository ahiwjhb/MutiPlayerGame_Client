using Core.AssetLoader;
using Core.MVVM.UI;
using Core.MVVM.Utility;
using Cysharp.Threading.Tasks;
using Game.UI.WindowManager;
using MultiPlayerGame.UI.Room;
using Network.Protocol;
using System;
using System.Collections.Generic;

namespace MultiPlayerGame.UI.RoomHall
{
    public class EnterRoomButtonClickedArgs : EventArgs 
    { 
        public int EnterRoomID { get; set; }
    }

    public class RoomHallViewModel : ViewModel
    {
        public int PlayerID { get; set; }

        public ObservableValue<string> InputRoomName { get; } = new(string.Empty);

        internal ObservableList<DisplayRoomViewModel> DisplayRoomViewModels { get; } = new(); 

        public RoomHallViewModel() {
            AddListener<EnterRoomButtonClickedArgs>(EnterRoom_OnEnterRoomButtonClicked);
        }

        public override async void OnStartOpen(IView view) {
            base.OnStartOpen(view);

            var result = await Client.RequestSearchRoomAsync("*", timeout: 3f);
            if (result.IsSuccessful == false) {
                WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Error);
            }
            else {
                var roomList = result.Args.Unpack<RoomListPack>().RoomList;
                DisplayRoomViewModels.Clear();
                SyncRoomListToServer(roomList);
            }
        }

        public async void SearchRoom() {
            if (string.IsNullOrEmpty(InputRoomName)) {
                WindowManager.OpenTipWindow("房间名不能为空!", AssetPath.Icon.OperationResult.Warring);
            }
            else {
                var result = await Client.RequestSearchRoomAsync(InputRoomName, timeout: 3f);
                if (result.IsSuccessful == false) {
                    WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Error);
                }
                else {
                    DisplayRoomViewModels.Clear();
                    var roomList = result.Args.Unpack<RoomListPack>().RoomList;
                    if (roomList.Count == 0) {
                        WindowManager.OpenTipWindow("没有匹配的房间名称", AssetPath.Icon.OperationResult.Warring);
                    }
                    else {
                        SyncRoomListToServer(roomList);
                    }
                }
            }
        }

        public async void CreateRoom() {
            if (string.IsNullOrEmpty(InputRoomName)) {
                WindowManager.OpenTipWindow("房间名不能为空！", AssetPath.Icon.OperationResult.Warring);
            }
            else if(InputRoomName == "*") {
                WindowManager.OpenTipWindow("不能含有敏感字符", AssetPath.Icon.OperationResult.Warring);
            }
            else {
                var result = await Client.RequestCreatRoomAsync(PlayerID, InputRoomName, maxPeopleLimit: 5, timeout: 3f);
                if (result.IsSuccessful == false) {
                    WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Warring);
                }
                else {
                    await OpenRoomView(result.Args.Unpack<RoomInnerInfo>());
                    WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Successful);
                }
            }
        } 

        private void SyncRoomListToServer(IEnumerable<RoomDisplayInfo> roomList) {
            foreach (var outerRoomInfo in roomList) {
                var itemViewModel = RoomDisplayInfoToViewModel(outerRoomInfo);
                DisplayRoomViewModels.Add(itemViewModel); ;
            }
        }

        private async void EnterRoom_OnEnterRoomButtonClicked(object sender, EventArgs args) {
            int joinRoomID = ((EnterRoomButtonClickedArgs)args).EnterRoomID;

            var result = await Client.RequestJoinRoomAsync(PlayerID, joinRoomID, timeout: 3f);
            if(result.IsSuccessful == false) {
                WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Error);
            }
            else {
                await OpenRoomView(result.Args.Unpack<RoomInnerInfo>());
                WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Successful);
            }
        }

        private DisplayRoomViewModel RoomDisplayInfoToViewModel(RoomDisplayInfo info) {
            var viewModel = new DisplayRoomViewModel(info.ID);
            viewModel.Parent = this;
            viewModel.RoomName.Value = info.RoomName;
            viewModel.CurrentPeopelNumberInRoom.Value = info.CurrentPeopleNumber;
            viewModel.MaxPeopelLimitInRoom.Value = info.MaxPeopleLimit;
            viewModel.RoomState.Value = info.RoomState == RoomState.Starting ? "游戏中" : "筹备中";
            return viewModel;
        }

        private async UniTask OpenRoomView(RoomInnerInfo info) {
            var viewModel = new RoomViewModel(info.ID, info.RoomOwnerID, PlayerID);
            foreach (var playerID in info.PlayerIDList) {
                var queryUserInfoResult = await Client.RequestUserInfoAsync(playerID, timeout: 1f);
                if (queryUserInfoResult.IsSuccessful) {
                    var userInfo = queryUserInfoResult.Args.Unpack<UserInfo>();
                    viewModel.DispalyPlayerViewModels.Add(userInfo.ToViewModel(playerID));
                }
            }
            foreach (var chatInfo in info.ChatHistories) {
                viewModel.ChatViewModels.Add(chatInfo.ToViewModel());
            }

            var roomView = WindowManager.GetWindow<RoomView>();
            roomView.ViewModel = viewModel;
            WindowManager.OpenWindow(roomView);
        }
    }  
}
