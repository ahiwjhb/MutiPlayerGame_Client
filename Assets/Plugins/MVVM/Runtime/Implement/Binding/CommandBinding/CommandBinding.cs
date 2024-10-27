#nullable enable
using Core.MVVM.Utility;

namespace Core.MVVM.Binding
{
    public sealed class CommandBinding : IBinding
    {
        private readonly ICommand _command;

        private readonly IEventNotifier _notifier;
        
        private readonly BindModel _bindModel;

        private bool _enable = false;

        internal CommandBinding(ICommand command, IEventNotifier eventNotifier, BindModel bindModel) {
            _command = command;
            _notifier = eventNotifier;
            _bindModel = bindModel;
        }

        public void Enable() {
            if (_enable) return;

            _notifier.OnEventHappened += SyncEventHappened;

            _enable = true;
        }

        public void Disable() {
            if (!_enable) return;

            _notifier.OnEventHappened -= SyncEventHappened;

            _enable = false;
        }

        public void Close() {
            Disable();
            (_notifier as ICloseable)?.Close();
        }

        private void SyncEventHappened() {
            _command.Execute();
            if(_bindModel == BindModel.OneTime) {
                Disable();
            }
        }
    }
}

namespace Core.MVVM.Binding
{
    public sealed class CommandBinding<TCommandArgs, TEventArgs> : IBinding where TEventArgs : TCommandArgs
    {
        private readonly ICommand<TCommandArgs> _command;

        private readonly IEventNotifier<TEventArgs> _notifier;

        private readonly BindModel _bindModel;

        private bool _enable = false;

        internal CommandBinding(ICommand<TCommandArgs> command, IEventNotifier<TEventArgs> eventNotifier, BindModel bindModel) {
            _command = command;
            _notifier = eventNotifier;
            _bindModel = bindModel;
        }

        public void Enable() {
            if (_enable) return;

            _notifier.OnEventHappened += SyncEventHappened;

            _enable = true;
        }

        public void Disable() {
            if (!_enable) return;

            _notifier.OnEventHappened -= SyncEventHappened;

            _enable = false;
        }

        public void Close() {
            Disable();
            (_notifier as ICloseable)?.Close();
        }

        private void SyncEventHappened(TEventArgs? eventArgs) {
            _command.Execute(eventArgs);
            if (_bindModel == BindModel.OneTime) {
                Disable();
            }
        }
    }
}