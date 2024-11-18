# nullable enable
using Core.MVVM.UI;
using Core.MVVM.Utility;
using Network.Protocol;

namespace MultiPlayerGame.UI.RoomHall
{ 
    internal class DisplayRoomViewModel : ViewModelBase
    {
        public ObservableValue<string> RoomName { get; } = new(string.Empty);

        public ObservableValue<string> WattingPeopelNumberText { get; } = new("0 / 0");

        public ObservableValue<RoomState> RoomState { get; } = new(global::Network.Protocol.RoomState.Watting);

        public ObservableValue<int> MaxPeopelLimitInRoom { get; } = new();

        public ObservableValue<int> CurrentPeopelNumberInRoom { get; } = new();

        public int RoomID { get; private set; }

        public DisplayRoomViewModel(int roomID) {
            RoomID = roomID;
            CurrentPeopelNumberInRoom.OnValueChanged += UpdateWattingPeopelNumberText_OnCurrentPeopleNumberChanged;
            MaxPeopelLimitInRoom.OnValueChanged += UpdateWattingPeopelNumberText_OnPeopelLimitChanged;
        }

        public void EnterRoom() {
            SendEvent(new EnterRoomButtonClickedArgs() {
                EnterRoomID = RoomID
            });
        }

        private void UpdateWattingPeopelNumberText_OnCurrentPeopleNumberChanged(int oldValue, int newValue) {
            string text = WattingPeopelNumberText.Value!;
            WattingPeopelNumberText.Value = newValue.ToString() + text[text.IndexOf(" /")..];
        }

        private void UpdateWattingPeopelNumberText_OnPeopelLimitChanged(int oldValue, int newValue) {
            string text = WattingPeopelNumberText.Value!;
            WattingPeopelNumberText.Value = text[..(text.IndexOf("/ ") + 2)] + newValue.ToString();
        }
    }
}
