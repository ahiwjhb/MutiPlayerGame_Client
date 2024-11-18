using TMPro;
using UnityEngine;
using Core.MVVM.UI;
using UnityEngine.UI;

namespace MultiPlayerGame.UI.Login
{
    public sealed class LoginView : MonoView<LoginView, LoginViewModel>
    {
        [SerializeField] TMP_Text _title;

        [SerializeField] TMP_InputField _usernameInputField;

        [SerializeField] TMP_InputField _passwordInputField;

        [SerializeField] Button _loginButton;

        [SerializeField] Button _switchToRegisterButton;


        protected override void Awake() {
            base.Awake();

            _title.text = "登录界面";
            _usernameInputField.placeholder.GetComponent<TMP_Text>().text = "用户名";
            _passwordInputField.placeholder.GetComponent<TMP_Text>().text = "密码";
            _loginButton.GetComponentInChildren<TMP_Text>().text = "登录";
            _switchToRegisterButton.GetComponentInChildren<TMP_Text>().text = "注册";

            Binder.BuildDataBind<bool>(v => v. _loginButton.interactable).To(vm => vm.LoginButtonEnable);
            Binder.BuildInvDataBind(vm => vm.Username).To(v => v._usernameInputField.text, v => v._usernameInputField.onEndEdit).TwoWay();
            Binder.BuildInvDataBind(vm => vm.Password).To(v => v._passwordInputField.text, v => v._passwordInputField.onEndEdit).TwoWay();
            Binder.BuildInvCommandBind(vm => vm.SwitchToRegisterView).To(v => v._switchToRegisterButton.onClick);
            Binder.BuildInvCommandBind(vm => vm.Login).To(v => v._loginButton.onClick);

            ViewModel = new LoginViewModel();
        }
    }
}