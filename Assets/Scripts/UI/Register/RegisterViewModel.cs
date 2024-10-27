using Core.MVVM.Utility;
using Core.AssetLoader;
using Game.UI.WindowManager;
using MultiPlayerGame.UI.Login;
using MultiPlayerGame.Network;

namespace MultiPlayerGame.UI.Register
{
    public class RegisterViewModel : ViewModel
    {
        public ObservableValue<string> Username { get;} = new(string.Empty);

        public ObservableValue<string> Password { get;} = new(string.Empty);

        public ObservableValue<bool> RegisterButtonEnable { get; } = new(true);

        public async void Register() {
            RegisterButtonEnable.Value = false;
            if (string.IsNullOrEmpty(Username)) {
                WindowManager.OpenTipWindow("用户名不能为空", AssetPath.Icon.OperationResult.Warring);
            }
            else if (string.IsNullOrEmpty(Password)) {
                WindowManager.OpenTipWindow("密码不能为空", AssetPath.Icon.OperationResult.Warring);
            }
            else {
                var result = await Services.Instance.GetService<Client>().RequestRegisterAsync(Username, Password, timeout: 2);
                if (result.IsSuccessful) {
                    WindowManager.OpenTipWindow("注册成功", AssetPath.Icon.OperationResult.Successful);
                }
                else {
                    WindowManager.OpenTipWindow(result.Information, AssetPath.Icon.OperationResult.Error);
                }
            }
            RegisterButtonEnable.Value = true;
        }

        public void SwitchToLoginView() {
            WindowManager.OpenWindow<LoginView>();
        }
    }
}
