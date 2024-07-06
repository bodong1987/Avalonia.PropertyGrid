using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PropertyModels.Extensions;

/// <summary>
/// Class LinqExtensions.
/// </summary>
public static class LinqExtensions
{
    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>System.Int32.</returns>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
    {
        var i = 0;

        foreach (var element in source)
        {
            if (predicate(element))
                return i;

            ++i;
        }

        return -1;
    }

    /// <summary>
    /// Indexes the of.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>System.Int32.</returns>
    public static int IndexOf<TSource>(this IEnumerable<TSource> source, int startIndex, Predicate<TSource> predicate)
    {
        var i = 0;
        var c = 0;

        foreach (var element in source)
        {
            if (c < startIndex)
            {
                ++c;
                ++i;
                continue;
            }

            if (predicate(element))
                return i;

            ++i;
        }

        return -1;
    }

    /// <summary>
    /// Finds the maximum element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <param name="projection">The projection.</param>
    /// <returns>T.</returns>
    /// <exception cref="InvalidOperationException">Empty List</exception>
    public static T MaxElement<T>(this IEnumerable<T> list, Converter<T, int> projection)
    {
        var enumerable = list as T[] ?? list.ToArray();
            
        if (!enumerable.Any())
        {
            throw new InvalidOperationException("Empty List");
        }

        var maxValue = int.MinValue;
        var result = default(T);

        foreach (var item in enumerable)
        {
            var value = projection(item);
                
            // ReSharper disable once InvertIf
            if (value > maxValue)
            {
                maxValue = value;
                result = item;
            }
        }

        return result;
    }

    /// <summary>
    /// Determines whether [contains] [the specified match].
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <param name="match">The match.</param>
    /// <returns><c>true</c> if [contains] [the specified match]; otherwise, <c>false</c>.</returns>
    public static bool Contains<T>(this IEnumerable<T> list, Predicate<T> match)
    {
        foreach (var v in list)
        {
            if (match(v))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Removes the specified match.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">The list.</param>
    /// <param name="match">The match.</param>
    /// <returns><c>true</c> if remove success, <c>false</c> otherwise.</returns>
    public static bool Remove<T>(this IList<T> list, Predicate<T> match)
    {
        var index = list.IndexOf(match);

        if (index == -1)
        {
            return false;
        }
            
        list.RemoveAt(index);
        return true;

    }


    /// <summary>
    /// Distinct the by predicate.
    /// </summary>
    /// <typeparam name="TSource">The type of the t source.</typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>IEnumerable&lt;TSource&gt;.</returns>
    /// <exception cref="System.ArgumentNullException">source</exception>
    /// <exception cref="ArgumentNullException">source</exception>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source
            .GroupBy(predicate)
            .Select(x => x.First());
    }

    /// <summary>
    /// Converts to binding list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">The source.</param>
    /// <returns>BindingList&lt;T&gt;.</returns>
    public static BindingList<T> ToBindingList<T>(this IEnumerable<T> source)
    {
        return new BindingList<T>(source.ToList());
    }

    /// <summary>
    /// Selects the specified selector.
    /// </summary>
    /// <typeparam name="TRet">The type of the t ret.</typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="selector">The selector.</param>
    /// <returns>IEnumerable&lt;TRet&gt;.</returns>
    public static IEnumerable<TRet> Select<TRet>(this IEnumerable enumerable, Func<object, TRet> selector)
    {
        foreach (var item in enumerable)
        {
            yield return selector(item);
        }
    }
}