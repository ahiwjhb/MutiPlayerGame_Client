# nullable enable
using Core.MVVM.UI;
using Game.UI.WindowManager;
using MultiPlayerGame.Network;

namespace MultiPlayerGame.UI
{

    public abstract class ViewModel : ViewModelBase
    {
        protected WindowManager WindowManager { get; }

        protected Client Client { get; }

        public ViewModel() {
            WindowManager = Services.Instance.GetService<WindowManager>();
            Client = Services.Instance.GetService<Client>();
        }
    }
}
