using System;
using System.Collections.Generic;
using System.Linq;
using ClearLogs.Parser;

namespace ClearLogs.Options
{
    internal sealed class OptionMap
    {
        sealed class MutuallyExclusiveInfo
        {
            public MutuallyExclusiveInfo(OptionInfo option)
            {
                BadOption = option; 
            }
            
            public OptionInfo BadOption { get; private set; }
            
            public void IncrementOccurrence() { ++Occurrence; }

            public int Occurrence { get; private set; }
        }
        
        readonly CommandLineParserSettings _settings;
        readonly Dictionary<string, string> _names;
        readonly Dictionary<string, OptionInfo> _map;
        readonly Dictionary<string, MutuallyExclusiveInfo> _mutuallyExclusiveSetMap;

        public OptionMap(int capacity, CommandLineParserSettings settings)
        {
            _settings = settings;

            IEqualityComparer<string> comparer = _settings.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

            _names = new Dictionary<string, string>(capacity, comparer);
            _map = new Dictionary<string, OptionInfo>(capacity * 2, comparer);

            if (_settings.MutuallyExclusive)
            {
                _mutuallyExclusiveSetMap = new Dictionary<string, MutuallyExclusiveInfo>(capacity, StringComparer.OrdinalIgnoreCase);
            }
        }

        public OptionInfo this[string key]
        {
            get
            {
                OptionInfo option = null;

                if (_map.ContainsKey(key))
                    option = _map[key];
                else
                {
                    if (_names.ContainsKey(key))
                    {
                        var optionKey = _names[key];
                        option = _map[optionKey];
                    }
                }

                return option;
            }
            set
            {
                _map[key] = value;

                if (value.HasBothNames)
                    _names[value.LongName] = value.ShortName;
            }
        }

        internal object RawOptions { private get; set; }

        public bool EnforceRules()
        {
            return EnforceMutuallyExclusiveMap() && EnforceRequiredRule();
        }

        public void SetDefaults()
        {
            foreach (var option in _map.Values)
            {
                option.SetDefault(RawOptions);
            }
        }

        private bool EnforceRequiredRule()
        {
            foreach (var option in _map.Values.Where(option => option.Required && !option.IsDefined))
            {
                BuildAndSetPostParsingStateIfNeeded(RawOptions, option, true, null);
                return false;
            }
            return true;
        }

        private bool EnforceMutuallyExclusiveMap()
        {
            if (!_settings.MutuallyExclusive)
                return true;

            foreach (var option in _map.Values.Where(option => option.IsDefined && option.MutuallyExclusiveSet != null))
            {
                BuildMutuallyExclusiveMap(option);
            }

            foreach (var info in _mutuallyExclusiveSetMap.Values.Where(info => info.Occurrence > 1))
            {
                BuildAndSetPostParsingStateIfNeeded(RawOptions, info.BadOption, null, true);
                return false;
            }

            return true;
        }

        private void BuildMutuallyExclusiveMap(OptionInfo option)
        {
            var setName = option.MutuallyExclusiveSet;

            if (!_mutuallyExclusiveSetMap.ContainsKey(setName))
            {
                _mutuallyExclusiveSetMap.Add(setName, new MutuallyExclusiveInfo(option));
            }

            _mutuallyExclusiveSetMap[setName].IncrementOccurrence();
        }

        private static void BuildAndSetPostParsingStateIfNeeded(object options, OptionInfo option, bool? required, bool? mutualExclusiveness)
        {
            var commandLineOptionsBase = options as CommandLineOptionsBase;
            if (commandLineOptionsBase == null) 
                return;

            var error = new ParsingError {
                                             BadOption = {
                                                             ShortName = option.ShortName, 
                                                             LongName = option.LongName
                                                         }
                                         };

            if (required != null) 
                error.ViolatesRequired = required.Value;
            if (mutualExclusiveness != null) 
                error.ViolatesMutualExclusiveness = mutualExclusiveness.Value;

            (commandLineOptionsBase).InternalLastPostParsingState.AddError(error);
        }
    }
}