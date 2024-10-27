#nullable enable
namespace Core.MVVM.Utility
{
    public interface IReadable 
    {
        public object? GetValue();
    }

    public interface IReadable<T> : IReadable
    {
        public new T? GetValue();


        object? IReadable.GetValue() {
            return GetValue();
        }
    }
}
