# nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.MVVM.Extension
{
    public static class ExceptionEx
    {
        private static readonly Action<Exception, string> messageSetter;

        static ExceptionEx() {
            var fieldInfo = typeof(Exception).GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic);
            ParameterExpression param01 = Expression.Parameter(typeof(Exception), nameof(param01));
            ParameterExpression param02 = Expression.Parameter(typeof(string), nameof(param02));
            BinaryExpression assginEx = Expression.Assign(Expression.Field(param01, fieldInfo!), param02);
            messageSetter = Expression.Lambda<Action<Exception, string>>(assginEx, param01, param02).Compile();
        }

        public static void ThrowIfNull<T>([NotNull]object? obj, string message) where T : Exception, new() {
            if(obj == null) {
                var e = new T();
                messageSetter(e, message);
                throw e;
            }
        }

        public static void ThrowIfNull<T>([NotNull] object? obj, string format, params object[] args) where T : Exception, new() {
            if (obj == null) {
                var e = new T();
                messageSetter(e, string.Format(format, args));
                throw e;
            }
        }

        public static void ThrowIf<T>(bool condition, string message) where T : Exception, new() {
            if (condition) {
                var e = new T();
                messageSetter(e, message);
                throw e;
            }
        }

        public static void ThrowIf<T>(bool condition, string format, params object[] args) where T : Exception, new() {
            if (condition) {
                var e = new T();
                messageSetter(e, string.Format(format, args));
                throw e;
            }
        }
    }
}
