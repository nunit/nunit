// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnit.TestUtilities
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

            Assert.Fail("After {0} attempts, only {1} value(s) found", maxTries, lookup.Count);
        }

        #region Helper Methods

        /// <summary>
        /// Count the number of actually unique values in an IEnumerable
        /// </summary>
        private static int CountUniqueValues(IEnumerable actual)
        {
            var list = new List<object>();

            foreach (object o1 in actual)
                if (!list.Contains(o1))
                    list.Add(o1);

            return list.Count;
        }

        private static int CountUniqueValues<T>(ActualValueDelegate<T> del, int count)
        {
            var list = new List<T>();

            while (count-- > 0)
            {
                T item = del();
                if (!list.Contains(item))
                    list.Add(item);
            }

            return list.Count;
        }

        #endregion

        #region Self-test

        [TestCase(1, 2, 3, 4, 5, ExpectedResult = 5)]
        [TestCase(1, 2, 3, 4, 3, ExpectedResult = 4)]
        [TestCase(1, 1, 1, 1, 1, ExpectedResult = 1)]
        [TestCase(1, 2, 1, 2, 1, ExpectedResult = 2)]
        [TestCase(1, 1, 1, 2, 2, ExpectedResult = 2)]
        [TestCase(ExpectedResult = 0)]
        public static int CountUniqueValuesTest(params int[] values)
        {
            return UniqueValues.CountUniqueValues(values);
        }

        #endregion
    }
}
