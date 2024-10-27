using TMPro;
using Core.MVVM.UI;
using UnityEngine.UI;
using UnityEngine;

namespace MultiPlayerGame.UI.Register
{
    public class RegisterView : MonoView<RegisterView, RegisterViewModel>
    {
        [SerializeField] TMP_InputField _usernameInputField;

        [SerializeField] TMP_InputField _passwordInputField;

        [SerializeField] Button _registerButton;

        [SerializeField] Button _switchToLoginButton;

        protected override void Awake() {
            base.Awake();

            transform.Find("Title").GetComponent<TMP_Text>().text = "注册界面";
            _usernameInputField.placeholder.GetComponent<TMP_Text>().text = "用户名";
            _passwordInputField.placeholder.GetComponent<TMP_Text>().text = "密码";
            _registerButton.GetComponentInChildren<TMP_Text>().text = "注册";
            _switchToLoginButton.GetComponentInChildren<TMP_Text>().text = "登录";

            Binder.BuildDataBind<bool>(v => v._switchToLoginButton.interactable).To(vm => vm.RegisterButtonEnable);
            Binder.BuildInvCommandBind(vm => vm.SwitchToLoginView).To(v => v._switchToLoginButton.onClick);
            Binder.BuildInvDataBind(vm => vm.Username).To(v => v._usernameInputField.text, v => v._usernameInputField.onEndEdit).TwoWay();
            Binder.BuildInvDataBind(vm => vm.Password).To(v => v._passwordInputField.text, v => v._passwordInputField.onEndEdit).TwoWay();
            Binder.BuildInvCommandBind(vm => vm.Register).To(v => v._registerButton.onClick);

            ViewModel = new RegisterViewModel();
        }
    }
}
