#nullable enable
using System;
using System.Reflection;

namespace Core.MVVM.Utility
{
    internal class PropertyInfoToReadableAdapter<TOwner, TValue> : IReadable<TValue>
    {
        private Func<TValue> _propertyGetterDelegate;

        public PropertyInfoToReadableAdapter(TOwner owner, PropertyInfo propertyInfo) {
            if (!propertyInfo.CanRead) {
                throw new ArgumentException("Property dont has getter accessor");
            }

            Func<TOwner, TValue> getter = (Func<TOwner, TValue>)propertyInfo.GetGetMethod().CreateDelegate(typeof(Func<TOwner, TValue>));
            _propertyGetterDelegate = () => getter(owner);
        }

        public TValue GetValue() {
            return _propertyGetterDelegate();
        }
    }
}
