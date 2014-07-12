using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SurveyCommon
{
    public static class StringStaticExtensions
    {
        public static int? ToNullableInt(this string s)
        {
            int a;
            return Int32.TryParse(s, out a) ? (int?) a : null;
        }

        public static string AsString (this IEnumerable<char> ch)
        {
            return new string(ch.ToArray());
        }

        public static string ClearOrderListElement(this string s)
        {
            var questionText = s.Trim();
            var result = questionText.SkipWhile(Char.IsDigit).AsString();
            
            if (result.Length > 0 && NumSuffixes.Contains(result[0]))
            {
                result = result.Skip(1).AsString();
            }
            return result;
        }

        public static bool IsValidEmail(this string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper, RegexOptions.None);
            } catch (TimeoutException) {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try {
                return Regex.IsMatch(strIn, @"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" + @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
            } catch (TimeoutException) {
                return false;
            }
        }

        public static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try {
                domainName = idn.GetAscii(domainName);
            } catch (ArgumentException) {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }

        private static bool invalid;
        private const string NumSuffixes = ".)";
    }
}
