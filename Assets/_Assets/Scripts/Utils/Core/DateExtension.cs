using System;
using System.Globalization;

namespace WatKhaoWong.Utils.Core
{
    public static class DateExtension
    {
        #region --Fields-- (In Class)
        private const string FullDateFormat = "d/M/yyyy h:mm:ss tt";
        private const string OnlyDateFormat = "d/M/yyyy";
        private const string OnlyTimeFormat = "h:mm:ss tt";

        private static readonly CultureInfo GregorianCulture = new CultureInfo("en-US")
        {
            DateTimeFormat = { Calendar = new GregorianCalendar() }
        };
        #endregion



        #region --Methods-- (Custom PUBLIC) ~STATIC~ ~Gregorian~
        public static string ToGregorianString(this DateTime dateTime)
        {
            return dateTime.ToString(FullDateFormat, GregorianCulture);
        }

        public static string ToGregorianString(this DateTime dateTime, string format)
        {
            return dateTime.ToString(format, GregorianCulture);
        }

        public static string ToGregorianString(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToString(FullDateFormat, GregorianCulture);
            }

            return dateTime.ToString();
        }

        public static string ToGregorianOnlyDateString(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToString(OnlyDateFormat, GregorianCulture);
            }

            return dateTime.ToString();
        }

        public static bool TryParseGregorian(this string dateString, out DateTime result)
        {
            return DateTime.TryParseExact(dateString, FullDateFormat, GregorianCulture, DateTimeStyles.None, out result);
        }

        public static bool TryParseGregorianOnlyDateFormat(this string dateString, out DateTime result)
        {
            return DateTime.TryParseExact(dateString, OnlyDateFormat, GregorianCulture, DateTimeStyles.None, out result);
        }
        #endregion
    }
}