using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

public static class CollectionExtensions
{
    public static void ForEach<T>(this T[] source, Action<T> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        for (int i = 0; i < source.Length; i++)
            action(source[i]);
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        Type sourceType = source.GetType();

        if (sourceType == typeof(List<T>))
        {
            ((List<T>) source).ForEach(action);
        }
        else if (sourceType == typeof(T[]))
        {
            ((T[]) source).ForEach(action);
        }
        else
        {
            foreach (T item in source)
                action(item);
        }
    }

    public static void ForEach<T>(this List<T> source, Action<T, int> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        for (int i = 0; i < source.Count; i++)
            action(source[i], i);
    }

    public static void ForEach<T>(this T[] source, Action<T, int> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        for (int i = 0; i < source.Length; i++)
            action(source[i], i);
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        Type sourceType = source.GetType();

        if (sourceType == typeof(List<T>))
        {
            ((List<T>) source).ForEach(action);
        }
        else if (sourceType == typeof(T[]))
        {
            ((T[]) source).ForEach(action);
        }
        else
        {
            foreach (T item in source)
            {
                int index = source.IndexOf(item);
                action(item, index);
            }
        }
    }

    public static IEnumerable<T> MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        TKey? maxKeyValue = source.Max(keySelector);
        return source.Where(item => keySelector(item)?.Equals(maxKeyValue) ?? false);
    }

    public static IEnumerable<T> MinBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        TKey? minKeyValue = source.Min(keySelector);
        return source.Where(item => keySelector(item)?.Equals(minKeyValue) ?? false);
    }

    public static bool None<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return !source.Any();
    }

    public static bool None<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return !source.Any(predicate);
    }

    public static IEnumerable<T> NonNullItems<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.Where(item => item != null);
    }

    public static int IndexOf<T>(this IEnumerable<T> source, T value)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        Type sourceType = source.GetType();

        if (sourceType == typeof(List<T>))
            return ((List<T>) source).IndexOf(value);

        if (sourceType == typeof(T[]))
            return Array.IndexOf((T[]) source, value);

        return source.IndexOf(value, null);
    }

    public static int IndexOf<T>(this IEnumerable<T> source, T value, IEqualityComparer<T>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        comparer ??= EqualityComparer<T>.Default;

        var foundItem = source
            .Select((item, index) => new { item, index })
            .FirstOrDefault(x => comparer.Equals(x.item, value));

        return foundItem != null ? foundItem.index : -1;
    }

    public static (IEnumerable<T> True, IEnumerable<T> False) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        // Deffered execution version

        var trueItems = source.Where(predicate);
        return (trueItems, source.Except(trueItems));

        // Immediate execution version

        /*
        List<T> trueItems = CreateList.With<T>();
        List<T> falseItems = CreateList.With<T>();

        source.ForEach(item =>
        {
            if (predicate(item))
                trueItems.Add(item);
            else
                falseItems.Add(item);
        });

        return (trueItems, falseItems);
        */
    }

    public static T RandomItem<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        Random random = new();
        T current = default!;
        int count = 0;

        source.ForEach(item =>
        {
            count++;
            if (random.Next(0, count) == 0)
                current = item;
        });

        if (count == 0)
            throw new InvalidOperationException("Sequence was empty");

        return current;
    }

    public static T RandomItem<T>(this IEnumerable<T> source, Random random)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        T current = default!;
        int count = 0;

        source.ForEach(item =>
        {
            count++;

            if (random.Next(0, count) == 0)
                current = item;
        });

        if (count == 0)
            throw new InvalidOperationException("Sequence was empty");

        return current;
    }

    public static T? RandomItemOrDefault<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        Random random = new();
        T? current = default;
        int count = 0;

        source.ForEach(item =>
        {
            count++;

            if (random.Next(0, count) == 0)
                current = item;
        });

        return current;
    }

    public static T? RandomItemOrDefault<T>(this IEnumerable<T> source, Random random)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        T? current = default;
        int count = 0;

        source.ForEach(item =>
        {
            count++;

            if (random.Next(0, count) == 0)
                current = item;
        });

        return current;
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        T[] items = source.ToArray();
        Random random = new();

        for (int i = items.Length - 1; i > 0; i--)
        {
            int swapIndex = random.Next(i + 1);
            (items[i], items[swapIndex]) = (items[swapIndex], items[i]);
        }

        return items;
    }

    public static IEnumerable<T> Without<T>(this IEnumerable<T> source, T value)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return source.Without(value, null);
    }

    public static IEnumerable<T> Without<T>(this IEnumerable<T> source, T value, IEqualityComparer<T>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        comparer ??= EqualityComparer<T>.Default;

        return source.Where(item => !comparer.Equals(item, value));
    }

    public static IEnumerable<T> WithoutFirst<T>(this IEnumerable<T> source, T value)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return source.WithoutFirst(value, null);
    }

    public static IEnumerable<T> WithoutFirst<T>(this IEnumerable<T> source, T value, IEqualityComparer<T>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        comparer ??= EqualityComparer<T>.Default;

        bool removed = false;

        foreach (var item in source)
        {
            if (comparer.Equals(item, value) && !removed)
            {
                removed = true;
                continue;
            }

            yield return item;
        }
    }

    public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source, T value)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        return source.WithoutLast(value, null);
    }

    public static IEnumerable<T> WithoutLast<T>(this IEnumerable<T> source, T value, IEqualityComparer<T>? comparer)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        comparer ??= EqualityComparer<T>.Default;

        return source.Reverse().WithoutFirst(value, comparer).Reverse();
    }
}

#nullable restore
