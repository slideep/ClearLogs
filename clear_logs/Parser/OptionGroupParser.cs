using System.Collections.Generic;
using ClearLogs.Options;

namespace ClearLogs.Parser
{
    internal sealed class OptionGroupParser : ArgumentParser
    {
        private readonly bool _ignoreUnkwnownArguments;

        public OptionGroupParser(bool ignoreUnkwnownArguments)
        {
            _ignoreUnkwnownArguments = ignoreUnkwnownArguments;
        }

        public override ParserState Parse(IArgumentEnumerator argumentEnumerator, OptionMap map, object options)
        {
            IArgumentEnumerator group = new OneCharStringEnumerator(argumentEnumerator.Current.Substring(1));
            while (group.MoveNext())
            {
                var option = map[group.Current];
                if (option == null)
                    return _ignoreUnkwnownArguments ? ParserState.MoveOnNextElement : ParserState.Failure;

                option.IsDefined = true;

                EnsureOptionArrayAttributeIsNotBoundToScalar(option);

                if (!option.IsBoolean)
                {
                    if (argumentEnumerator.IsLast && group.IsLast)
                        return ParserState.Failure;

                    bool valueSetting;
                    IList<string> items;
                    if (!group.IsLast)
                    {
                        if (!option.IsArray)
                        {
                            valueSetting = option.SetValue(group.GetRemainingFromNext(), options);
                            if (!valueSetting)
                                DefineOptionThatViolatesFormat(option);

                            return BooleanToParserState(valueSetting);
                        }

                        EnsureOptionAttributeIsArrayCompatible(option);

                        items = GetNextInputValues(argumentEnumerator);
                        items.Insert(0, @group.GetRemainingFromNext());

                        valueSetting = option.SetValue(items, options);
                        if (!valueSetting)
                            DefineOptionThatViolatesFormat(option);

                        return BooleanToParserState(valueSetting, true);
                    }

                    if (!argumentEnumerator.IsLast && !IsInputValue(argumentEnumerator.Next))
                        return ParserState.Failure;
                    if (!option.IsArray)
                    {
                        valueSetting = option.SetValue(argumentEnumerator.Next, options);
                        if (!valueSetting)
                            DefineOptionThatViolatesFormat(option);

                        return BooleanToParserState(valueSetting, true);
                    }

                    EnsureOptionAttributeIsArrayCompatible(option);

                    items = GetNextInputValues(argumentEnumerator);

                    valueSetting = option.SetValue(items, options);
                    if (!valueSetting)
                        DefineOptionThatViolatesFormat(option);

                    return BooleanToParserState(valueSetting);
                }

                if (!@group.IsLast && map[@group.Next] == null)
                    return ParserState.Failure;

                if (!option.SetValue(true, options))
                    return ParserState.Failure;
            }

            return ParserState.Success;
        }
    }
}