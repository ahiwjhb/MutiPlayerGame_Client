#nullable enable
using Core.MVVM.Utility;

namespace Core.MVVM.Binding
{
    public sealed class DataBinding<TObserver, TTarget> : IBinding where TTarget : TObserver
    {
        private readonly IWriteable<TObserver> _observerWriter;

        private readonly IObservableValue<TTarget> _target;

        private readonly BindModel _bindModel;

        private bool _enable;

        internal DataBinding(IWriteable<TObserver> valueWriter, IObservableValue<TTarget> observationTarget, BindModel bindModel) {
            _observerWriter = valueWriter;
            _target = observationTarget;
            _bindModel = bindModel;
        }

        public void Enable() {
            if (_enable) return;

            SyncValue();            
            _target.OnValueChanged += SyncObserverValue_OnObservableValueChanged;
            _enable = true;
        }


        public void Disable() {
            if (!_enable) return;

            _target.OnValueChanged -= SyncObserverValue_OnObservableValueChanged;

            _enable = false;
        }

        public void Close() {
            Disable();
            (_target as ICloseable)?.Close();
        }

        private void SyncObserverValue_OnObservableValueChanged(TTarget? oldValue, TTarget? newValue) {
            SyncValue();
            if (_bindModel == BindModel.OneTime) {
                Disable();
            }
        }

        private void SyncValue() {
            _observerWriter.SetValue(_target.GetValue()!);
        }
    }
}
