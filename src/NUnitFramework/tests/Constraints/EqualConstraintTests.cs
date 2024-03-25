// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities.Comparers;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class EqualConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new EqualConstraint(4);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "4";
            StringRepresentation = "<equal 4>";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { 4, 4.0f, 4.0d, 4.0000m };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData(5, "5"),
            new TestCaseData(null, "null"),
            new TestCaseData("Hello", "\"Hello\""),
            new TestCaseData(double.NaN, double.NaN.ToString()),
            new TestCaseData(double.PositiveInfinity, double.PositiveInfinity.ToString())
        };
#pragma warning restore IDE0052 // Remove unread private members

        [Test]
        public void Complex_PassesEquality()
        {
            Assert.That(new System.Numerics.Complex(1, 100), Is.EqualTo(new System.Numerics.Complex(1, 100)));
        }

        #region StringEquality
        [Test]
        public void RespectsCultureWhenCaseIgnored()
        {
            var constraint = new EqualConstraint("r\u00E9sum\u00E9").IgnoreCase;

            var result = constraint.ApplyTo("re\u0301sume\u0301");

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void DoesntRespectCultureWhenCasingMatters()
        {
            var constraint = new EqualConstraint("r\u00E9sum\u00E9");

            var result = constraint.ApplyTo("re\u0301sume\u0301");

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void Bug524CharIntWithoutOverload()
        {
            char c = '\u0000';
            Assert.That(c, Is.EqualTo(0));
        }

        #endregion

        #region StreamEquality

        public class StreamEquality
        {
            private const string HelloString = "Greetings";
            private const string GoodbyeString = "GoodByte!";

            [Test]
            public void UnSeekableActualStreamEqual()
            {
                using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(HelloString));

                using var actualArchive = CreateZipArchive(HelloString);
                ZipArchiveEntry entry = actualArchive.Entries[0];

                using Stream entryStream = entry.Open(); // an archive in read mode returns a DeflateStream, which is un-seekable
                Assert.That(entryStream, Is.EqualTo(expectedStream));
            }

            [Test]
            public void UnSeekableActualStreamUnequal()
            {
                using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(HelloString));

                using var actualArchive = CreateZipArchive(GoodbyeString);
                ZipArchiveEntry entry = actualArchive.Entries[0];

                using Stream entryStream = entry.Open(); // an archive in read mode returns a DeflateStream, which is un-seekable
                Assert.That(entryStream, Is.Not.EqualTo(expectedStream));
            }

            [Test]
            public void UnSeekableExpectedStreamEqual()
            {
                using var actualStream = new MemoryStream(Encoding.UTF8.GetBytes(HelloString));

                using var actualArchive = CreateZipArchive(HelloString);
                ZipArchiveEntry entry = actualArchive.Entries[0];

                using Stream expectedStream = entry.Open(); // an archive in read mode returns a DeflateStream, which is un-seekable
                Assert.That(actualStream, Is.EqualTo(expectedStream));
            }

            [Test]
            public void UnSeekableExpectedStreamUnequal()
            {
                using var actualStream = new MemoryStream(Encoding.UTF8.GetBytes(HelloString));

                using var actualArchive = CreateZipArchive(GoodbyeString);
                ZipArchiveEntry entry = actualArchive.Entries[0];

                using Stream expectedStream = entry.Open(); // an archive in read mode returns a DeflateStream, which is un-seekable
                Assert.That(actualStream, Is.Not.EqualTo(expectedStream));
            }

            [Test]
            public void UnSeekableActualAndExpectedStreamsEqual()
            {
                using var expectedArchive = CreateZipArchive(HelloString);
                ZipArchiveEntry expectedEntry = expectedArchive.Entries[0];
                using Stream expectedStream = expectedEntry.Open();

                using var actualArchive = CreateZipArchive(HelloString);
                ZipArchiveEntry actualEntry = actualArchive.Entries[0];
                using Stream actualStream = expectedEntry.Open();

                Assert.That(actualStream, Is.EqualTo(expectedStream));
            }

            [Test]
            public void UnSeekableActualAndExpectedStreamsUnequal()
            {
                using var expectedArchive = CreateZipArchive(HelloString);
                ZipArchiveEntry expectedEntry = expectedArchive.Entries[0];
                using Stream expectedStream = expectedEntry.Open();

                using var actualArchive = CreateZipArchive(GoodbyeString);
                ZipArchiveEntry actualEntry = actualArchive.Entries[0];
                using Stream actualStream = actualEntry.Open();

                Assert.That(expectedStream, Is.Not.EqualTo(actualStream));
            }

            [Test]
            public void UnSeekableLargeActualStreamEqual()
            {
                // This creates a string that exceeds 4096 bytes for the StreamsComparer loop.
                string streamValue = string.Concat(Enumerable.Repeat("Greetings from a stream that is from the other side!", 100));

                using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(streamValue));

                using var actualArchive = CreateZipArchive(streamValue);
                ZipArchiveEntry entry = actualArchive.Entries[0];

                using Stream entryStream = entry.Open();
                Assert.That(entryStream, Is.EqualTo(expectedStream));
            }

            [Test]
            public void UnSeekableLargeActualStreamUnequal()
            {
                // This creates a string that exceeds 4096 bytes for the StreamsComparer loop.
                string streamValue = string.Concat(Enumerable.Repeat("Greetings from a stream that is from the other side!", 100));

                string unequalStream = string.Concat(streamValue, "Some extra difference at the end.");

                using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(streamValue));

                using var actualArchive = CreateZipArchive(unequalStream);
                ZipArchiveEntry entry = actualArchive.Entries[0];

                using Stream entryStream = entry.Open();
                Assert.That(entryStream, Is.Not.EqualTo(expectedStream));
            }

            [Test]
            public void SeekableEmptyStreamEqual()
            {
                using var expectedStream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));

                using var actualStream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));

                Assert.That(actualStream, Is.EqualTo(expectedStream));
            }

            private static ZipArchive CreateZipArchive(string content)
            {
                var archiveContents = new MemoryStream();
                using (var archive = new ZipArchive(archiveContents, ZipArchiveMode.Create, leaveOpen: true))
                {
                    ZipArchiveEntry demoFile = archive.CreateEntry($"{content} entry");

                    using Stream entryStream = demoFile.Open();

                    using var entryFs = new StreamWriter(entryStream);
                    entryFs.Write(content);
                    entryFs.Flush();
                }

                return new ZipArchive(archiveContents, ZipArchiveMode.Read, leaveOpen: false);
            }
        }

        #endregion

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
                Assert.That(actual, new EqualConstraint(expected).Within(300_000).Milliseconds);
            }

            [Test]
            public void CanMatchDatesWithinTicks()
            {
                DateTime expected = new DateTime(2007, 4, 1, 13, 0, 0);
                DateTime actual = new DateTime(2007, 4, 1, 13, 1, 0);
                Assert.That(actual, new EqualConstraint(expected).Within(TimeSpan.TicksPerMinute * 5).Ticks);
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

        public class DateTimeOffsetShouldBeSame
        {
            [Datapoints]
            public static readonly DateTimeOffset[] SameDateTimeOffsets =
                {
                    new DateTimeOffset(new DateTime(2014, 1, 30, 12, 34, 56), new TimeSpan(6, 15, 0)),
                    new DateTimeOffset(new DateTime(2014, 1, 30, 9, 19, 56), new TimeSpan(3, 0, 0)),
                    new DateTimeOffset(new DateTime(2014, 1, 30, 9, 19, 56), new TimeSpan(3, 1, 0)),
                    new DateTimeOffset(new DateTime(2014, 1, 30, 9, 19, 55), new TimeSpan(3, 0, 0)),
                    new DateTimeOffset(new DateTime(2014, 1, 30, 9, 19, 55), new TimeSpan(3, 50, 0))
                };

            [Theory]
            public void PositiveEqualityTest(DateTimeOffset value1, DateTimeOffset value2)
            {
                Assume.That(value1 == value2);

                Assert.That(value1, Is.EqualTo(value2));
            }

            [Theory]
            public void NegativeEqualityTest(DateTimeOffset value1, DateTimeOffset value2)
            {
                Assume.That(value1 != value2);

                Assert.That(value1, Is.Not.EqualTo(value2));
            }

            [Theory]
            public void PositiveEqualityTestWithTolerance(DateTimeOffset value1, DateTimeOffset value2)
            {
                Assume.That((value1 - value2).Duration() <= new TimeSpan(0, 1, 0));

                Assert.That(value1, Is.EqualTo(value2).Within(1).Minutes);
            }

            [Theory]
            public void NegativeEqualityTestWithTolerance(DateTimeOffset value1, DateTimeOffset value2)
            {
                Assume.That((value1 - value2).Duration() > new TimeSpan(0, 1, 0));

                Assert.That(value1, Is.Not.EqualTo(value2).Within(1).Minutes);
            }

            [Theory]
            public void NegativeEqualityTestWithToleranceAndWithSameOffset(DateTimeOffset value1, DateTimeOffset value2)
            {
                Assume.That((value1 - value2).Duration() > new TimeSpan(0, 1, 0));

                Assert.That(value1, Is.Not.EqualTo(value2).Within(1).Minutes.WithSameOffset);
            }

            [Theory]
            public void PositiveEqualityTestWithToleranceAndWithSameOffset(DateTimeOffset value1, DateTimeOffset value2)
            {
                Assume.That((value1 - value2).Duration() <= new TimeSpan(0, 1, 0));
                Assume.That(value1.Offset == value2.Offset);

                Assert.That(value1, Is.EqualTo(value2).Within(1).Minutes.WithSameOffset);
            }

            [Theory]
            public void NegativeEqualityTestWithinToleranceAndWithSameOffset(DateTimeOffset value1, DateTimeOffset value2)
            {
                Assume.That((value1 - value2).Duration() <= new TimeSpan(0, 1, 0));
                Assume.That(value1.Offset != value2.Offset);

                Assert.That(value1, Is.Not.EqualTo(value2).Within(1).Minutes.WithSameOffset);
            }
        }

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
                var actual = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(5).Minutes);
            }

            [Test]
            public void CanMatchDatesWithinSeconds()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(300).Seconds);
            }

            [Test]
            public void CanMatchDatesWithinMilliseconds()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(300_000).Milliseconds);
            }

            [Test]
            public void CanMatchDatesWithinTicks()
            {
                var expected = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 0, 0));
                var actual = new DateTimeOffset(new DateTime(2007, 4, 1, 13, 1, 0));
                Assert.That(actual, new EqualConstraint(expected).Within(TimeSpan.TicksPerMinute * 5).Ticks);
            }

            [Test]
            public void DTimeOffsetCanMatchDatesWithinHours()
            {
                var a = DateTimeOffset.Parse("2012-01-01T12:00Z");
                var b = DateTimeOffset.Parse("2012-01-01T12:01Z");
                Assert.That(a, Is.EqualTo(b).Within(TimeSpan.FromMinutes(2)));
            }

            [Test]
            public void FailsOnDateTimeOffsetOutsideOfTimeSpanTolerance()
            {
                var a = DateTimeOffset.Parse("2012-01-01T12:00Z");
                var b = DateTimeOffset.Parse("2012-01-01T12:01Z");
                var ex = Assert.Throws<AssertionException>(() => Assert.That(a, new EqualConstraint(b).Within(10).Seconds));
                Assert.That(ex?.Message, Does.Contain($"+/- {MsgUtils.FormatValue(TimeSpan.FromSeconds(10))}"));
                Assert.That(ex?.Message, Does.Contain($"{MsgUtils.FormatValue(TimeSpan.FromMinutes(1))}"));
            }
        }

        #endregion

        #region DictionaryEquality

        public class DictionaryEquality
        {
            [Test]
            public void CanMatchDictionaries_SameOrder()
            {
                Assert.That(new Dictionary<int, int> { { 0, 0 }, { 1, 1 }, { 2, 2 } }, Is.EqualTo(new Dictionary<int, int> { { 0, 0 }, { 1, 1 }, { 2, 2 } }));
            }

            [Test]
            public void CanMatchDictionaries_Failure()
            {
                Assert.Throws<AssertionException>(
                    () => Assert.That(new Dictionary<int, int> { { 0, 0 }, { 1, 5 }, { 2, 2 } }, Is.EqualTo(new Dictionary<int, int> { { 0, 0 }, { 1, 1 }, { 2, 2 } })));
            }

            [Test]
            public void CanMatchDictionaries_DifferentOrder()
            {
                Assert.That(new Dictionary<int, int> { { 0, 0 }, { 2, 2 }, { 1, 1 } }, Is.EqualTo(new Dictionary<int, int> { { 0, 0 }, { 1, 1 }, { 2, 2 } }));
            }

            [Test]
            public void CanMatchHashtables_SameOrder()
            {
                Assert.That(new Hashtable { { 0, 0 }, { 1, 1 }, { 2, 2 } }, Is.EqualTo(new Hashtable { { 0, 0 }, { 1, 1 }, { 2, 2 } }));
            }

            [Test]
            public void CanMatchHashtables_Failure()
            {
                Assert.Throws<AssertionException>(
                    () => Assert.That(new Hashtable { { 0, 0 }, { 1, 5 }, { 2, 2 } }, Is.EqualTo(new Hashtable { { 0, 0 }, { 1, 1 }, { 2, 2 } })));
            }

            [Test]
            public void CanMatchHashtables_DifferentOrder()
            {
                Assert.That(new Hashtable { { 0, 0 }, { 2, 2 }, { 1, 1 } }, Is.EqualTo(new Hashtable { { 0, 0 }, { 1, 1 }, { 2, 2 } }));
            }

            [Test]
            public void CanMatchHashtableWithDictionary()
            {
                Assert.That(new Dictionary<int, int> { { 0, 0 }, { 2, 2 }, { 1, 1 } }, Is.EqualTo(new Hashtable { { 0, 0 }, { 1, 1 }, { 2, 2 } }));
            }
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
                Assert.That(ex?.Message, Does.Contain("+/- 1 Ulps"));
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
                Assert.That(ex?.Message, Does.Contain("+/- 1 Ulps"));
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
                Assert.That(ex?.Message, Does.Contain("+/- 10.0d Percent"));
                var expectedPercentDiff = (10000 - (double)value) / 100;
                Assert.That(ex?.Message, Does.Contain($"{MsgUtils.FormatValue(expectedPercentDiff)} Percent"));
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
                Assert.That(ex?.Message, Does.Contain("+/- 10.0f Percent"));
                double expectedPercentDiff = (10000 - (float)value) / 100;
                Assert.That(ex?.Message, Does.Contain($"{MsgUtils.FormatValue(expectedPercentDiff)} Percent"));
            }

            [TestCase(1.21)]
            [TestCase(1.19)]
            public void FailsOnDoublesOutsideOfAbsoluteTolerance(object value)
            {
                const double tolerance = 0.001;
                var ex = Assert.Throws<AssertionException>(() => Assert.That(value, new EqualConstraint(1.2).Within(tolerance)));
                Assert.That(ex?.Message, Does.Contain($"+/- {MsgUtils.FormatValue(tolerance)}"));
                var expectedAbsoluteDiff = 1.2 - (double)value;
                Assert.That(ex?.Message, Does.Contain($"{MsgUtils.FormatValue(expectedAbsoluteDiff)}"));
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

            [Test]
            public void CanMatchNegativeZeroToZeroForDoubles()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(0d, Is.EqualTo(-0d).Within(1).Ulps);
                    Assert.That(-0d, Is.EqualTo(0d).Within(1).Ulps);
                });
            }

            [Test]
            public void CanMatchNegativeZeroToZeroForFloats()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(0f, Is.EqualTo(-0f).Within(1).Ulps);
                    Assert.That(-0f, Is.EqualTo(0f).Within(1).Ulps);
                });
            }
        }

        #endregion

        #region ObjectEquality

        public class ObjectEquality
        {
            [Test]
            public void CompareObjectsWithToleranceAsserts()
            {
                Assert.Throws<NotSupportedException>(() => Assert.That("abc", new EqualConstraint("abcd").Within(1)));
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
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4).Using(comparer));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void CanCompareUncomparableTypes()
            {
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
                Assert.That(2 + 2, Is.Not.EqualTo("4"));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
                var comparer = new ConvertibleComparer();
                Assert.That(2 + 2, Is.EqualTo("4").Using(comparer));
            }

            [Test]
            public void UsesProvidedEqualityComparer()
            {
                var comparer = new ObjectEqualityComparer();
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4).Using(comparer));
                    Assert.That(comparer.Called, "Comparer was not called");
                });
            }

            [Test]
            public void UsesProvidedEqualityComparerForExpectedIsString()
            {
                var comparer = new ObjectToStringEqualityComparer();
                Assert.Multiple(() =>
                {
                    Assert.That(4, Is.EqualTo("4").Using(comparer));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void UsesProvidedEqualityComparerForActualIsString()
            {
                var comparer = new ObjectToStringEqualityComparer();
                Assert.Multiple(() =>
                {
                    Assert.That("4", Is.EqualTo(4).Using(comparer));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void UsesProvidedComparerForExpectedIsString()
            {
                var comparer = new ObjectToStringComparer();
                Assert.Multiple(() =>
                {
                    Assert.That(4, Is.EqualTo("4").Using(comparer));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void UsesProvidedComparerForActualIsString()
            {
                var comparer = new ObjectToStringComparer();
                Assert.Multiple(() =>
                {
                    Assert.That("4", Is.EqualTo(4).Using(comparer));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void UsesProvidedGenericEqualityComparer()
            {
                var comparer = new GenericEqualityComparer<int>();
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4).Using(comparer));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void UsesProvidedGenericComparer()
            {
                var comparer = new GenericComparer<int>();
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4).Using(comparer));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void UsesProvidedGenericComparison()
            {
                var comparer = new GenericComparison<int>();
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4).Using(comparer.Delegate));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void UsesProvidedGenericEqualityComparison()
            {
                var comparer = new GenericEqualityComparison<int>();
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4).Using<int>(comparer.Delegate));
                    Assert.That(comparer.WasCalled, "Comparer was not called");
                });
            }

            [Test]
            public void UsesBooleanReturningDelegate()
            {
                Assert.That(2 + 2, Is.EqualTo(4).Using<int>((x, y) => x.Equals(y)));
            }

            [Test]
            public void UsesProvidedLambda_IntArgs()
            {
                Assert.That(2 + 2, Is.EqualTo(4).Using<int>((x, y) => x.CompareTo(y)));
            }

            [Test, SetCulture("en-US")]
            public void UsesProvidedLambda_StringArgs()
            {
                Assert.That("hello", Is.EqualTo("HELLO").Using<string>((x, y) => StringUtil.Compare(x, y, true)));
            }

            [Test]
            public void UsesProvidedListComparer()
            {
                var list1 = new List<int>() { 2, 3 };
                var list2 = new List<int>() { 3, 4 };

                var list11 = new List<List<int>>() { list1 };
                var list22 = new List<List<int>>() { list2 };
                var comparer = new IntListEqualComparer();

                Assert.That(list11, new CollectionEquivalentConstraint(list22).Using(comparer));
            }

            public class IntListEqualComparer : IEqualityComparer<List<int>>
            {
                public bool Equals(List<int>? x, List<int>? y)
                {
                    if (ReferenceEquals(x, y))
                        return true;
                    if (x is null || y is null)
                        return false;

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
                var array1 = new[] { 2, 3 };
                var array2 = new[] { 3, 4 };

                var list11 = new List<int[]>() { array1 };
                var list22 = new List<int[]>() { array2 };
                var comparer = new IntArrayEqualComparer();

                Assert.That(list11, new CollectionEquivalentConstraint(list22).Using(comparer));
            }

            public class IntArrayEqualComparer : IEqualityComparer<int[]>
            {
                public bool Equals(int[]? x, int[]? y)
                {
                    if (ReferenceEquals(x, y))
                        return true;
                    if (x is null || y is null)
                        return false;

                    return x.Length == y.Length;
                }

                public int GetHashCode(int[] obj)
                {
                    return obj.Length.GetHashCode();
                }
            }

            [Test]
            public void HasMemberHonorsUsingWhenCollectionsAreOfDifferentTypes()
            {
                ICollection strings = new List<string> { "1", "2", "3" };
                Assert.That(strings, Has.Member(2).Using<string, int>((s, i) => i.ToString() == s));
            }

            [Test, SetCulture("en-US")]
            public void UsesProvidedPredicateForItemComparison()
            {
                var expected = new[] { "yeti", "łysy", "rysiu" };
                var actual = new[] { "YETI", "Łysy", "RySiU" };

                Assert.That(actual, Is.EqualTo(expected).Using<string>((x, y) => StringUtil.StringsEqual(x, y, true)));
            }

            [Test]
            public void UsesProvidedPredicateForItemComparisonDifferentTypes()
            {
                var expected = new[] { 1, 2, 3 };
                var actual = new[] { "1", "2", "3" };

                Assert.That(actual, Is.EqualTo(expected).Using<string, int>((s, i) => i.ToString() == s));
            }
        }

        #endregion

        #region TypeEqualityMessages
        private static readonly string NL = Environment.NewLine;
        private static IEnumerable DifferentTypeSameValueTestData
        {
            get
            {
                var ptr = new System.IntPtr(0);
                var exampleTestA = new ExampleTest.ClassA(0);
                var exampleTestB = new ExampleTest.ClassB(0);
                var clipTestA = new ExampleTest.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Inner.Clip.ReallyLongClassNameShouldBeHere();
                var clipTestB = new ExampleTest.Clip.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Inner.Clip.ReallyLongClassNameShouldBeHere();
                yield return new object[] { 0, ptr };
                yield return new object[] { exampleTestA, exampleTestB };
                yield return new object[] { clipTestA, clipTestB };
            }
        }
        [Test]
        public void SameValueDifferentTypeExactMessageMatch()
        {
#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            var ex = Assert.Throws<AssertionException>(() => Assert.That(new IntPtr(0), Is.EqualTo(0)));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
            Assert.That(ex?.Message, Does.Contain("  Expected: 0 (Int32)" + NL + "  But was:  0 (IntPtr)" + NL));
        }

        private class Dummy
        {
            internal readonly int Value;

            public Dummy(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return "Dummy " + Value;
            }
        }

        private class Dummy1
        {
            internal readonly int Value;

            public Dummy1(int value)
            {
                Value = value;
            }

            public override string ToString()
            {
                return "Dummy " + Value;
            }
        }

        private class DummyGenericClass<T>
        {
            private readonly object _obj;

            public DummyGenericClass(object obj)
            {
                _obj = obj;
            }

            public override string? ToString()
            {
                return _obj.ToString();
            }
        }

        [Test]
        public void TestSameValueDifferentTypeUsingGenericTypes()
        {
            var d1 = new Dummy(12);
            var d2 = new Dummy1(12);
            var dc1 = new DummyGenericClass<Dummy>(d1);
            var dc2 = new DummyGenericClass<Dummy1>(d2);

#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            var ex = Assert.Throws<AssertionException>(() => Assert.That(dc2, Is.EqualTo(dc1)));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
            var expectedMsg =
                "  Expected: <Dummy 12> (EqualConstraintTests+DummyGenericClass`1[EqualConstraintTests+Dummy])" + Environment.NewLine +
                "  But was:  <Dummy 12> (EqualConstraintTests+DummyGenericClass`1[EqualConstraintTests+Dummy1])" + Environment.NewLine;

            Assert.That(ex?.Message, Does.Contain(expectedMsg));
        }

        [Test]
        public void SameValueAndTypeButDifferentReferenceShowNotShowTypeDifference()
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(Is.Zero, Is.EqualTo(Is.Zero)));
            Assert.That(ex?.Message, Does.Contain("  Expected: <<equal 0>>" + NL + "  But was:  <<equal 0>>" + NL));
        }

        [Test, TestCaseSource(nameof(DifferentTypeSameValueTestData))]
        public void SameValueDifferentTypeRegexMatch(object expected, object actual)
        {
            var ex = Assert.Throws<AssertionException>(() => Assert.That(actual, Is.EqualTo(expected)));
            Assert.That(ex?.Message, Does.Match(@"\s*Expected\s*:\s*.*\s*\(.+\)\r?\n\s*But\s*was\s*:\s*.*\s*\(.+\)"));
        }
    }
    namespace ExampleTest.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Inner.Clip
    {
        internal class ReallyLongClassNameShouldBeHere
        {
            public override bool Equals(object? obj)
            {
                if (obj is null || GetType() != obj.GetType())
                {
                    return false;
                }
                return obj.ToString() == ToString();
            }
            public override int GetHashCode()
            {
                return "a".GetHashCode();
            }
            public override string ToString()
            {
                return "a";
            }
        }
    }
    namespace ExampleTest.Clip.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Inner.Outer.Middle.Inner.Clip
    {
        internal class ReallyLongClassNameShouldBeHere
        {
            public override bool Equals(object? obj)
            {
                if (obj is null || GetType() != obj.GetType())
                {
                    return false;
                }
                return obj.ToString() == ToString();
            }
            public override int GetHashCode()
            {
                return "a".GetHashCode();
            }

            public override string ToString()
            {
                return "a";
            }
        }
    }
    namespace ExampleTest
    {
        internal class BaseTest
        {
            private readonly int _value;
            public BaseTest()
            {
                _value = 0;
            }
            public BaseTest(int value)
            {
                _value = value;
            }
            public override bool Equals(object? obj)
            {
                if (obj is null || GetType() != obj.GetType())
                {
                    return false;
                }
                return _value.Equals(((BaseTest)obj)._value);
            }

            public override string ToString()
            {
                return _value.ToString();
            }

            public override int GetHashCode()
            {
                return _value.GetHashCode();
            }
        }

        internal class ClassA : BaseTest
        {
            public ClassA(int x) : base(x)
            {
            }
        }

        internal class ClassB : BaseTest
        {
            public ClassB(int x) : base(x)
            {
            }
        }
    }
    #endregion

    /// <summary>
    /// ConvertibleComparer is used in testing to ensure that objects
    /// of different types can be compared when appropriate.
    /// </summary>
    /// <remark>Introduced when testing issue 1897.
    /// https://github.com/nunit/nunit/issues/1897
    /// </remark>
    public class ConvertibleComparer : IComparer<IConvertible>
    {
        public int Compare(IConvertible? x, IConvertible? y)
        {
            if (ReferenceEquals(x, y))
                return 0;
            if (x is null)
                return -1;
            if (y is null)
                return 1;

            var str1 = Convert.ToString(x, CultureInfo.InvariantCulture);
            var str2 = Convert.ToString(y, CultureInfo.InvariantCulture);
            return string.Compare(str1, str2, StringComparison.Ordinal);
        }
    }
}
