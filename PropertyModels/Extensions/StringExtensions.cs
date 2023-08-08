using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PropertyModels.Extensions
{
    /// <summary>
    /// Class StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether [contains] [the specified to check].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="toCheck">To check.</param>
        /// <param name="comp">The comp.</param>
        /// <returns><c>true</c> if [contains] [the specified to check]; otherwise, <c>false</c>.</returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Lefts the specified length.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">length - Length is less than zero</exception>
        public static string Left(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
            }

            return (length <= value.Length) ? value.Substring(0, length) : value;
        }

        /// <summary>
        /// Gets the rightmost <paramref name="length" /> characters from a string.
        /// </summary>
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        /// <exception cref="System.ArgumentNullException">value</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">length - Length is less than zero</exception>
        /// <exception cref="ArgumentNullException">value</exception>
        /// <exception cref="ArgumentOutOfRangeException">length - Length is less than zero</exception>
        public static string Right(/*[NotNull]*/ this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
            }

            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }


        /// <summary>
        /// Returns true if ... is valid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is valid; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [is null or empty] [the specified value]; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// is the equal.
        /// compare ignore case
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool iEquals(this string value, string other)
        {
            if (value == null && other == null)
            {
                return true;
            }
            else if (value == null || other == null)
            {
                return false;
            }

            return value.Equals(other, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// is the starts with.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool iStartsWith(this string value, string other)
        {
            if (value == null && other == null)
            {
                return true;
            }
            else if (value == null || other == null)
            {
                return false;
            }

            return value.StartsWith(other, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// is the ends with.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool iEndsWith(this string value, string other)
        {
            if (value == null && other == null)
            {
                return true;
            }
            else if (value == null || other == null)
            {
                return false;
            }

            return value.EndsWith(other, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Replaces the case insensitive.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="search">The search.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceCaseInsensitive(this string input, string search, string value)
        {
            if (input.IsNotNullOrEmpty())
            {
                string result = System.Text.RegularExpressions.Regex.Replace(
                    input,
                    System.Text.RegularExpressions.Regex.Escape(search),
                    value.Replace("$", "$$"),
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    );

                return result;
            }

            return input;
        }

        /// <summary>
        /// Generates the hash32.
        /// 获取稳定的HashCode
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns>
        /// System.UInt32.
        /// </returns>
        public static uint GetDeterministicHashCode(this string s, bool ignoreCase = false)
        {
            uint h = 0;
            int len = s.Length;
            if (len > 0)
            {
                int off = 0;

                for (int i = 0; i < len; i++)
                {
                    char c = s[off++];
                    if (ignoreCase && c >= 'A' && c <= 'Z')
                    {
                        c += (char)('a' - 'A');
                    }
                    h = 31 * h + c;
                }
            }
            return h;
        }

        /// <summary>
        /// Generates the hash64.
        /// 获取稳定的HashCode
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns>
        /// System.UInt64.
        /// </returns>
        public static ulong GetDeterministicHashCode64(this string s, bool ignoreCase = false)
        {
            ulong h = 0;
            int len = s.Length;
            if (len > 0)
            {
                int off = 0;

                for (int i = 0; i < len; i++)
                {
                    char c = s[off++];
                    if (ignoreCase && c >= 'A' && c <= 'Z')
                    {
                        c += (char)('a' - 'A');
                    }
                    h = 31 * h + c;
                }
            }
            return h;
        }

        /// <summary>
        /// Lasts the index of.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>System.Int32.</returns>
        public static int LastIndexOf(this string s, Predicate<char> predicate)
        {
            if (s.IsNullOrEmpty())
            {
                return -1;
            }

            for (int i = s.Length - 1; i >= 0; --i)
            {
                char c = s[i];
                if (predicate(c))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>System.Int32.</returns>
        public static int IndexOf(this string s, Predicate<char> predicate)
        {
            return IndexOf(s, 0, predicate);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>System.Int32.</returns>
        public static int IndexOf(this string s, int startIndex, Predicate<char> predicate)
        {
            if (s.IsNullOrEmpty())
            {
                return -1;
            }

            for (int i = startIndex; i < s.Length; ++i)
            {
                char c = s[i];
                if (predicate(c))
                {
                    return i;
                }
            }

            return -1;
        }
    }

}
