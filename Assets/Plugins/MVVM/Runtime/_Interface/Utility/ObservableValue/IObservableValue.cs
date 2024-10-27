#nullable enable
using UnityEngine;

namespace Core.MVVM.Utility
{
    public interface IObservableValue<TValue> : IValueChangedNotifier<TValue>, IReadable<TValue>
    {
        public delegate bool ValueChangeVerifyHandler(TValue? oldValue, TValue? newValue);

        public TValue? Value { get;}

        TValue? IReadable<TValue>.GetValue() {
            return Value;
        }
    }
}
