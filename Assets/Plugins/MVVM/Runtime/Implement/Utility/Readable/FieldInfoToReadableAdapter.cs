#nullable enable
using Core.MVVM.Extension;
using System;
using System.Reflection;

namespace Core.MVVM.Utility
{
    internal class FieldInfoToReadableAdapter<TOwner, TValue> : IReadable<TValue>
    {
        private Func<TValue> _fieldGetterDelegate;

        public FieldInfoToReadableAdapter(TOwner owner, FieldInfo fieldInfo) {
            Func<TOwner, TValue> getter = fieldInfo.CreateGetterExpression<TOwner, TValue>().Compile();
            _fieldGetterDelegate = () => getter(owner);
        }

        public TValue GetValue() {
            return _fieldGetterDelegate();
        }
    }
}
