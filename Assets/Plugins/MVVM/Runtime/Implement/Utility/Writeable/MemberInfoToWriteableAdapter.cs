#nullable enable
using System.Reflection;
using System;

namespace Core.MVVM.Utility
{
    internal class MemberInfoToWriteableAdapter<TOwner, TMember> : IWriteable<TMember>
    {
        private readonly IWriteable<TMember> writeable;

        public MemberInfoToWriteableAdapter(TOwner owner, MemberInfo memberInfo) {
            if (memberInfo is FieldInfo fieldInfo) {
                writeable = new FieldInfoToWriteableAdapter<TOwner, TMember>(owner, fieldInfo);
            }
            else if (memberInfo is PropertyInfo propertyInfo) {
                writeable = new PropertyInfoToWriteableAdapter<TOwner, TMember>(owner, propertyInfo);
            }
            else {
                throw new NotSupportedException();
            }
        }

        public void SetValue(TMember value) {
            writeable.SetValue(value);
        }
    }
}
