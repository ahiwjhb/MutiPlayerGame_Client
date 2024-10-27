#nullable enable
using System;

namespace Core.MVVM.Utility
{
    internal sealed class ObservableValueCaseDecorator<OldType, NewType> : IObservableValue<NewType>, ICloseable
    {
        private readonly IObservableValue<OldType> _concreteObservable;

        private readonly Func<OldType?, NewType?> _valueCaseDelegate;

        public event IValueChangedNotifier<NewType>.ValueChangedHandler? OnValueChanged;

        public event Action<NewType?>? OnEventHappened;

        public NewType? Value => _valueCaseDelegate(_concreteObservable.Value);

        public ObservableValueCaseDecorator(IObservableValue<OldType> observable, Func<OldType?, NewType?> valueCaseDelegate) {
            _concreteObservable = observable;
            _valueCaseDelegate = valueCaseDelegate;
            _concreteObservable.OnValueChanged += SyncNotifyEvent_OnConcreteObservableValueChanged;
        }

        public void Close() {
            _concreteObservable.OnValueChanged -= SyncNotifyEvent_OnConcreteObservableValueChanged;
        }

        private void SyncNotifyEvent_OnConcreteObservableValueChanged(OldType? oldValue, OldType? newValue) {
            OnValueChanged?.Invoke(_valueCaseDelegate(oldValue), _valueCaseDelegate(newValue));
            OnEventHappened?.Invoke(_valueCaseDelegate(newValue));
        }
    }
}
