using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ClearLogs.Attributes;
using ClearLogs.Common;
using ClearLogs.Options;
using ClearLogs.Parser;

namespace ClearLogs.Help
{
    /// <summary>
    /// Models an help text and collects related informations.
    /// You can assign it in place of a <see cref="System.String"/> instance.
    /// </summary>
    public class HelpText
    {
        private const int BuilderCapacity = 128;
        private const int DefaultMaximumLength = 80; // default console width
        private int? _maximumDisplayWidth;
        private string _heading;
        private readonly StringBuilder _preOptionsHelp;
        private StringBuilder _optionsHelp;
        private readonly StringBuilder _postOptionsHelp;
        private const string DefaultRequiredWord = "Required.";

        /// <summary>
        /// Occurs when an option help text is formatted.
        /// </summary>
        public event EventHandler<FormatOptionHelpTextEventArgs> FormatOptionHelpText;

        /// <summary>
        /// Message type enumeration.
        /// </summary>
        public enum MessageEnum : short
        {
            /// <summary>
            /// Parsing error due to a violation of the format of an option value.
            /// </summary>
            ParsingErrorViolatesFormat,
            /// <summary>
            /// Parsing error due to a violation of mandatory option.
            /// </summary>
            ParsingErrorViolatesRequired,
            /// <summary>
            /// Parsing error due to a violation of the mutual exclusiveness of two or more option.
            /// </summary>
            ParsingErrorViolatesExclusiveness
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpText"/> class.
        /// </summary>
        public HelpText()
        {
            _preOptionsHelp = new StringBuilder(BuilderCapacity);
            _postOptionsHelp = new StringBuilder(BuilderCapacity);
            SentenceBuilder = BaseSentenceBuilder.CreateBuiltIn();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpText"/> class 
        /// specifying the sentence builder.
        /// </summary>
        /// <param name="sentenceBuilder">
        /// A <see cref="BaseSentenceBuilder"/> instance.
        /// </param>
        public HelpText(BaseSentenceBuilder sentenceBuilder)
            : this()
        {
            if (sentenceBuilder == null) throw new ArgumentNullException("sentenceBuilder");
            SentenceBuilder = sentenceBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpText"/> class
        /// specifying the sentence builder and heading informations.
        /// </summary>
        /// <param name="sentenceBuilder">
        /// A <see cref="BaseSentenceBuilder"/> instance.
        /// </param>
        /// <param name="heading">
        /// A string with heading information or
        /// an instance of <see cref="HeadingInfo"/>.
        /// </param>
        public HelpText(BaseSentenceBuilder sentenceBuilder, string heading)
            : this(heading)
        {

            SentenceBuilder = sentenceBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpText"/> class
        /// specifying heading and copyright informations.
        /// </summary>
        /// <param name="heading">A string with heading information or
        /// an instance of <see cref="HeadingInfo"/>.</param>
        /// <exception cref="System.ArgumentException">Thrown when one or more parameters <paramref name="heading"/> are null or empty strings.</exception>
        public HelpText(string heading)
            : this()
        {
            _heading = heading;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpText"/> class
        /// specifying heading and copyright informations.
        /// </summary>
        /// <param name="heading">A string with heading information or
        /// an instance of <see cref="HeadingInfo"/>.</param>
        /// <param name="options">The instance that collected command line arguments parsed with <see cref="CommandLineParser"/> class.</param>
        /// <exception cref="System.ArgumentException">Thrown when one or more parameters <paramref name="heading"/> are null or empty strings.</exception>
        public HelpText(string heading, object options)
            : this()
        {
            _heading = heading;
            AddOptions(options);
        }

        public HelpText(BaseSentenceBuilder sentenceBuilder, string heading, object options)
            : this(heading, options)
        {
            SentenceBuilder = sentenceBuilder;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HelpText"/> class using common defaults.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="HelpText"/> class.
        /// </returns>
        /// <param name='options'>
        /// The instance that collected command line arguments parsed with <see cref="CommandLineParser"/> class.
        /// </param>
        public static HelpText AutoBuild(object options)
        {
            return AutoBuild(options, null);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HelpText"/> class using common defaults.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="HelpText"/> class.
        /// </returns>
        /// <param name='options'>
        /// The instance that collected command line arguments parsed with <see cref="CommandLineParser"/> class.
        /// </param>
        /// <param name='err'>
        /// A delegate used to customize the text block for reporting parsing errors.
        /// </param>
        public static HelpText AutoBuild(object options, HandleParsingErrors err)
        {
            var title = ReflectionUtil.GetAttribute<AssemblyTitleAttribute>();
            if (title == null) throw new InvalidOperationException("HelpText::AutoBuild() requires that you define AssemblyTitleAttribute.");
            var version = ReflectionUtil.GetAttribute<AssemblyInformationalVersionAttribute>();
            if (version == null) throw new InvalidOperationException("HelpText::AutoBuild() requires that you define AssemblyInformationalVersionAttribute.");
            var copyright = ReflectionUtil.GetAttribute<AssemblyCopyrightAttribute>();
            if (copyright == null) throw new InvalidOperationException("HelpText::AutoBuild() requires that you define AssemblyCopyrightAttribute.");

            var auto = new HelpText
            {
                Heading = new HeadingInfo(Path.GetFileNameWithoutExtension(title.Title), version.InformationalVersion),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };

            if (err != null)
            {
                var typedTarget = options as CommandLineOptionsBase;
                if (typedTarget != null)
                {
                    err(auto);
                }
            }

            auto.AddOptions(options);

            return auto;
        }

        public static void DefaultParsingErrorsHandler(CommandLineOptionsBase options, HelpText current)
        {
            if (options.InternalLastPostParsingState.Errors.Count <= 0)
                return;

            var errors = current.RenderParsingErrorsText(options, 2); // indent with two spaces
            if (string.IsNullOrEmpty(errors))
                return;

            current.AddPreOptionsLine(string.Concat(Environment.NewLine, current.SentenceBuilder.ErrorsHeadingText));

            var lines = errors.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines) { current.AddPreOptionsLine(line); }
        }

        /// <summary>
        /// Sets the heading information string.
        /// You can directly assign a <see cref="HeadingInfo"/> instance.
        /// </summary>
        public string Heading
        {
            set
            {
                _heading = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of the display.  This determines word wrap when displaying the text.
        /// </summary>
        /// <value>The maximum width of the display.</value>
        public int MaximumDisplayWidth
        {
            get { return _maximumDisplayWidth.HasValue ? _maximumDisplayWidth.Value : DefaultMaximumLength; }
            set { _maximumDisplayWidth = value; }
        }

        /// <summary>
        /// Gets or sets the format of options for adding or removing dashes.
        /// It modifies behavior of <see>
        ///                           <cref>AddOptions</cref>
        ///                         </see> method.
        /// </summary>
        public bool AddDashesToOption { get; set; }

        /// <summary>
        /// Gets or sets whether to add an additional line after the description of the option.
        /// </summary>
        public bool AdditionalNewLineAfterOption { get; set; }

        public BaseSentenceBuilder SentenceBuilder { get; private set; }

        /// <summary>
        /// Adds a text line after copyright and before options usage informations.
        /// </summary>
        /// <param name="value">A <see cref="System.String"/> instance.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="value"/> is null or empty string.</exception>
        public void AddPreOptionsLine(string value)
        {
            AddPreOptionsLine(value, MaximumDisplayWidth);
        }

        private void AddPreOptionsLine(string value, int maximumLength)
        {
            AddLine(_preOptionsHelp, value, maximumLength);
        }

        /// <summary>
        /// Adds a text line at the bottom, after options usage informations.
        /// </summary>
        /// <param name="value">A <see cref="System.String"/> instance.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="value"/> is null or empty string.</exception>
        public void AddPostOptionsLine(string value)
        {
            AddLine(_postOptionsHelp, value);
        }

        /// <summary>
        /// Adds a text block with options usage informations.
        /// </summary>
        /// <param name="options">The instance that collected command line arguments parsed with <see cref="CommandLineParser"/> class.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="options"/> is null.</exception>
        public void AddOptions(object options)
        {
            AddOptions(options, DefaultRequiredWord);
        }

        /// <summary>
        /// Adds a text block with options usage informations.
        /// </summary>
        /// <param name="options">The instance that collected command line arguments parsed with the <see cref="CommandLineParser"/> class.</param>
        /// <param name="requiredWord">The word to use when the option is required.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="options"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="requiredWord"/> is null or empty string.</exception>
        public void AddOptions(object options, string requiredWord)
        {
            AddOptions(options, requiredWord, MaximumDisplayWidth);
        }

        /// <summary>
        /// Adds a text block with options usage informations.
        /// </summary>
        /// <param name="options">The instance that collected command line arguments parsed with the <see cref="CommandLineParser"/> class.</param>
        /// <param name="requiredWord">The word to use when the option is required.</param>
        /// <param name="maximumLength">The maximum length of the help documentation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="options"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="requiredWord"/> is null or empty string.</exception>
        public void AddOptions(object options, string requiredWord, int maximumLength)
        {
            var optionList = ReflectionUtil.RetrievePropertyAttributeList<BaseOptionAttribute>(options);
            var optionHelp = ReflectionUtil.RetrieveMethodAttributeOnly<HelpOptionAttribute>(options);

            if (optionHelp != null)
            {
                optionList.Add(optionHelp);
            }

            if (optionList.Count == 0)
            {
                return;
            }

            int maxLength = GetMaxLength(optionList);
            _optionsHelp = new StringBuilder(BuilderCapacity);
            int remainingSpace = maximumLength - (maxLength + 6);
            foreach (BaseOptionAttribute option in optionList)
            {
                AddOption(requiredWord, maxLength, option, remainingSpace);
            }
        }

        /// <summary>
        /// Builds a string that contains a parsing error message.
        /// </summary>
        /// <param name="options">
        /// An options target <see cref="CommandLineOptionsBase"/> instance that collected command line arguments parsed with the <see cref="CommandLineParser"/> class.
        /// </param>
        /// <param name="indent"> </param>
        /// <returns>
        /// The <see cref="System.String"/> that contains the parsing error message.
        /// </returns>
        public string RenderParsingErrorsText(CommandLineOptionsBase options, int indent)
        {
            if (options.InternalLastPostParsingState.Errors.Count == 0)
            {
                return string.Empty;
            }
            var text = new StringBuilder();
            foreach (var e in options.InternalLastPostParsingState.Errors)
            {
                var line = new StringBuilder();
                line.Append(StringUtil.Spaces(indent));
                if (!string.IsNullOrEmpty(e.BadOption.ShortName))
                {
                    line.Append('-');
                    line.Append(e.BadOption.ShortName);
                    if (!string.IsNullOrEmpty(e.BadOption.LongName)) line.Append('/');
                }
                if (!string.IsNullOrEmpty(e.BadOption.LongName))
                {
                    line.Append("--");
                    line.Append(e.BadOption.LongName);
                }
                line.Append(" ");
                line.Append(e.ViolatesRequired
                                ? SentenceBuilder.RequiredOptionMissingText
                                : SentenceBuilder.OptionWord);
                if (e.ViolatesFormat)
                {
                    line.Append(" ");
                    line.Append(SentenceBuilder.ViolatesFormatText);
                }
                if (e.ViolatesMutualExclusiveness)
                {
                    if (e.ViolatesFormat || e.ViolatesRequired)
                    {
                        line.Append(" ");
                        line.Append(SentenceBuilder.AndWord);
                    }
                    line.Append(" ");
                    line.Append(SentenceBuilder.ViolatesMutualExclusivenessText);
                }
                line.Append('.');
                text.AppendLine(line.ToString());
            }
            return text.ToString();
        }

        private void AddOption(string requiredWord, int maxLength, BaseOptionAttribute option, int widthOfHelpText)
        {
            _optionsHelp.Append("  ");
            var optionName = new StringBuilder(maxLength);
            if (option.HasShortName)
            {
                if (AddDashesToOption)
                {
                    optionName.Append('-');
                }

                optionName.AppendFormat("{0}", option.ShortName);

                if (option.HasLongName)
                {
                    optionName.Append(", ");
                }
            }

            if (option.HasLongName)
            {
                if (AddDashesToOption)
                {
                    optionName.Append("--");
                }

                optionName.AppendFormat("{0}", option.LongName);
            }

            _optionsHelp.Append(optionName.Length < maxLength
                                    ? optionName.ToString().PadRight(maxLength)
                                    : optionName.ToString());

            _optionsHelp.Append("    ");
            if (option.Required)
            {
                option.HelpText = String.Format("{0} ", requiredWord) + option.HelpText;
            }

            var e = new FormatOptionHelpTextEventArgs(option);
            OnFormatOptionHelpText(e);
            option.HelpText = e.Option.HelpText;

            if (!string.IsNullOrEmpty(option.HelpText))
            {
                do
                {
                    var wordBuffer = 0;
                    var words = option.HelpText.Split(new[] { ' ' });
                    for (var i = 0; i < words.Length; i++)
                    {
                        if (words[i].Length < (widthOfHelpText - wordBuffer))
                        {
                            _optionsHelp.Append(words[i]);
                            wordBuffer += words[i].Length;
                            if ((widthOfHelpText - wordBuffer) > 1 && i != words.Length - 1)
                            {
                                _optionsHelp.Append(" ");
                                wordBuffer++;
                            }
                        }
                        else if (words[i].Length >= widthOfHelpText && wordBuffer == 0)
                        {
                            _optionsHelp.Append(words[i].Substring(
                                0,
                                widthOfHelpText
                                                     ));
                            wordBuffer = widthOfHelpText;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    option.HelpText = option.HelpText.Substring(Math.Min(
                        wordBuffer,
                        option.HelpText.Length
                                                                     ))
                        .Trim();
                    if (option.HelpText.Length > 0)
                    {
                        _optionsHelp.Append(Environment.NewLine);
                        _optionsHelp.Append(new string(' ', maxLength + 6));
                    }
                } while (option.HelpText.Length > widthOfHelpText);
            }
            _optionsHelp.Append(option.HelpText);
            _optionsHelp.Append(Environment.NewLine);
            if (AdditionalNewLineAfterOption)
                _optionsHelp.Append(Environment.NewLine);
        }

        /// <summary>
        /// Returns the help informations as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The <see cref="System.String"/> that contains the help informations.</returns>
        public override string ToString()
        {
            const int extraLength = 10;
            var builder = new StringBuilder(GetLength(_heading) +
                                             GetLength(_preOptionsHelp) + GetLength(_optionsHelp) + extraLength
                );

            builder.Append(_heading);

            if (_preOptionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(_preOptionsHelp);
            }
            if (_optionsHelp != null && _optionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(Environment.NewLine);
                builder.Append(_optionsHelp);
            }
            if (_postOptionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(_postOptionsHelp);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the help informations to a <see cref="System.String"/>.
        /// </summary>
        /// <param name="info">This <see cref="HelpText"/> instance.</param>
        /// <returns>The <see cref="System.String"/> that contains the help informations.</returns>
        public static implicit operator string(HelpText info)
        {
            return info.ToString();
        }

        private void AddLine(StringBuilder builder, string value)
        {

            AddLine(builder, value, MaximumDisplayWidth);
        }

        private static void AddLine(StringBuilder builder, string value, int maximumLength)
        {

            if (builder.Length > 0)
            {
                builder.Append(Environment.NewLine);
            }
            do
            {
                var wordBuffer = 0;
                var words = value.Split(new[] { ' ' });
                for (var i = 0; i < words.Length; i++)
                {
                    if (words[i].Length < (maximumLength - wordBuffer))
                    {
                        builder.Append(words[i]);
                        wordBuffer += words[i].Length;
                        if ((maximumLength - wordBuffer) > 1 && i != words.Length - 1)
                        {
                            builder.Append(" ");
                            wordBuffer++;
                        }
                    }
                    else if (words[i].Length >= maximumLength && wordBuffer == 0)
                    {
                        builder.Append(words[i].Substring(0, maximumLength));
                        wordBuffer = maximumLength;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                value = value.Substring(Math.Min(wordBuffer, value.Length));
                if (value.Length > 0)
                {
                    builder.Append(Environment.NewLine);
                }
            } while (value.Length > maximumLength);
            builder.Append(value);
        }

        private static int GetLength(string value)
        {
            return value == null ? 0 : value.Length;
        }

        private static int GetLength(StringBuilder value)
        {
            return value == null ? 0 : value.Length;
        }

        private int GetMaxLength(IEnumerable<BaseOptionAttribute> optionList)
        {
            var length = 0;
            foreach (var option in optionList)
            {
                var optionLength = 0;
                var hasShort = option.HasShortName;
                var hasLong = option.HasLongName;
                if (hasShort)
                {
                    optionLength += option.ShortName.Length;
                    if (AddDashesToOption)
                        ++optionLength;
                }
                if (hasLong)
                {
                    optionLength += option.LongName.Length;
                    if (AddDashesToOption)
                        optionLength += 2;
                }
                if (hasShort && hasLong)
                {
                    optionLength += 2;
                }
                length = Math.Max(length, optionLength);
            }
            return length;
        }

        protected virtual void OnFormatOptionHelpText(FormatOptionHelpTextEventArgs e)
        {
            var handler = FormatOptionHelpText;

            if (handler != null)
                handler(this, e);
        }
    }
}