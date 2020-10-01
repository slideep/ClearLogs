using System;
using System.Text;

namespace ClearLogs.Help
{
    /// <summary>
    ///     Models the heading informations part of an help text.
    ///     You can assign it where you assign any <see cref="System.String" /> instance.
    /// </summary>
    public class HeadingInfo
    {
        private readonly string _programName;
        private readonly string _version;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HeadingInfo" /> class
        ///     specifying program name and version.
        /// </summary>
        /// <param name="programName">The name of the program.</param>
        /// <param name="version">The version of the program.</param>
        /// <exception cref="System.ArgumentException">
        ///     Thrown when parameter <paramref name="programName" /> is null or empty
        ///     string.
        /// </exception>
        public HeadingInfo(string programName, string version)
        {
            _programName = programName;
            _version = version;
        }

        /// <summary>
        ///     Returns the heading informations as a <see cref="System.String" />.
        /// </summary>
        /// <returns>The <see cref="System.String" /> that contains the heading informations.</returns>
        public override string ToString()
        {
            var isVersionNull = string.IsNullOrWhiteSpace(_version);
            var builder = new StringBuilder(_programName.Length +
                                            (!isVersionNull ? _version.Length + 1 : 0)
            );
            builder.Append(_programName);
            if (!isVersionNull)
            {
                builder.Append(' ');
                builder.Append(_version);
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Converts the heading informations to a <see cref="System.String" />.
        /// </summary>
        /// <param name="info">This <see cref="HeadingInfo" /> instance.</param>
        /// <returns>The <see cref="System.String" /> that contains the heading informations.</returns>
        public static implicit operator string(HeadingInfo info)
        {
            if (info == null) throw new ArgumentNullException("info");
            return info.ToString();
        }
    }
}