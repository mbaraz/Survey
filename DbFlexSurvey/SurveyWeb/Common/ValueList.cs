using System;
using System.Collections.Generic;
using System.Linq;
using SurveyCommon;

namespace SurveyWeb.Common
{
    internal static class ValueList
    {
        public static IEnumerable<string> FromString(string listText)
        {
            var lines = listText.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(StringStaticExtensions.ClearOrderListElement);
        }
    }
}