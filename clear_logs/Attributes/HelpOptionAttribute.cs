using System;
using System.Reflection;
using ClearLogs.Common;

namespace ClearLogs.Attributes
{
    /// <summary>
    /// Indicates the instance method that must be invoked when it becomes necessary show your help screen.
    /// The method signature is an instance method with no parameters and <see cref="System.String"/>
    /// return value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property,
        AllowMultiple=false,
        Inherited=true)]
    public sealed class HelpOptionAttribute : BaseOptionAttribute
    {
        private const string DefaultHelpText = "Display this help screen.";

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpOptionAttribute"/> class.
        /// </summary>
        public HelpOptionAttribute()
            : this(null, "help")
        {
            HelpText = DefaultHelpText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpOptionAttribute"/> class.
        /// Allows you to define short and long option names.
        /// </summary>
        /// <param name="shortName">The short name of the option or null if not used.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public HelpOptionAttribute(string shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
            HelpText = DefaultHelpText;
        }

        /// <summary>
        /// Returns always false for this kind of option.
        /// This behaviour can't be changed by design; if you try set <see cref="Required"/>
        /// an <see cref="System.InvalidOperationException"/> will be thrown.
        /// </summary>
        public override bool Required
        {
            get { return false; }
            set { throw new InvalidOperationException(); }
        }

        internal static void InvokeMethod(object target,
                                          Pair<MethodInfo, HelpOptionAttribute> pair, out string text)
        {
            text = null;

            var method = pair.Left;
            if (!CheckMethodSignature(method))
                throw new MemberAccessException();

            text = (string)method.Invoke(target, null);
        }

        private static bool CheckMethodSignature(MethodInfo value)
        {
            return value.ReturnType == typeof(string) && value.GetParameters().Length == 0;
        }
    }
}