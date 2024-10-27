#nullable enable
using Core.MVVM.UI;
using System;
using UnityEngine.Events;

namespace Core.MVVM.Utility
{
    internal sealed class UnityEventToObservableAdapter<TValue> : IObservableValue<TValue>
    {
        private UnityEvent<TValue> _unityEvent;

        private ObservableValue<TValue> _observable;

        public TValue? Value => _observable.Value;

        public event IValueChangedNotifier<TValue>.ValueChangedHandler OnValueChanged {
            add => _observable.OnValueChanged += value;
            remove => _observable.OnValueChanged -= value;
        }

        public event Action<TValue> OnEventHappened {
            add => _observable.OnEventHappened += value;
            remove => _observable.OnEventHappened -= value;
        }

        public UnityEventToObservableAdapter(TValue defaultValue, UnityEvent<TValue> valueUpdatedTrigger) {
            _observable = new ObservableValue<TValue>(defaultValue);

            _unityEvent = valueUpdatedTrigger;
            _unityEvent.AddListener(SyncValue_OnUnityEventHappend);
        }

        private void SyncValue_OnUnityEventHappend(TValue newValue) {
            _observable.Value = newValue;
        }
    }
}
