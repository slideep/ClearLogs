using System;
using System.Collections.Generic;
using ClearLogs.Options;

namespace ClearLogs.Parser
{
    internal abstract class ArgumentParser
    {
        protected ArgumentParser()
        {
            PostParsingState = new List<ParsingError>();
        }

        public abstract ParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options);

        public List<ParsingError> PostParsingState { get; private set; }

        protected void DefineOptionThatViolatesFormat(OptionInfo option)
        {
            PostParsingState.Add(new ParsingError(option.ShortName, option.LongName, true));
        }

        public static ArgumentParser Create(string argument, bool ignoreUnknownArguments)
        {
            if (argument.Equals("-", StringComparison.Ordinal))
                return null;

            if (argument[0] == '-' && argument[1] == '-')
                return new LongOptionParser(ignoreUnknownArguments);

            return argument[0] == '-' ? new OptionGroupParser(ignoreUnknownArguments) : null;
        }

        public static bool IsInputValue(string argument)
        {
            if (argument.Length > 0)
                return argument.Equals("-", StringComparison.Ordinal) || argument[0] != '-';

            return true;
        }
      
        protected static IList<string> GetNextInputValues(IArgumentEnumerator ae)
        {
            IList<string> list = new List<string>();

            while (ae.MoveNext())
            {
                if (IsInputValue(ae.Current))
                    list.Add(ae.Current);
                else
                    break;
            }
            if (!ae.MovePrevious())
                throw new CommandLineParserException();

            return list;
        }

        public static bool CompareShort(string argument, string option, bool caseSensitive)
        {
            return string.Compare(argument, "-" + option, !caseSensitive) == 0;
        }

        public static bool CompareLong(string argument, string option, bool caseSensitive)
        {
            return string.Compare(argument, "--" + option, !caseSensitive) == 0;
        }

        protected static ParserState BooleanToParserState(bool value)
        {
            return BooleanToParserState(value, false);
        }

        protected static ParserState BooleanToParserState(bool value, bool addMoveNextIfTrue)
        {
            if (value && !addMoveNextIfTrue)
                return ParserState.Success;

            if (value)
                return ParserState.Success | ParserState.MoveOnNextElement;

            return ParserState.Failure;
        }

        protected static void EnsureOptionAttributeIsArrayCompatible(OptionInfo option)
        {
            if (!option.IsAttributeArrayCompatible)
                throw new CommandLineParserException();
        }

        protected static void EnsureOptionArrayAttributeIsNotBoundToScalar(OptionInfo option)
        {
            if (!option.IsArray && option.IsAttributeArrayCompatible)
                throw new CommandLineParserException();
        }
    }
}