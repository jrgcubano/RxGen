using System;
using System.Collections.Generic;

namespace RxGen.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> @this)
        {
            if (@this == null)
                throw new ArgumentNullException("source");

            return new HashSet<T>(@this);
        }
    }
}