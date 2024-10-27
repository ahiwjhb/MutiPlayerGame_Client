#nullable enable
using System;
using System.Collections.Generic;

namespace Core.MVVM.Utility
{
    public sealed class ObservableValue<TValue> : IObservableValue<TValue>, IWriteable<TValue>
    {
        private TValue? _value;

        public event IObservableValue<TValue>.ValueChangedHandler? OnValueChanged;

        public event Action<TValue?>? OnEventHappened;

        public IObservableValue<TValue>.ValueChangeVerifyHandler? ValueChangeVerify { get; set; }

        public ObservableValue(): this(default, null!) { }

        public ObservableValue(IObservableValue<TValue>.ValueChangeVerifyHandler valueChangeVerify) :this(default, valueChangeVerify) { }

        public ObservableValue(TValue value) : this(value, null!) { }

        public ObservableValue(TValue? value, IObservableValue<TValue>.ValueChangeVerifyHandler valueChangeVerify) {
            _value = value;
            ValueChangeVerify = valueChangeVerify;
        }

        public TValue? Value {
            get => _value;
            set {
                if (EqualityComparer<TValue>.Default.Equals(_value!, value!) == false && (ValueChangeVerify == null || ValueChangeVerify(_value, value))) {
                    var old = _value;
                    _value = value;
                    OnValueChanged?.Invoke(old, value);
                    OnEventHappened?.Invoke(value);
                }
            }
        }

        public void SetValue(TValue value) {
            Value = value;
        }

        public override string ToString() {
            return $"{typeof(TValue).Name} {Value?.ToString()}";
        }

        public static implicit operator TValue?(ObservableValue<TValue> observable) {
            return observable.Value;
        }
    }
}
