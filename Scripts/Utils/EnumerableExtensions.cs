using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumerableExtensions
{
    public static ulong Sum<TSource>(
        this IEnumerable<TSource> items,
        Func<TSource, ulong> selector
    ) => items.Aggregate(0ul, (sum, item) => selector(item));

    public static IEnumerable<TResult> SelectWhile<TSource, TResult>(
        this IEnumerable<TSource> items,
        Func<TSource, (bool, TResult)> selector
    )
    {
        foreach (var item in items)
        {
            var (goNext, value) = selector(item);
            yield return value;
            if (!goNext)
                break;
        }
    }

    public static IEnumerable<(TFirst, TSecond)> ZipTuple<TFirst, TSecond>(
        this IEnumerable<TFirst> first,
        IEnumerable<TSecond> second
    ) => first.Zip(second, (a, b) => (a, b));
}
