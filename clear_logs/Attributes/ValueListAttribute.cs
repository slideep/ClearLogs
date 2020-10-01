using System;
using System.Collections.Generic;
using System.Reflection;
using ClearLogs.Common;

namespace ClearLogs.Attributes
{
    /// <summary>
    ///     Models a list of command line arguments that are not options.
    ///     Must be applied to a field compatible with an <see cref="System.Collections.Generic.IList&lt;T&gt;" /> interface
    ///     of <see cref="System.String" /> instances.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ValueListAttribute : Attribute
    {
        private ValueListAttribute()
        {
            MaximumElements = -1;
        }

        /// <summary>
        ///     Gets or sets the maximum element allow for the list managed by <see cref="ValueListAttribute" /> type.
        ///     If lesser than 0, no upper bound is fixed.
        ///     If equal to 0, no elements are allowed.
        /// </summary>
        public int MaximumElements { get; set; }

        private Type ConcreteType { get; }

        internal static IList<string> GetReference(object target)
        {
            var property = GetProperty(target, out var concreteType);

            if (property == null || concreteType == null)
                return null;

            property.SetValue(target, Activator.CreateInstance(concreteType), null);

            return (IList<string>) property.GetValue(target, null);
        }

        internal static ValueListAttribute GetAttribute(object target)
        {
            var list = ReflectionUtil.RetrievePropertyList<ValueListAttribute>(target);
            if (list == null || list.Count == 0)
                return null;

            if (list.Count > 1)
                throw new InvalidOperationException();

            var pairZero = list[0];

            return pairZero.Item2;
        }

        private static PropertyInfo GetProperty(object target, out Type concreteType)
        {
            concreteType = null;

            var list = ReflectionUtil.RetrievePropertyList<ValueListAttribute>(target);
            if (list == null || list.Count == 0)
                return null;

            if (list.Count > 1)
                throw new InvalidOperationException();

            var (propertyInfo, valueListAttribute) = list[0];
            concreteType = valueListAttribute.ConcreteType;

            return propertyInfo;
        }
    }
}