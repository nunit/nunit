// ***********************************************************************
// Copyright (c) 2007-2013 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using NUnit.TestUtilities.Comparers;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class EqualConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new EqualConstraint(4);
            expectedDescription = "4";
            stringRepresentation = "<equal 4>";
        }

        private object[] SuccessData = new object[] {4, 4.0f, 4.0d, 4.0000m};

        private object[] FailureData = new object[]
            {
                new TestCaseData(5, "5"),
                new TestCaseData(null, "null"),
                new TestCaseData("Hello", "\"Hello\""),
                new TestCaseData(double.NaN, double.NaN.ToString()),
                new TestCaseData(double.PositiveInfinity, double.PositiveInfinity.ToString())
            };

        #region DateTimeEquality

        public class DateTimeEquality
        {
            [Test]
            public void CanMatchDates()
            {
                DateTime expected = new DateTime(2007, 4, 1);
                DateTime actual = new DateTime(2007, 4, 1);
                Assert.That(actual, new EqualConstraint(expected));
            }

            [Test]
            public void CanMatchDatesWithinTimeSpan()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 1, 13, 1, 0);
                TimeSpan tolerance = TimeSpan.FromMinutes(5.0);
                Assert.That(actual, new EqualConstraint(expected).Within(tolerance));
            }

            [Test]
            public void CanMatchDatesWithinDays()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 4, 13, 0, 0);
                Assert.That(actual, new EqualConstraint(expected).Within(5).Days);
            }

            [Test]
            public void CanMatchDatesWithinHours()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 1, 16, 0, 0);
                Assert.That(actual, new EqualConstraint(expected).Within(5).Hours);
            }


            [Test]
            public void CanMatchUsingIsEqualToWithinTimeSpan()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 1, 13, 1, 0);
                Assert.That(actual, Is.EqualTo(expected).Within(TimeSpan.FromMinutes(2)));
            }


            [Test]
            public void CanMatchDatesWithinMinutes()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 1, 13, 1, 0);
                Assert.That(actual, new EqualConstraint(expected).Within(5).Minutes);
            }

            [Test]
            public void CanMatchTimeSpanWithinMinutes()
            {
                TimeSpan expected = new TimeSpan(10, 0, 0);
                TimeSpan actual = new TimeSpan(10, 2, 30);
                Assert.That(actual, new EqualConstraint(expected).Within(5).Minutes);
            }

            [Test]
            public void CanMatchDatesWithinSeconds()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 1, 13, 1, 0);
                Assert.That(actual, new EqualConstraint(expected).Within(300).Seconds);
            }

            [Test]
            public void CanMatchDatesWithinMilliseconds()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 1, 13, 1, 0);
                Assert.That(actual, new EqualConstraint(expected).Within(300000).Milliseconds);
            }

            [Test]
            public void CanMatchDatesWithinTicks()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 1, 13, 1, 0);
                Assert.That(actual, new EqualConstraint(expected).Within(TimeSpan.TicksPerMinute*5).Ticks);
            }

            [Test]
            public void ErrorIfDaysPrecedesWithin()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(DateTime.Now, Is.EqualTo(DateTime.Now).Days.Within(5)));
            }

            [Test]
            public void ErrorIfHoursPrecedesWithin()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(DateTime.Now, Is.EqualTo(DateTime.Now).Hours.Within(5)));
            }

            [Test]
            public void ErrorIfMinutesPrecedesWithin()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(DateTime.Now, Is.EqualTo(DateTime.Now).Minutes.Within(5)));
            }

            [Test]
            public void ErrorIfSecondsPrecedesWithin()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(DateTime.Now, Is.EqualTo(DateTime.Now).Seconds.Within(5)));
            }

            [Test]
            public void ErrorIfMillisecondsPrecedesWithin()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(DateTime.Now, Is.EqualTo(DateTime.Now).Milliseconds.Within(5)));
            }

            [Test]
            public void ErrorIfTicksPrecedesWithin()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(DateTime.Now, Is.EqualTo(DateTime.Now).Ticks.Within(5)));
            }
        }

        #endregion

        #region DateTimeOffsetEquality

