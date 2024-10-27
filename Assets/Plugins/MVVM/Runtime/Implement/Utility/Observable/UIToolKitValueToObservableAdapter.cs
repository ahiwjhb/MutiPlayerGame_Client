#nullable enable
using System;
using UnityEngine.UIElements;

namespace Core.MVVM.Utility
{
    internal sealed class UIToolKitValueToObservableAdapter<TValue> : IObservableValue<TValue>, ICloseable
    {
        private readonly CallbackEventHandler _uiToolkitEvent;

        private readonly ObservableValue<TValue> _observable;

        public TValue? Value => _observable.Value;

        public event IValueChangedNotifier<TValue>.ValueChangedHandler OnValueChanged {
            add => _observable.OnValueChanged += value;
            remove => _observable.OnValueChanged -= value;
        }

        public event Action<TValue> OnEventHappened {
            add => _observable.OnEventHappened += value;
            remove => _observable.OnEventHappened -= value;
        }

        public UIToolKitValueToObservableAdapter(INotifyValueChanged<TValue> uiToolkitEvent) {
            _observable = new ObservableValue<TValue>(uiToolkitEvent.value);

            _uiToolkitEvent = (CallbackEventHandler)uiToolkitEvent;
            _uiToolkitEvent.RegisterCallback<ChangeEvent<TValue>>(SyncValue_OnUIToolkitEventHappend);
        }

        public void Close() {
            _uiToolkitEvent.UnregisterCallback<ChangeEvent<TValue>>(SyncValue_OnUIToolkitEventHappend);
        }

        private void SyncValue_OnUIToolkitEventHappend(ChangeEvent<TValue> evt) {
            _observable.Value = evt.newValue;
        }
    }
}