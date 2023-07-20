// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.TestUtilities
{
    /// <summary>
    /// UniqueValues is used to check a set of values to ensure
    /// that all values are unique or close enough to it. We
    /// specify how close as a ratio.
    /// </summary>
    public class UniqueValues
    {
        /// <summary>
        /// Call a delegate until a certain number of unique values are returned,
        /// up to a maximum number of tries. Assert that the target was reached.
        /// </summary>
        public static void Check<T>(ActualValueDelegate<T> del, int targetCount, int maxTries)
            where T : notnull
        {
            var lookup = new Dictionary<T, int>();

            while (--maxTries >= 0)
            {
                T val = del();
                if (!lookup.ContainsKey(val))
                {
                    lookup.Add(val, 1);
                    if (lookup.Count >= targetCount)
                        return;
                }
            }

            Assert.Fail($"After {maxTries} attempts, only {lookup.Count} value(s) found");
        }
    }
}
