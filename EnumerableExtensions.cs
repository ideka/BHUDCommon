﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ideka.BHUDCommon
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<(TA, TB)> Zip<TA, TB>(this IEnumerable<TA> source, IEnumerable<TB> other)
            => source.Zip(other, (a, b) => (a, b));

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            => source.MaxBy(selector, null);

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector,
            IComparer<TKey> comparer)
            => source.MostBy(selector, comparer, true);

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            => source.MinBy(selector, null);

        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector,
            IComparer<TKey> comparer)
            => source.MostBy(selector, comparer, false);

        private static TSource MostBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer, bool max)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            comparer = comparer ?? Comparer<TKey>.Default;
            int factor = max ? -1 : 1;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                    throw new InvalidOperationException("Sequence contains no elements");

                var most = sourceIterator.Current;
                var mostKey = selector(most);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, mostKey) * factor < 0)
                    {
                        most = candidate;
                        mostKey = candidateProjected;
                    }
                }

                return most;
            }
        }
    }
}