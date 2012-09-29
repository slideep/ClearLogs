using System;
using ClearLogs.Help;

namespace ClearLogs.Attributes
{
    public abstract class MultiLineTextAttribute : Attribute
    {
        readonly string _line1;
        readonly string _line2;
        readonly string _line3;
        readonly string _line4;
        readonly string _line5;

        protected MultiLineTextAttribute(string line1)
        {
            _line1 = line1;
        }

        protected MultiLineTextAttribute(string line1, string line2)
            : this(line1)
        {
            _line2 = line2;
        }

        protected MultiLineTextAttribute(string line1, string line2, string line3)
            : this(line1, line2)
        {
            _line3 = line3;
        }

        protected MultiLineTextAttribute(string line1, string line2, string line3, string line4)
            : this(line1, line2, line3)
        {
            _line4 = line4;
        }

        protected MultiLineTextAttribute(string line1, string line2, string line3, string line4, string line5)
            : this(line1, line2, line3, line4)
        {
            _line5 = line5;
        }

    }
}