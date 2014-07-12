using System;
using System.Collections.Generic;
using System.Linq;

namespace SurveyCommon
{
    public static class EnumerableStaticExtensions
    {
        public static IEnumerable<int> CheckForNulls(this IEnumerable<int?> source)
        {
            foreach (var answer in source) {
                if (answer == null)
                    throw new ArgumentNullException("source");

                yield return (int)answer;
            }
        }

        public static int MaxOrDefault<T>(this ICollection<T> args, Func<T, int> selector)
        {
            if (args == null)
                return 0;

            return args.Any() ? args.Max(selector) : 0;
        }

        public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<KeyValuePair<T1, T2>> openAnswers)
        {
            return openAnswers.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static IEnumerable<T> removeLefts<T>(this IEnumerable<T> source, int count = 1)
        {

            return source.Except(source.Take(count));
        }
    }
}
