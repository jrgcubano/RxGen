using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using RxGen.People.Models;

namespace RxGen.People.Api
{
    public static class PeopleRequestGuard
    {
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidResults(int results, string parameterName)
        {
            if (results < 1 || results > 5000)
            {
                throw new ArgumentException("Value must be between 1 and 5000", parameterName);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidPage(int page, string parameterName)
        {
            if (page <= 0)
            {
                throw new ArgumentException("Value must be greather than 0", parameterName);
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NotFields(IEnumerable<Field> collection, string parameterName)
        {
            if (collection.Count() > 0)
            {
                throw new ArgumentException("You can only include or exclude fields, not both at the same time.", parameterName);
            }
        }
    }
}
