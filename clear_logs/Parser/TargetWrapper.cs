using System.Collections.Generic;
using ClearLogs.Attributes;

namespace ClearLogs.Parser
{
    internal class TargetWrapper
    {
        private readonly object _target;
        private readonly IList<string> _valueList;
        private readonly ValueListAttribute _vla;

        public TargetWrapper(object target)
        {
            _target = target;
            _vla = ValueListAttribute.GetAttribute(_target);
            if (IsValueListDefined)
                _valueList = ValueListAttribute.GetReference(_target);
        }

        public bool IsValueListDefined { get { return _vla != null; } }

        public bool AddValueItemIfAllowed(string item)
        {
            if (_vla.MaximumElements == 0 || _valueList.Count == _vla.MaximumElements)
                return false;

            lock (this)
            {
                _valueList.Add(item);
            }

            return true;
        }
    }
}