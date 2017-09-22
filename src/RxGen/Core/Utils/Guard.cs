using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace RxGen.Core.Utils
{
    public static class Guard
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotNull(object target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static bool IsBetween<TValue>(this TValue value, TValue low, TValue high)
            where TValue : IComparable
        {
            return Comparer<TValue>.Default.Compare(low, value) <= 0 && Comparer<TValue>.Default.Compare(high, value) >= 0;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Between<TValue>(TValue target, TValue lower, TValue upper, string parameterName)
            where TValue : IComparable
        {
            if (!target.IsBetween(lower, upper))
            {
                throw new ArgumentException($"Value must be between {lower} and {upper}", parameterName);
            }
        }
    }
}