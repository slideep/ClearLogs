using System;
using System.Collections.Generic;
using System.Reflection;

namespace ClearLogs.Common
{
    internal static class ReflectionUtil
    {
        public static IList<(PropertyInfo, TAttribute)> RetrievePropertyList<TAttribute>(object target)
            where TAttribute : Attribute
        {
            var list = new List<(PropertyInfo, TAttribute)>();
            if (target != null)
            {
                var propertiesInfo = target.GetType().GetProperties();

                foreach (var property in propertiesInfo)
                {
                    if (!property.CanRead || !property.CanWrite)
                        continue;

                    var setMethod = property.GetSetMethod();
                    if (setMethod == null || setMethod.IsStatic)
                        continue;

                    var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                    if (attribute != null)
                        list.Add((property, (TAttribute) attribute));
                }
            }

            return list;
        }

        public static (MethodInfo, TAttribute) RetrieveMethod<TAttribute>(object target)
            where TAttribute : Attribute
        {
            var info = target.GetType().GetMethods();

            foreach (var method in info)
            {
                if (method.IsStatic)
                    continue;

                var attribute =
                    Attribute.GetCustomAttribute(method, typeof(TAttribute), false);
                if (attribute != null)
                    return (method, (TAttribute) attribute);
            }

            return (null, null);
        }

        public static TAttribute RetrieveMethodAttributeOnly<TAttribute>(object target)
            where TAttribute : Attribute
        {
            var info = target.GetType().GetMethods();

            foreach (var method in info)
            {
                if (method.IsStatic)
                    continue;

                var attribute =
                    Attribute.GetCustomAttribute(method, typeof(TAttribute), false);
                if (attribute != null)
                    return (TAttribute) attribute;
            }

            return null;
        }

        public static IList<TAttribute> RetrievePropertyAttributeList<TAttribute>(object target)
            where TAttribute : Attribute
        {
            IList<TAttribute> list = new List<TAttribute>();
            var info = target.GetType().GetProperties();

            foreach (var property in info)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var setMethod = property.GetSetMethod();
                if (setMethod == null || setMethod.IsStatic)
                    continue;

                var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                if (attribute != null)
                    list.Add((TAttribute) attribute);
            }

            return list;
        }

        public static TAttribute GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            var a = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(TAttribute), false);
            if (a.Length <= 0) return null;
            return (TAttribute) a[0];
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}