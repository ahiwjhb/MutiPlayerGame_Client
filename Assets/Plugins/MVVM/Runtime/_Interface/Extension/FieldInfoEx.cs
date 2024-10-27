#nullable enable
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.MVVM.Extension
{
    public static class FieldInfoEx
    {
        public static Expression<Action<TOwner, TValue>> CreateSetterExpression<TOwner, TValue>(this FieldInfo fieldInfo) {
            var ownerParameExp = Expression.Parameter(typeof(TOwner), "owner");
            var newValueParameExp = Expression.Parameter(fieldInfo.FieldType, "value");
            var fieldGetterExp = Expression.Field(fieldInfo.IsStatic ? null : ownerParameExp, fieldInfo);
            var setterExp = Expression.Assign(fieldGetterExp, newValueParameExp);
            return Expression.Lambda<Action<TOwner, TValue>>(setterExp, ownerParameExp, newValueParameExp);
        }

        public static Expression CreateSetterExpression(this FieldInfo fieldInfo, Type delegateType, Type ownerType){
            var ownerParameExp = Expression.Parameter(ownerType, "owner");
            var newValueParameExp = Expression.Parameter(fieldInfo.FieldType, "value");
            var fieldGetterExp = Expression.Field(fieldInfo.IsStatic ? null : ownerParameExp, fieldInfo);
            var setterExp = Expression.Assign(fieldGetterExp, newValueParameExp);
            return Expression.Lambda(delegateType, setterExp, ownerParameExp, newValueParameExp);
        }

        public static Expression<Func<TOwner, TValue>> CreateGetterExpression<TOwner, TValue>(this FieldInfo fieldInfo) {
            var ownerParameExp = Expression.Parameter(typeof(TOwner), "owner");
            var newValueParameExp = Expression.Parameter(fieldInfo.FieldType, "value");
            var fieldGetterExp = Expression.Field(fieldInfo.IsStatic ? null : ownerParameExp, fieldInfo);
            return Expression.Lambda<Func<TOwner, TValue>>(fieldGetterExp, ownerParameExp, newValueParameExp);
        }

        public static Expression CreateGetterExpression(this FieldInfo fieldInfo, Type delegateType, Type ownerType) {
            var ownerParameExp = Expression.Parameter(ownerType, "owner");
            var newValueParameExp = Expression.Parameter(fieldInfo.FieldType, "value");
            var fieldGetterExp = Expression.Field(fieldInfo.IsStatic ? null : ownerParameExp, fieldInfo);
            return Expression.Lambda(delegateType, fieldGetterExp, ownerParameExp, newValueParameExp);
        }
    }
}