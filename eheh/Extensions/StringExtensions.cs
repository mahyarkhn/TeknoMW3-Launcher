using eheh.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eheh.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveColors(this string str)
        {
            foreach (object value in Enum.GetValues(typeof(ColorEnum)))
            {
                str = str.Replace("^" + (int)value, "");
            }
            str = str.Replace("^;", "");
            str = str.Replace("^:", "");
            return str;
        }
    }
}
