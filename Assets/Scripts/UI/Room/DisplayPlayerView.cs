# nullable enable
using Core.MVVM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiPlayerGame.UI.Room
{
    public class DisplayPlayerView : MonoView<DisplayPlayerView, DispalyPlayerViewModel>
    {
        [SerializeField] TMP_Text _playerNameText = null!;

        [SerializeField] Image _profilePhoto = null!;

        [SerializeField] Image _signalStrengthIcon = null!;

        [SerializeField] TMP_Text _prepareStateText = null!;

        protected override void Awake() {
            base.Awake();

            Binder.BuildDataBind<string>(v => v._playerNameText.text).To(vm => vm.PlayerName);
            Binder.BuildDataBind<Sprite>(v => v._profilePhoto.sprite).To(vm => vm.ProfilePhoto);
            Binder.BuildDataBind<Sprite>(v => v._signalStrengthIcon.sprite).To(vm => vm.SignalStrengthIcon);
            Binder.BuildDataBind<string>(v => v._prepareStateText.text).To(vm => vm.PrepareState);
        }
    }
}
