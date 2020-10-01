using System;
using System.Collections.Generic;
using System.IO;
using ClearLogs.Attributes;
using ClearLogs.Common;
using ClearLogs.Options;

namespace ClearLogs.Parser
{
    /// <summary>
    /// Provides methods to parse command line arguments.
    /// Default implementation for <see cref="ICommandLineParser"/>.
    /// </summary>
    public class CommandLineParser : ICommandLineParser
    {
        private readonly CommandLineParserSettings _settings;

        // special constructor for singleton instance, parameter ignored
        private CommandLineParser(bool singleton)
        {
            _settings = new CommandLineParserSettings(false, false, Console.Error);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineParser"/> class.
        /// </summary>
        public CommandLineParser()
        {
            _settings = new CommandLineParserSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineParser"/> class,
        /// configurable with a <see cref="CommandLineParserSettings"/> object.
        /// </summary>
        /// <param name="settings">The <see cref="CommandLineParserSettings"/> object is used to configure
        /// aspects and behaviors of the parser.</param>
        public CommandLineParser(CommandLineParserSettings settings)
        {
            _settings = settings;
        }

        static CommandLineParser()
        {
            Default = new CommandLineParser(true);
        }

        /// <summary>
        /// Singleton instance created with basic defaults.
        /// </summary>
        public static ICommandLineParser Default { get; private set; }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="options">An object's instance used to receive values.
        /// Parsing rules are defined using <see cref="BaseOptionAttribute"/> derived types.</param>
        /// <returns>True if parsing process succeed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArguments(string[] args, object options)
        {
            return ParseArguments(args, options, _settings.HelpWriter);
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes.
        /// This overload allows you to specify a <see cref="System.IO.TextWriter"/> derived instance for write text messages.         
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="options">An object's instance used to receive values.
        /// Parsing rules are defined using <see cref="BaseOptionAttribute"/> derived types.</param>
        /// <param name="helpWriter">Any instance derived from <see cref="System.IO.TextWriter"/>,
        /// usually <see cref="System.Console.Error"/>. Setting this argument to null, will disable help screen.</param>
        /// <returns>True if parsing process succeed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArguments(string[] args, object options, TextWriter helpWriter)
        {
            var pair = ReflectionUtil.RetrieveMethod<HelpOptionAttribute>(options);

            if (helpWriter != null)
            {
                if (ParseHelp(args, pair.Item2) || !DoParseArguments(args, options))
                {
                    string helpText;
                    HelpOptionAttribute.InvokeMethod(options, pair, out helpText);
                    helpWriter.Write(helpText);
                    return false;
                }
                return true;
            }

            return DoParseArguments(args, options);
        }

        private bool DoParseArguments(string[] args, object options)
        {
            var hadError = false;
            var optionMap = OptionInfo.CreateMap(options, _settings);
            optionMap.SetDefaults();
            var target = new TargetWrapper(options);

            using (var arguments = new StringArrayEnumerator(args))
            {
                while (arguments.MoveNext())
                {
                    var argument = arguments.Current;

                    if (string.IsNullOrWhiteSpace(argument))
                        continue;

                    var parser = ArgumentParser.Create(argument, _settings.IgnoreUnknownArguments);
                    if (parser != null)
                    {
                        var result = parser.Parse(arguments, optionMap, options);
                        if ((result & ParserState.Failure) == ParserState.Failure)
                        {
                            SetPostParsingStateIfNeeded(options, parser.PostParsingState);
                            hadError = true;
                            continue;
                        }

                        if ((result & ParserState.MoveOnNextElement) == ParserState.MoveOnNextElement)
                            arguments.MoveNext();
                    }
                    else if (target.IsValueListDefined)
                    {
                        if (!target.AddValueItemIfAllowed(argument))
                        {
                            hadError = true;
                        }
                    }
                }
            }

            hadError |= !optionMap.EnforceRules();

            return !hadError;
        }

        private bool ParseHelp(IEnumerable<string> args, HelpOptionAttribute helpOption)
        {
            var caseSensitive = _settings.CaseSensitive;

            foreach (var t in args)
            {
                if (!string.IsNullOrWhiteSpace(helpOption.ShortName))
                {
                    if (ArgumentParser.CompareShort(t, helpOption.ShortName, caseSensitive))
                        return true;
                }

                if (string.IsNullOrWhiteSpace(helpOption.LongName)) 
                    continue;

                if (ArgumentParser.CompareLong(t, helpOption.LongName, caseSensitive))
                    return true;
            }

            return false;
        }

        private static void SetPostParsingStateIfNeeded(object options, IEnumerable<ParsingError> state)
        {
            if (options is CommandLineOptionsBase commandLineOptionsBase)

                foreach (var parsingError in state)
                {
                    commandLineOptionsBase.InternalLastPostParsingState.AddError(parsingError);  
                }
        }
    }
}