# nullable enable
using Core.MVVM.UI;
using Core.MVVM.Utility;
using Cysharp.Threading.Tasks;
using MultiPlayerGame.Procedure;
using MultiPlayerGame.UI.Chat;
using MultiPlayerGame.UI.RoomHall;
using Network.Protocol;
using System.Linq;

namespace MultiPlayerGame.UI.Room
{

    public class RoomViewModel : ViewModel
    {
        public int RoomID { get;}

        public int RoomOwnerID { get; }

        public int LocalPlayerID { get; }

        public ObservableValue<string> PrepareOrStartGameButtonText { get; } = new(string.Empty);

        public ObservableValue<bool> SendChatButtonEnable { get; } = new(false);

        public ObservableValue<string> InputChatText { get; } = new(string.Empty);

        public ObservableList<ChatViewModel> ChatViewModels { get; } = new();

        public ObservableList<DispalyPlayerViewModel> DispalyPlayerViewModels { get; } = new();

        public RoomViewModel(int roomID, int roomOwnerID, int playerID) {
            RoomID = roomID;
            RoomOwnerID = roomOwnerID;
            LocalPlayerID = playerID;

            if (RoomOwnerID == LocalPlayerID) {
                PrepareOrStartGameButtonText.Value = "开始";
            }
            else {
                PrepareOrStartGameButtonText.Value = "准备";
            }

            InputChatText.OnValueChanged += (_, newValue) => {
                SendChatButtonEnable.Value = string.IsNullOrEmpty(newValue) == false;
            };
        }

        public override void OnOpenFinished(IView view) {
            base.OnOpenFinished(view);
            Client.AddMessageListener<StartGameRequest>(StartGame);
            Client.AddMessageListener<JoinRoomRequest>(HandleOtherPlayerJoinRoom);
            Client.AddMessageListener<ExitRoomRequest>(HandelOtherPlayerExitRoom);
            Client.AddMessageListener<ChatInfo>(HandleSendChat);
        }

        public override void OnStartColse(IView view) {
            base.OnStartColse(view);
            Client.RemoveMessageListener<StartGameRequest>(StartGame);
            Client.RemoveMessageListener<JoinRoomRequest>(HandleOtherPlayerJoinRoom);
            Client.RemoveMessageListener<ExitRoomRequest>(HandelOtherPlayerExitRoom);
            Client.RemoveMessageListener<ChatInfo>(HandleSendChat);
        }

        public void SendChat() {
            var sendChatRequest = new SendChatRequest() {
                RequesterID = LocalPlayerID,
                RoomID = RoomID,
                ChatContent = InputChatText
            };

            Client.SendMessageAsync(sendChatRequest);
            InputChatText.Value = string.Empty;
        }

        public void PrepareOrStartGame() {
            if(RoomOwnerID == LocalPlayerID) {
                var request = new StartGameRequest() {
                    RoomID = RoomID
                };
                Client.SendMessageAsync(request);
            }
            else {

            }
        }

        public async void ExitRoom() {
            var result = await Client.RequestExitRoomAsync(LocalPlayerID, RoomID, timeout: 3f);
            if (result.IsSuccessful) {
                WindowManager.OpenWindow<RoomHallView>();
            }
        }

        private void StartGame(object _, StartGameRequest __) {
            MainProcedure.Instance.StartGame(LocalPlayerID, DispalyPlayerViewModels.Select(item => item.PlayerID));
        }

        private async void HandleOtherPlayerJoinRoom(object _, JoinRoomRequest request) {
            if (request.JoinRoomID != RoomID) return;

            var userInfoRequest = await Client.RequestUserInfoAsync(request.RequesterID, timeout: 2f);
            if (userInfoRequest.IsSuccessful) {
                var userInfo = userInfoRequest.Args.Unpack<UserInfo>();
                lock (DispalyPlayerViewModels) {
                    DispalyPlayerViewModels.Add(userInfo.ToViewModel(request.RequesterID));
                }
            }
        }

        private void HandelOtherPlayerExitRoom(object _, ExitRoomRequest request) {
            if (request.ExitRoomID != RoomID) return;

            int removeIndex = -1;
            for(int i = DispalyPlayerViewModels.Count - 1; i >= 0; --i) {
                if (DispalyPlayerViewModels[i].PlayerID == request.RequesterID) {
                    removeIndex = i;
                    break;
                }
            }

            if (removeIndex != -1) {
                lock (DispalyPlayerViewModels) {
                    if (removeIndex != -1) {
                        DispalyPlayerViewModels.RemoveAt(removeIndex);
                    }
                }
            }
        }

        private async void HandleSendChat(object _, ChatInfo chatInfo) {
            ChatViewModels.Add(await chatInfo.ToViewModel());
        }
    }
}
