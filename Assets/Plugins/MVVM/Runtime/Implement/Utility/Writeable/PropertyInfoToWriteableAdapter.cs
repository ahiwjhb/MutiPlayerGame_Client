#nullable enable
using System.Reflection;
using System;

namespace Core.MVVM.Utility
{
    internal class PropertyInfoToWriteableAdapter<TOwner, TMember> : IWriteable<TMember>
    {
        private readonly Action<TMember> _propertyWriter;

        public PropertyInfoToWriteableAdapter(TOwner memberOwner, PropertyInfo propertyInfo) {
            if (!propertyInfo.CanWrite) {
                throw new ArgumentException("Property dont has setter accessor");
            }

            Action<TOwner, TMember> writer = (Action<TOwner, TMember>)propertyInfo.GetSetMethod().CreateDelegate(typeof(Action<TOwner, TMember>));
            _propertyWriter = newValue => writer(memberOwner, newValue);
        }

        public void SetValue(TMember value) {
            _propertyWriter(value);
        }
    }
}