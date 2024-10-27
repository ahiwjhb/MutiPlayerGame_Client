#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.MVVM.Extension
{
    public static class TypeEx
    {
        public static bool IsDeriveFromGenericType(this Type type, Type genericType) {
            if (genericType.IsGenericType == false && type.IsGenericType == false) {
                return genericType.IsAssignableFrom(type);
            }

            genericType = genericType.GetGenericTypeDefinition();
            if (genericType.IsInterface) {
                return type.GetInterfaces().Append(type).Where(t => t.IsGenericType).Any(t =>  t.GetGenericTypeDefinition() == genericType);
            }

            for (var t = type; t != typeof(object); t = t.BaseType) {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericType) {
                    return true;
                }
            }

            return false;
        }

        //public static IEnumerable<Type> GetGenericInerfaces(this Type type, Type genericType) {
        //    IEnumerable<Type> types = type.GetInterfaces();
        //    if (type.IsInterface) {
        //        types.Append(type);
        //    }
        //    return types.Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == genericType);
        //}

        //public static Type GetGenericType(this Type type, Type genericType) {
        //    for(Type t = type; t != typeof(object); t = t.BaseType) {
        //        if(t.IsGenericType && t.GetGenericTypeDefinition() == genericType) {
        //            return t;
        //        }
        //    }
        //    return null;
        //}
    }
}
