using Core.AssetLoader;
using Core.MVVM.Utility;
using Game.UI.WindowManager;
using MultiPlayerGame.UI.Register;
using MultiPlayerGame.UI.RoomHall;
using Google.Protobuf.WellKnownTypes;
using MultiPlayerGame.Network;
using System.Threading.Tasks;

namespace MultiPlayerGame.UI.Login
{
    public sealed class LoginViewModel : ViewModel
    {
        public ObservableValue<string> Username { get;} = new(string.Empty);

        public ObservableValue<string> Password { get;} = new(string.Empty);

        public ObservableValue<bool> LoginButtonEnable { get; } = new(true);

        public async void Login() {
            LoginButtonEnable.Value = false;
            if (string.IsNullOrEmpty(Username)) {
                WindowManager.OpenTipWindow("用户名不能为空", AssetPath.Icon.OperationResult.Warring);
            }
            else if (string.IsNullOrEmpty(Password)) {
                WindowManager.OpenTipWindow("密码不能为空", AssetPath.Icon.OperationResult.Warring);
            }
            else {
                var result = await Client.RequsetLoginAsync(Username, Password, timeout: 3f);
                if (result.IsSuccessful == false) {
                    WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Error);
                }
                else {
                    int loginUserID = result.Args.Unpack<Int32Value>().Value;
                    var roomHallView = WindowManager.GetWindow<RoomHallView>();
                    roomHallView.ViewModel.PlayerID = loginUserID;
                    WindowManager.OpenWindow(roomHallView);
                    WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Successful);
                }
            }
            LoginButtonEnable.Value = true;
        }

        public void SwitchToRegisterView() {
            var windowManager =  Services.Instance.GetService<WindowManager>();
            windowManager.OpenWindow<RegisterView>();
        }
    }
}