#if !NETCF
        public class DateTimeOffSetEquality
        {
            [Test]
            public void CanMatchDates()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1));
                var actual = new DateTimeOffset(new DateTime(2007, 4, 1));
                Assert.That(actual, new EqualConstraint(expected));
            }

            [Test]
            public void CanMatchDatesWithinTimeSpan()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                var tolerance = TimeSpan.FromMinutes(5.0);
                Assert.That(actual, new EqualConstraint(expected).Within(tolerance));
            }

            [Test]
            public void CanMatchDatesWithinDays()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual = new DateTimeOffset(new DateTime(2007, 4, 4, 13, 0, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(5).Days);
            }

            [Test]
            public void CanMatchUsingIsEqualToWithinTimeSpan()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, Is.EqualTo(expected).Within(TimeSpan.FromMinutes(2)));
            }

            [Test]
            public void CanMatchDatesWithinMinutes()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual =  new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(5).Minutes);
            }

            [Test]
            public void CanMatchDatesWithinSeconds()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual =  new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(300).Seconds);
            }

            [Test]
            public void CanMatchDatesWithinMilliseconds()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual =  new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(300000).Milliseconds);
            }

            [Test]
            public void CanMatchDatesWithinTicks()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual =  new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(TimeSpan.TicksPerMinute*5).Ticks);
            }

            [Test]
            public void DTimeOffsetCanMatchDatesWithinHours()
            {
                var a = DateTimeOffset.Parse("2012-01-01T12:00Z");
                var b = DateTimeOffset.Parse("2012-01-01T12:01Z");
                Assert.That(a, Is.EqualTo(b).Within(TimeSpan.FromMinutes(2)));
            }
        }
