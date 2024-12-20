#nullable enable
using Core.AssetLoader;
using Cysharp.Threading.Tasks;
using MultiPlayerGame.Network;
using MultiPlayerGame.UI.Chat;
using MultiPlayerGame.UI.Room;
using Network.Protocol;
using UnityEngine;
namespace MultiPlayerGame
{
    public static class ProtocolExtension
    {
        public static DispalyPlayerViewModel ToViewModel(this UserInfo info, int id) {
            var viewModel = new DispalyPlayerViewModel();
            viewModel.PlayerID = id;
            viewModel.PlayerName.Value = info.Name;
            viewModel.ProfilePhoto.Value = Services.Instance.GetService<IAssetLoader>().Load<Sprite>(AssetPath.Icon.DefaultProfilePhoto);
            viewModel.SignalStrengthIcon.Value = Services.Instance.GetService<IAssetLoader>().Load<Sprite>(AssetPath.Icon.SignalStrength.Good);
            return viewModel;
        }

        public static async UniTask<ChatViewModel> ToViewModel(this ChatInfo chatInfo) {
            var viewModel = new ChatViewModel();
            var res = await Services.Instance.GetService<Client>().RequestUserInfoAsync(chatInfo.SenderID, timeout: 3f);
            viewModel.ChatText.Value = chatInfo.ChatContent;
            if (res.IsSuccessful) {
                viewModel.ChatterName.Value = res.Args.Unpack<UserInfo>().Name;
            }
            return viewModel;
        }

        public static void Set(this global::Network.Protocol.Vector3 self, UnityEngine.Vector3 other) {
            self.X = other.x;
            self.Y = other.y;
            self.Z = other.z;
        }

        public static UnityEngine.Vector3 ToUnityVector(this global::Network.Protocol.Vector3 self) {
            return new UnityEngine.Vector3(self.X, self.Y, self.Z);
        }
    }
}
