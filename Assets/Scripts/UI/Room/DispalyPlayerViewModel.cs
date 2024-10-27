# nullable enable
using Core.MVVM.Utility;
using UnityEngine;

namespace MultiPlayerGame.UI.Room {

    public class DispalyPlayerViewModel : ViewModel
    {
        public int PlayerID { get; set; }

        public ObservableValue<string> PlayerName { get; } = new(string.Empty);

        public ObservableValue<Sprite> ProfilePhoto { get; } = new();

        public ObservableValue<Sprite> SignalStrengthIcon { get; } = new();

        public ObservableValue<string> PrepareState { get; } = new(string.Empty);
    }
}
