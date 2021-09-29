using System;
using System.Globalization;

namespace TestApp.Data
{
    public static class Converter
    {
        private static string[] _Formats { get; set; } = null;
        public static string[] Formats { get
            {
                if (_Formats != null) return _Formats;
                string[] formats = new string[(int)Math.Pow(2, 2)];
                int i = 0;
                foreach (string month in new string[] { "M", "MM" })
                {
                    foreach (string day in new string[] { "dd", "d" })
                    {
                        formats[i++] = $"yyyy-{month}-{day}";
                    }
                }
                _Formats = formats;
                return formats;
            }
        }
        public static bool ParseToDay(this string value, out DateTime output)
        {
            if (DateTime.TryParseExact(value.Split('_')[0], Formats, CultureInfo.CurrentUICulture, DateTimeStyles.None, out DateTime time))
            {
                output = time;
                return true;
            }
            output = DateTime.MinValue;
            return false;
        }
    }
}
