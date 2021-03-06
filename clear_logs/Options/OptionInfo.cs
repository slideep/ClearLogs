using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using ClearLogs.Attributes;
using ClearLogs.Common;
using ClearLogs.Parser;

namespace ClearLogs.Options
{
    [DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
    internal sealed class OptionInfo
    {
        private readonly OptionAttribute _attribute;
        private readonly object _defaultValue;
        private readonly bool _hasDefaultValue;
        private readonly PropertyInfo _property;
        private readonly object _setValueLock = new object();

        private OptionInfo(OptionAttribute attribute, PropertyInfo property)
        {
            if (attribute != null)
            {
                Required = attribute.Required;
                HelpText = attribute.HelpText;
                ShortName = attribute.ShortName;
                LongName = attribute.LongName;
                MutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
                _defaultValue = attribute.DefaultValue;
                _hasDefaultValue = attribute.HasDefaultValue;
                _attribute = attribute;
            }
            else
            {
                throw new ArgumentNullException("attribute", "The attribute is mandatory");
            }

            if (property != null)
                _property = property;
            else
                throw new ArgumentNullException("property", "The property is mandatory");
        }

        public string ShortName { get; private set; }

        public string LongName { get; private set; }

        public string MutuallyExclusiveSet { get; }

        public bool Required { get; }

        private string HelpText { get; }

        public bool IsBoolean => _property.PropertyType == typeof(bool);

        public bool IsArray => _property.PropertyType.IsArray;

        public bool IsAttributeArrayCompatible => _attribute is OptionArrayAttribute;

        public bool IsDefined { get; set; }

        public bool HasBothNames => ShortName != null && LongName != null;

        public static OptionMap CreateMap(object target, CommandLineParserSettings settings)
        {
            var list = ReflectionUtil.RetrievePropertyList<OptionAttribute>(target);
            if (list != null)
            {
                var map = new OptionMap(list.Count, settings);

                foreach (var (propertyInfo, optionAttribute) in list.Where(pair => pair.Item2 != null))
                    map[optionAttribute.UniqueName] = new OptionInfo(optionAttribute, propertyInfo);

                map.RawOptions = target;

                return map;
            }

            return null;
        }

        public bool SetValue(string value, object options)
        {
            if (_attribute is OptionListAttribute)
                return SetValueList(value, options);

            return ReflectionUtil.IsNullableType(_property.PropertyType)
                ? SetNullableValue(value, options)
                : SetValueScalar(value, options);
        }

        public bool SetValue(IList<string> values, object options)
        {
            var elementType = _property.PropertyType.GetElementType();
            if (elementType != null)
            {
                var array = Array.CreateInstance(elementType, values.Count);

                for (var i = 0; i < array.Length; i++)
                    try
                    {
                        lock (_setValueLock)
                        {
                            array.SetValue(
                                Convert.ChangeType(values[i], elementType, Thread.CurrentThread.CurrentCulture), i);
                            _property.SetValue(options, array, null);
                        }
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
            }

            return true;
        }

        private bool SetValueScalar(string value, object options)
        {
            try
            {
                if (_property.PropertyType.IsEnum)
                    lock (_setValueLock)
                    {
                        _property.SetValue(options, Enum.Parse(_property.PropertyType, value, true), null);
                    }
                else
                    lock (_setValueLock)
                    {
                        _property.SetValue(options,
                            Convert.ChangeType(value, _property.PropertyType, Thread.CurrentThread.CurrentCulture),
                            null);
                    }
            }
            catch (InvalidCastException) // Convert.ChangeType
            {
                return false;
            }
            catch (FormatException) // Convert.ChangeType
            {
                return false;
            }
            catch (ArgumentException) // Enum.Parse
            {
                return false;
            }

            return true;
        }

        private bool SetNullableValue(string value, object options)
        {
            var nc = new NullableConverter(_property.PropertyType);

            try
            {
                lock (_setValueLock)
                {
                    _property.SetValue(options, nc.ConvertFromString(null, Thread.CurrentThread.CurrentCulture, value),
                        null);
                }
            }
            // the FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException,
            // so we've catch directly Exception
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool SetValue(bool value, object options)
        {
            lock (_setValueLock)
            {
                _property.SetValue(options, value, null);

                return true;
            }
        }

        private bool SetValueList(string value, object options)
        {
            lock (_setValueLock)
            {
                _property.SetValue(options, new List<string>(), null);

                var fieldRef = (IList<string>) _property.GetValue(options, null);
                var values = value.Split(((OptionListAttribute) _attribute).Separator);

                foreach (var t in values) fieldRef.Add(t);

                return true;
            }
        }

        public void SetDefault(object options)
        {
            if (!_hasDefaultValue) return;
            lock (_setValueLock)
            {
                try
                {
                    _property.SetValue(options, _defaultValue, null);
                }
                catch (Exception e)
                {
                    throw new CommandLineParserException("Bad default value.", e);
                }
            }
        }
    }
}