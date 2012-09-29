using ClearLogs.Options;

namespace ClearLogs.Parser
{
    internal sealed class LongOptionParser : ArgumentParser
    {
        private readonly bool _ignoreUnkwnownArguments;

        public LongOptionParser(bool ignoreUnkwnownArguments)
        {
            _ignoreUnkwnownArguments = ignoreUnkwnownArguments;
        }

        public override ParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options)
        {
            var parts = argumentEnumerator.Current.Substring(2).Split(new[] { '=' }, 2);
            var option = map[parts[0]];
            bool valueSetting;

            if (option == null)
                return _ignoreUnkwnownArguments ? ParserState.MoveOnNextElement : ParserState.Failure;

            option.IsDefined = true;

            EnsureOptionArrayAttributeIsNotBoundToScalar(option);

            if (!option.IsBoolean)
            {
                if (parts.Length == 1 && (argumentEnumerator.IsLast || !IsInputValue(argumentEnumerator.Next)))
                    return ParserState.Failure;

                if (parts.Length == 2)
                {
                    if (!option.IsArray)
                    {
                        valueSetting = option.SetValue(parts[1], options);
                        if (!valueSetting)
                            DefineOptionThatViolatesFormat(option);

                        return BooleanToParserState(valueSetting);
                    }

                    EnsureOptionAttributeIsArrayCompatible(option);

                    var items = GetNextInputValues(argumentEnumerator);
                    items.Insert(0, parts[1]);

                    valueSetting = option.SetValue(items, options);
                    if (!valueSetting)
                        DefineOptionThatViolatesFormat(option);

                    return BooleanToParserState(valueSetting);
                }
                else
                {
                    if (!option.IsArray)
                    {
                        valueSetting = option.SetValue(argumentEnumerator.Next, options);
                        if (!valueSetting)
                            DefineOptionThatViolatesFormat(option);

                        return BooleanToParserState(valueSetting, true);
                    }

                    EnsureOptionAttributeIsArrayCompatible(option);

                    var items = GetNextInputValues(argumentEnumerator);

                    valueSetting = option.SetValue(items, options);
                    if (!valueSetting)
                        DefineOptionThatViolatesFormat(option);

                    return BooleanToParserState(valueSetting);
                }
            }

            if (parts.Length == 2)
                return ParserState.Failure;

            valueSetting = option.SetValue(true, options);
            if (!valueSetting)
                DefineOptionThatViolatesFormat(option);

            return BooleanToParserState(valueSetting);
        }
    }
}