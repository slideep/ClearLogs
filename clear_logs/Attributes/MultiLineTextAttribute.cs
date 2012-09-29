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

        internal void AddToHelpText(HelpText helpText, bool before)
        {
            if (before)
            {
                if (!string.IsNullOrEmpty(_line1)) helpText.AddPreOptionsLine(_line1);
                if (!string.IsNullOrEmpty(_line2)) helpText.AddPreOptionsLine(_line2);
                if (!string.IsNullOrEmpty(_line3)) helpText.AddPreOptionsLine(_line3);
                if (!string.IsNullOrEmpty(_line4)) helpText.AddPreOptionsLine(_line4);
                if (!string.IsNullOrEmpty(_line5)) helpText.AddPreOptionsLine(_line5);
            }
            else
            {
                if (!string.IsNullOrEmpty(_line1)) helpText.AddPostOptionsLine(_line1);
                if (!string.IsNullOrEmpty(_line2)) helpText.AddPostOptionsLine(_line2);
                if (!string.IsNullOrEmpty(_line3)) helpText.AddPostOptionsLine(_line3);
                if (!string.IsNullOrEmpty(_line4)) helpText.AddPostOptionsLine(_line4);
                if (!string.IsNullOrEmpty(_line5)) helpText.AddPostOptionsLine(_line5);
            }
        }
    }
}