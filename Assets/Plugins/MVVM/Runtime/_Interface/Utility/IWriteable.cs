#nullable enable
namespace Core.MVVM.Utility     
{
    public interface IWriteable 
    {
        public void SetValue(object value);
    }

    public interface IWriteable<T> : IWriteable
    {
        public void SetValue(T value);

        void IWriteable.SetValue(object value) {
            SetValue((T)value);
        }
    }
}
