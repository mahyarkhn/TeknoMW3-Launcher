using System;

namespace eheh.Helpers
{
    public class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        private const string FileSizeFormat = "fs";

        private const decimal OneKiloByte = 1024m;

        private const decimal OneMegaByte = 1048576m;

        private const decimal OneGigaByte = 1073741824m;

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null || !format.StartsWith("fs"))
            {
                return DefaultFormat(format, arg, formatProvider);
            }
            if (arg is string)
            {
                return DefaultFormat(format, arg, formatProvider);
            }
            decimal num;
            try
            {
                num = Convert.ToDecimal(arg);
            }
            catch
            {
                return DefaultFormat(format, arg, formatProvider);
            }
            string arg2;
            if (num > 1073741824m)
            {
                num /= 1073741824m;
                arg2 = "GB";
            }
            else if (num > 1048576m)
            {
                num /= 1048576m;
                arg2 = "MB";
            }
            else if (num > 1024m)
            {
                num /= 1024m;
                arg2 = "kB";
            }
            else
            {
                arg2 = " B";
            }
            string text = format.Substring(2);
            if (string.IsNullOrEmpty(text))
            {
                text = "2";
            }
            return string.Format("{0:N" + text + "}{1}", num, arg2);
        }

        private static string DefaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            IFormattable formattable = arg as IFormattable;
            if (formattable != null)
            {
                return formattable.ToString(format, formatProvider);
            }
            return arg.ToString();
        }
    }
}
