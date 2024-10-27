#nullable enable
using Core.MVVM.Extension;
using System;
using System.Reflection;

namespace Core.MVVM.Utility
{
    internal class FieldInfoToWriteableAdapter<TOwner, TMember> : IWriteable<TMember>
    {
        private readonly Action<TMember> _fieldWriterDelegate;

        public FieldInfoToWriteableAdapter(TOwner memberOwner, FieldInfo fieldInfo) {
            Action<TOwner, TMember> writer = fieldInfo.CreateSetterExpression<TOwner, TMember>().Compile();
            _fieldWriterDelegate = newValue => writer(memberOwner, newValue);
        }

        public void SetValue(TMember value) {
            _fieldWriterDelegate(value);
        }
    }
}
