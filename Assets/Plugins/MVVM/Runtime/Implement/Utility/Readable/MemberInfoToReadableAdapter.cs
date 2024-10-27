#nullable enable
using System.Diagnostics.Contracts;
using System.Reflection;
using System;

namespace Core.MVVM.Utility
{
    internal class MemberInfoToReadableAdapter<TOwner, TMember> : IReadable<TMember>
    {
        private readonly IReadable<TMember> readable;

        public MemberInfoToReadableAdapter(TOwner owner, MemberInfo memberInfo) {
            Contract.Assert(memberInfo is FieldInfo or PropertyInfo);

            if(memberInfo is FieldInfo fieldInfo) {
                readable = new FieldInfoToReadableAdapter<TOwner, TMember>(owner, fieldInfo);
            }
            else if(memberInfo is PropertyInfo propertyInfo) {
                readable = new PropertyInfoToReadableAdapter<TOwner, TMember>(owner, propertyInfo);
            }
            else {
                throw new NotSupportedException();
            }
        }

        public TMember? GetValue() {
            return readable.GetValue();
        }
    }
}
