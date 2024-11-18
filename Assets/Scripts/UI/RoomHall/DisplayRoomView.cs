# nullable enable
using Core.MVVM.UI;
using Network.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiPlayerGame.UI.RoomHall
{
    internal class DisplayRoomView : MonoView<DisplayRoomView, DisplayRoomViewModel>
    {
        [SerializeField] TMP_Text _roomNameText = null!;

        [SerializeField] TMP_Text _wattingPeopleNumberText = null!;

        [SerializeField] TMP_Text _roomStateText = null!;

        [SerializeField] Button _enterToRoomButton = null!;

        protected override void Awake() {
            base.Awake();

            Binder.BuildDataBind<string>(v => v._roomNameText.text).To(vm => vm.RoomName);
            Binder.BuildDataBind<string>(v => v._wattingPeopleNumberText.text).To(vm => vm.WattingPeopelNumberText);
            Binder.BuildDataBind<string>(v => v._roomNameText.text).To(vm => vm.RoomName);
            Binder.BuildDataBind<string>(v => v._roomStateText.text).To(vm => vm.RoomState).ValueWrap<RoomState, string>(value => value == RoomState.Watting ? "筹备中" : "已开始");
            Binder.BuildInvCommandBind(vm => vm.EnterRoom).To(v => v._enterToRoomButton.onClick);
        }
    }
}
