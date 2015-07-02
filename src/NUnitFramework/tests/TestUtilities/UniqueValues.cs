using System;
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
        /// Call a delegate a specified number of times and check that
        /// the returned values are more or less unique.
        /// </summary>
        public static void Check<T>(ActualValueDelegate<T> del, int count, double successRatio)
        {
            int minExpected = (int)(count * successRatio);

            int unique = CountUniqueValues(del, count);
            Assert.That(unique, Is.Not.EqualTo(1), "All values were the same!");

            // TODO: Change to an actual warning once we implement them
            Assert.That(unique, Is.GreaterThanOrEqualTo(minExpected), "WARNING: The number of unique values less than expected.");
        }

        public static void Check<T>(ActualValueDelegate<T> del, int count)
        {
            Check(del, count, 0.8);
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
