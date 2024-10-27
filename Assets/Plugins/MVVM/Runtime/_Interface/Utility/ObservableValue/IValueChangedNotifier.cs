#nullable enable
#nullable enable
namespace Core.MVVM.Utility
{
    public interface IValueChangedNotifier<TValue> : IEventNotifier<TValue>
    {
        public delegate void ValueChangedHandler(TValue? oldValue, TValue? newValue);

        public event ValueChangedHandler OnValueChanged;

        public void AddHandler(ValueChangedHandler handler) {
            OnValueChanged += handler;
        }

        public void RemoveHandler(ValueChangedHandler handler) {
            OnValueChanged -= handler;
        }
    }
}
