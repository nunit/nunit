// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.IO;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class EqualTests
    {
        [Test]
        public void FailedStringMatchShowsFailurePosition()
        {
            CheckExceptionMessage(
                Assert.Throws<AssertionException>(() =>
                {
                    Assert.That("abcdgfe", Is.EqualTo("abcdefg"));
                }));
        }

        private static readonly string TestString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        [Test]
        public void LongStringsAreTruncated()
        {
            string expected = TestString;
            string actual = TestString.Replace('k', 'X');

            CheckExceptionMessage(
                Assert.Throws<AssertionException>(() =>
                {
                    Assert.That(actual, Is.EqualTo(expected));
                }));
        }

        [Test]
        public void LongStringsAreTruncatedAtBothEndsIfNecessary()
        {
            string expected = TestString;
            string actual = TestString.Replace('Z', '?');

            CheckExceptionMessage(
                Assert.Throws<AssertionException>(() =>
                {
                    Assert.That(actual, Is.EqualTo(expected));
                }));
        }

        [Test]
        public void LongStringsAreTruncatedAtFrontEndIfNecessary()
        {
            string expected = TestString;
            string actual = TestString + "+++++";

            CheckExceptionMessage(
                Assert.Throws<AssertionException>(() =>
                {
                    Assert.That(actual, Is.EqualTo(expected));
                }));
        }

        //        [Test]
        //        public void NamedAndUnnamedColorsCompareAsEqual()
        //        {
        //            EqualConstraint.SetConstraintForType(typeof(Color), typeof(SameColorAs));
        //            Assert.That(System.Drawing.Color.Red,
        //                Is.EqualTo(System.Drawing.Color.FromArgb(255, 0, 0)));
        //        }

        private static void CheckExceptionMessage(Exception ex)
        {
            string nl = Environment.NewLine;

            StringReader rdr = new StringReader(ex.Message);
            /* skip */
            rdr.ReadLine();
            rdr.ReadLine(); // Skip actualExpression, constraintExpression string
            string? expected = rdr.ReadLine();
            Assert.That(expected, Is.Not.Null);
            if (expected.Length > 11)
                expected = expected.Substring(11);
            string? actual = rdr.ReadLine();
            Assert.That(actual, Is.Not.Null);
            if (actual.Length > 11)
                actual = actual.Substring(11);
            string? line = rdr.ReadLine();
            Assert.That(line, new NotConstraint(new EqualConstraint(null)), "No caret line displayed");
            int caret = line.Substring(11).IndexOf('^');

            int minLength = Math.Min(expected.Length, actual.Length);
            int minMatch = Math.Min(caret, minLength);

            if (caret != minLength)
            {
                if (caret > minLength ||
                    expected.Substring(0, minMatch) != actual.Substring(0, minMatch) ||
                    expected[caret] == actual[caret])
                {
                    Assert.Fail("Message Error: Caret does not point at first mismatch..." + nl + ex.Message);
                }
            }

            if (expected.Length > 68 || actual.Length > 68 || caret > 68)
                Assert.Fail("Message Error: Strings are not truncated..." + nl + ex.Message);
        }

        //public class SameColorAs : Constraint
        //{
        //    private Color expectedColor;

        //    public SameColorAs(Color expectedColor)
        //    {
        //        this.expectedColor = expectedColor;
        //    }

        //    public override IConstraintResult Matches(object actual)
        //    {
        //        this.actual = actual;
        //        return new Result
        //                   {HasSucceeded = actual is Color && ((Color) actual).ToArgb() == expectedColor.ToArgb()};
        //    }

        //    private class Result : IConstraintResult
        //    {
        //        public bool HasSucceeded { get; set; }
        //        public object Actual { get; set; }
        //        public object Expected { get; set; }
        //        public string Name { get; set; }
        //        public string Description { get; set; }
        //        public string Predicate { get; set; }
        //        public string Modifier { get; set; }

        //        public void WriteDescriptionTo(MessageWriter writer)
        //        {
        //        }

        //        public void WriteMessageTo(MessageWriter writer)
        //        {
        //        }

        //        public void WriteActualValueTo(MessageWriter writer)
        //        {
        //        }
        //    }
        //}

        [Test]
        public void TestPropertyWithPrivateSetter()
        {
            SomeClass obj = new SomeClass();
            Assert.That(obj.BrokenProp, Is.EqualTo(string.Empty));
        }

        private class SomeClass
        {
            public string BrokenProp
            {
                get => string.Empty;
                private set { }
            }
        }
    }
}