#endif

        #endregion

        #region DictionaryEquality

        public class DictionaryEquality
        {
            [Test]
            public void CanMatchDictionaries_SameOrder()
            {
                Assert.AreEqual(new Dictionary<int, int> {{0, 0}, {1, 1}, {2, 2}},
                                new Dictionary<int, int> {{0, 0}, {1, 1}, {2, 2}});
            }

            [Test]
            public void CanMatchDictionaries_Failure()
            {
                Assert.Throws<AssertionException>(
                    () => Assert.AreEqual(new Dictionary<int, int> {{0, 0}, {1, 1}, {2, 2}},
                                          new Dictionary<int, int> {{0, 0}, {1, 5}, {2, 2}}));
            }

            [Test]
            public void CanMatchDictionaries_DifferentOrder()
            {
                Assert.AreEqual(new Dictionary<int, int> {{0, 0}, {1, 1}, {2, 2}},
                                new Dictionary<int, int> {{0, 0}, {2, 2}, {1, 1}});
            }

#if !SILVERLIGHT
            [Test]
            public void CanMatchHashtables_SameOrder()
            {
                Assert.AreEqual(new Hashtable {{0, 0}, {1, 1}, {2, 2}},
                                new Hashtable {{0, 0}, {1, 1}, {2, 2}});
            }

            [Test]
            public void CanMatchHashtables_Failure()
            {
                Assert.Throws<AssertionException>(
                    () => Assert.AreEqual(new Hashtable {{0, 0}, {1, 1}, {2, 2}},
                                          new Hashtable {{0, 0}, {1, 5}, {2, 2}}));
            }

            [Test]
            public void CanMatchHashtables_DifferentOrder()
            {
                Assert.AreEqual(new Hashtable {{0, 0}, {1, 1}, {2, 2}},
                                new Hashtable {{0, 0}, {2, 2}, {1, 1}});
            }

            [Test]
            public void CanMatchHashtableWithDictionary()
            {
                Assert.AreEqual(new Hashtable {{0, 0}, {1, 1}, {2, 2}},
                                new Dictionary<int, int> {{0, 0}, {2, 2}, {1, 1}});
            }
#endif
        }

        #endregion

        #region FloatingPointEquality

        public class FloatingPointEquality
        {
            [TestCase(double.NaN)]
            [TestCase(double.PositiveInfinity)]
            [TestCase(double.NegativeInfinity)]
            [TestCase(float.NaN)]
            [TestCase(float.PositiveInfinity)]
            [TestCase(float.NegativeInfinity)]
            public void CanMatchSpecialFloatingPointValues(object value)
            {
                Assert.That(value, new EqualConstraint(value));
            }

            [TestCase(20000000000000004.0)]
            [TestCase(19999999999999996.0)]
            public void CanMatchDoublesWithUlpTolerance(object value)
            {
                Assert.That(value, new EqualConstraint(20000000000000000.0).Within(1).Ulps);
            }

            [TestCase(20000000000000008.0)]
            [TestCase(19999999999999992.0)]
            public void FailsOnDoublesOutsideOfUlpTolerance(object value)
            {
                var ex = Assert.Throws<AssertionException>(() => Assert.That(value, new EqualConstraint(20000000000000000.0).Within(1).Ulps));
                Assert.That(ex.Message, Does.Contain("+/- 1 Ulps"));
            }

            [TestCase(19999998.0f)]
            [TestCase(20000002.0f)]
            public void CanMatchSinglesWithUlpTolerance(object value)
            {
                Assert.That(value, new EqualConstraint(20000000.0f).Within(1).Ulps);
            }

            [TestCase(19999996.0f)]
            [TestCase(20000004.0f)]
            public void FailsOnSinglesOutsideOfUlpTolerance(object value)
            {
                var ex = Assert.Throws<AssertionException>(() => Assert.That(value, new EqualConstraint(20000000.0f).Within(1).Ulps));
                Assert.That(ex.Message, Does.Contain("+/- 1 Ulps"));
            }

            [TestCase(9500.0)]
            [TestCase(10000.0)]
            [TestCase(10500.0)]
            public void CanMatchDoublesWithRelativeTolerance(object value)
            {
                Assert.That(value, new EqualConstraint(10000.0).Within(10.0).Percent);
            }

            [TestCase(8500.0)]
            [TestCase(11500.0)]
            public void FailsOnDoublesOutsideOfRelativeTolerance(object value)
            {
                var ex = Assert.Throws<AssertionException>(() => Assert.That(value, new EqualConstraint(10000.0).Within(10.0).Percent));
                Assert.That(ex.Message, Does.Contain("+/- 10.0d Percent"));
            }

            [TestCase(9500.0f)]
            [TestCase(10000.0f)]
            [TestCase(10500.0f)]
            public void CanMatchSinglesWithRelativeTolerance(object value)
            {
                Assert.That(value, new EqualConstraint(10000.0f).Within(10.0f).Percent);
            }

            [TestCase(8500.0f)]
            [TestCase(11500.0f)]
            public void FailsOnSinglesOutsideOfRelativeTolerance(object value)
            {
                var ex = Assert.Throws<AssertionException>(() => Assert.That(value, new EqualConstraint(10000.0f).Within(10.0f).Percent));
                Assert.That(ex.Message, Does.Contain("+/- 10.0f Percent"));
            }

            /// <summary>Applies both the Percent and Ulps modifiers to cause an exception</summary>
            [Test]
            public void ErrorWithPercentAndUlpsToleranceModes()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    var shouldFail = new EqualConstraint(100.0f).Within(10.0f).Percent.Ulps;
                });
            }

            /// <summary>Applies both the Ulps and Percent modifiers to cause an exception</summary>
            [Test]
            public void ErrorWithUlpsAndPercentToleranceModes()
            {
                Assert.Throws<InvalidOperationException>(() =>
                {
                    EqualConstraint shouldFail = new EqualConstraint(100.0f).Within(10.0f).Ulps.Percent;
                });
            }

            [Test]
            public void ErrorIfPercentPrecedesWithin()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(1010, Is.EqualTo(1000).Percent.Within(5)));
            }

            [Test]
            public void ErrorIfUlpsPrecedesWithin()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(1010.0, Is.EqualTo(1000.0).Ulps.Within(5)));
            }

            [TestCase(1000, 1010)]
            [TestCase(1000U, 1010U)]
            [TestCase(1000L, 1010L)]
            [TestCase(1000UL, 1010UL)]
            public void ErrorIfUlpsIsUsedOnIntegralType(object x, object y)
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(y, Is.EqualTo(x).Within(2).Ulps));
            }

            [Test]
            public void ErrorIfUlpsIsUsedOnDecimal()
            {
                Assert.Throws<InvalidOperationException>(() => Assert.That(100m, Is.EqualTo(100m).Within(2).Ulps));
            }
        }

        #endregion

        #region UsingModifier

        public class UsingModifier
        {
            [Test]
            public void UsesProvidedIComparer()
            {
                var comparer = new ObjectComparer();
                Assert.That(2 + 2, Is.EqualTo(4).Using(comparer));
                Assert.That(comparer.WasCalled, "Comparer was not called");
            }

            [Test]
            public void UsesProvidedEqualityComparer()
            {
                var comparer = new ObjectEqualityComparer();
                Assert.That(2 + 2, Is.EqualTo(4).Using(comparer));
                Assert.That(comparer.Called, "Comparer was not called");
            }

            [Test]
            public void UsesProvidedGenericEqualityComparer()
            {
                var comparer = new GenericEqualityComparer<int>();
                Assert.That(2 + 2, Is.EqualTo(4).Using(comparer));
                Assert.That(comparer.WasCalled, "Comparer was not called");
            }

            [Test]
            public void UsesProvidedGenericComparer()
            {
                var comparer = new GenericComparer<int>();
                Assert.That(2 + 2, Is.EqualTo(4).Using(comparer));
                Assert.That(comparer.WasCalled, "Comparer was not called");
            }

            [Test]
            public void UsesProvidedGenericComparison()
            {
                var comparer = new GenericComparison<int>();
                Assert.That(2 + 2, Is.EqualTo(4).Using(comparer.Delegate));
                Assert.That(comparer.WasCalled, "Comparer was not called");
            }

            [Test]
            public void UsesProvidedLambda_IntArgs()
            {
                Assert.That(2 + 2, Is.EqualTo(4).Using<int>((x, y) => x.CompareTo(y)));
            }

            [Test]
            public void UsesProvidedLambda_StringArgs()
            {
                Assert.That("hello", Is.EqualTo("HELLO").Using<string>((x, y) => StringUtil.Compare(x, y, true)));
            }

            [Test]
            public void UsesProvidedListComparer()
            {
                var list1 = new List<int>() {2, 3};
                var list2 = new List<int>() {3, 4};

                var list11 = new List<List<int>>() {list1};
                var list22 = new List<List<int>>() {list2};
                var comparer = new IntListEqualComparer();

                Assert.That(list11, new CollectionEquivalentConstraint(list22).Using(comparer));
            }

            public class IntListEqualComparer : IEqualityComparer<List<int>>
            {
                public bool Equals(List<int> x, List<int> y)
                {
                    return x.Count == y.Count;
                }

                public int GetHashCode(List<int> obj)
                {
                    return obj.Count.GetHashCode();
                }
            }

            [Test]
            public void UsesProvidedArrayComparer()
            {
                var array1 = new int[] {2, 3};
                var array2 = new int[] {3, 4};

                var list11 = new List<int[]>() {array1};
                var list22 = new List<int[]>() {array2};
                var comparer = new IntArrayEqualComparer();

                Assert.That(list11, new CollectionEquivalentConstraint(list22).Using(comparer));
            }

            public class IntArrayEqualComparer : IEqualityComparer<int[]>
            {
                public bool Equals(int[] x, int[] y)
                {
                    return x.Length == y.Length;
                }

                public int GetHashCode(int[] obj)
                {
                    return obj.Length.GetHashCode();
                }
            }
        }

        #endregion
    }
}