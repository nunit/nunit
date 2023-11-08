// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace NUnit.Framework.Tests.Constraints
{
    /// <summary>
    /// Summary description for MsgUtilTests.
    /// </summary>
    [TestFixture]
    public static class MsgUtilTests
    {
        #region FormatValue
        private class CustomFormattableType
        {
        }

        [Test]
        public static void FormatValue_ContextualCustomFormatterInvoked_FactoryArg()
        {
            TestContext.AddFormatter(next => val => (val is CustomFormattableType) ? "custom_formatted" : next(val));

            Assert.That(MsgUtils.FormatValue(new CustomFormattableType()), Is.EqualTo("custom_formatted"));
        }

        [Test]
        public static void FormatValue_ContextualCustomFormatterNotInvokedForNull()
        {
            // If this factory is actually called with null, it will throw
            TestContext.AddFormatter(next => val => (val.GetType() == typeof(CustomFormattableType)) ? val.ToString()! : next(val));

            Assert.That(MsgUtils.FormatValue(null), Is.EqualTo("null"));
        }

        [Test]
        public static void FormatValue_ContextualCustomFormatterInvoked_FormatterArg()
        {
            TestContext.AddFormatter<CustomFormattableType>(val => "custom_formatted_using_type");

            Assert.That(MsgUtils.FormatValue(new CustomFormattableType()), Is.EqualTo("custom_formatted_using_type"));
        }

        [Test]
        public static void FormatValue_IntegerIsWrittenAsIs()
        {
            Assert.That(MsgUtils.FormatValue(42), Is.EqualTo("42"));
        }

        [Test]
        public static void FormatValue_StringIsWrittenWithQuotes()
        {
            Assert.That(MsgUtils.FormatValue("Hello"), Is.EqualTo("\"Hello\""));
        }

        // This test currently fails because control character replacement is
        // done at a higher level...
        // TODO: See if we should do it at a lower level
        //            [Test]
        //            public static void ControlCharactersInStringsAreEscaped()
        //            {
        //                WriteValue("Best Wishes,\r\n\tCharlie\r\n");
        //                Assert.That(writer.ToString(), Is.Is.EqualTo("\"Best Wishes,\\r\\n\\tCharlie\\r\\n\""));
        //            }

        [Test]
        public static void FormatValue_FloatIsWrittenWithTrailingF()
        {
            Assert.That(MsgUtils.FormatValue(0.5f), Is.EqualTo("0.5f"));
        }

        [Test]
        public static void FormatValue_FloatIsWrittenToNineDigits()
        {
            string s = MsgUtils.FormatValue(0.33333333333333f);
            int digits = s.Length - 3;   // 0.dddddddddf
            Assert.That(digits, Is.EqualTo(9));
        }

        [Test]
        public static void FormatValue_DoubleIsWrittenWithTrailingD()
        {
            Assert.That(MsgUtils.FormatValue(0.5d), Is.EqualTo("0.5d"));
        }

        [Test]
        public static void FormatValue_DoubleIsWrittenToSeventeenDigits()
        {
            string s = MsgUtils.FormatValue(0.33333333333333333333333333333333333333333333d);
            Assert.That(s, Has.Length.EqualTo(20)); // add 3 for leading 0, decimal and trailing d
        }

        [Test]
        public static void FormatValue_DecimalIsWrittenWithTrailingM()
        {
            Assert.That(MsgUtils.FormatValue(0.5m), Is.EqualTo("0.5m"));
        }

        [Test]
        public static void FormatValue_DecimalIsWrittenToTwentyNineDigits()
        {
            Assert.That(MsgUtils.FormatValue(12345678901234567890123456789m), Is.EqualTo("12345678901234567890123456789m"));
        }

        [Test]
        public static void FormatValue_DateTimeTest()
        {
            Assert.That(MsgUtils.FormatValue(new DateTime(2007, 7, 4, 9, 15, 30, 123)), Is.EqualTo("2007-07-04 09:15:30.123"));
        }

        [Test]
        public static void FormatValue_DateTimeOffsetTest()
        {
            Assert.That(MsgUtils.FormatValue(new DateTimeOffset(2007, 7, 4, 9, 15, 30, 123, TimeSpan.FromHours(8))), Is.EqualTo("2007-07-04 09:15:30.123+08:00"));
        }

        [TestCase('a', "'a'")]
        [TestCase('h', "'h'")]
        [TestCase('z', "'z'")]
        public static void FormatValue_CharTest(char c, string expected)
        {
            Assert.That(MsgUtils.FormatValue(c), Is.EqualTo(expected));
        }

        [TestCase("First", null, "[\"First\", null]")]
        [TestCase("First", "Second", "[\"First\", \"Second\"]")]
        [TestCase(123, 'h', "[123, 'h']")]
        public static void FormatValue_KeyValuePairTest(object key, object? value, string expectedResult)
        {
            string s = MsgUtils.FormatValue(new KeyValuePair<object, object?>(key, value));
            Assert.That(s, Is.EqualTo(expectedResult));
        }

        [TestCase("First", null, "[\"First\", null]")]
        [TestCase("First", "Second", "[\"First\", \"Second\"]")]
        [TestCase(123, 'h', "[123, 'h']")]
        public static void FormatValue_DirectoryEntryTest(object key, object? value, string expectedResult)
        {
            string s = MsgUtils.FormatValue(new DictionaryEntry(key, value));
            Assert.That(s, Is.EqualTo(expectedResult));
        }

        [Test]
        public static void FormatValue_EmptyValueTupleTest()
        {
            string s = MsgUtils.FormatValue(ValueTuple.Create());
            Assert.That(s, Is.EqualTo("()"));
        }

        [Test]
        public static void FormatValue_OneElementValueTupleTest()
        {
            string s = MsgUtils.FormatValue(ValueTuple.Create("Hello"));
            Assert.That(s, Is.EqualTo("(\"Hello\")"));
        }

        [Test]
        public static void FormatValue_TwoElementsValueTupleTest()
        {
            string s = MsgUtils.FormatValue(("Hello", 123));
            Assert.That(s, Is.EqualTo("(\"Hello\", 123)"));
        }

        [Test]
        public static void FormatValue_ThreeElementsValueTupleTest()
        {
            string s = MsgUtils.FormatValue(("Hello", 123, 'a'));
            Assert.That(s, Is.EqualTo("(\"Hello\", 123, 'a')"));
        }

        [Test]
        public static void FormatValue_EightElementsValueTupleTest()
        {
            var tuple = (1, 2, 3, 4, 5, 6, 7, 8);

            string s = MsgUtils.FormatValue(tuple);
            Assert.That(s, Is.EqualTo("(1, 2, 3, 4, 5, 6, 7, 8)"));
        }

        [Test]
        public static void FormatValue_EightElementsValueTupleNestedTest()
        {
            var tuple = (1, 2, 3, 4, 5, 6, 7, (8, "9"));

            string s = MsgUtils.FormatValue(tuple);
            Assert.That(s, Is.EqualTo("(1, 2, 3, 4, 5, 6, 7, (8, \"9\"))"));
        }

        [Test]
        public static void FormatValue_FifteenElementsValueTupleTest()
        {
            var tuple = (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, "12", 13, 14, "15");

            string s = MsgUtils.FormatValue(tuple);
            Assert.That(s, Is.EqualTo("(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, \"12\", 13, 14, \"15\")"));
        }

        [Test]
        public static void FormatValue_OneElementTupleTest()
        {
            string s = MsgUtils.FormatValue(Tuple.Create("Hello"));
            Assert.That(s, Is.EqualTo("(\"Hello\")"));
        }

        [Test]
        public static void FormatValue_TwoElementsTupleTest()
        {
            string s = MsgUtils.FormatValue(Tuple.Create("Hello", 123));
            Assert.That(s, Is.EqualTo("(\"Hello\", 123)"));
        }

        [Test]
        public static void FormatValue_ThreeElementsTupleTest()
        {
            string s = MsgUtils.FormatValue(Tuple.Create("Hello", 123, 'a'));
            Assert.That(s, Is.EqualTo("(\"Hello\", 123, 'a')"));
        }

        [Test]
        public static void FormatValue_EightElementsTupleTest()
        {
            var tuple = Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8);
            string s = MsgUtils.FormatValue(tuple);
            Assert.That(s, Is.EqualTo("(1, 2, 3, 4, 5, 6, 7, 8)"));
        }

        [Test]
        public static void FormatValue_EightElementsTupleNestedTest()
        {
            var tuple = Tuple.Create(1, 2, 3, 4, 5, 6, 7, Tuple.Create(8, "9"));
            string s = MsgUtils.FormatValue(tuple);
            Assert.That(s, Is.EqualTo("(1, 2, 3, 4, 5, 6, 7, (8, \"9\"))"));
        }

        [Test]
        public static void FormatValue_FifteenElementsTupleTest()
        {
            var tupleLastElements = Tuple.Create(8, 9, 10, 11, "12", 13, 14, "15");
            var tuple = new Tuple<int, int, int, int, int, int, int, Tuple<int, int, int, int, string, int, int, Tuple<string>>>
                (1, 2, 3, 4, 5, 6, 7, tupleLastElements);

            string s = MsgUtils.FormatValue(tuple);
            Assert.That(s, Is.EqualTo("(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, \"12\", 13, 14, \"15\")"));
        }

        #endregion

        #region EscapeControlChars

        [TestCase("\n", "\\n")]
        [TestCase("\n\n", "\\n\\n")]
        [TestCase("\n\n\n", "\\n\\n\\n")]
        [TestCase("\r", "\\r")]
        [TestCase("\r\r", "\\r\\r")]
        [TestCase("\r\r\r", "\\r\\r\\r")]
        [TestCase("\r\n", "\\r\\n")]
        [TestCase("\n\r", "\\n\\r")]
        [TestCase("This is a\rtest message", "This is a\\rtest message")]
        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("\t", "\\t")]
        [TestCase("\t\n", "\\t\\n")]
        [TestCase("\\r\\n", "\\\\r\\\\n")]
        // TODO: Figure out why this fails in Mono
        //[TestCase("\0", "\\0")]
        [TestCase("\a", "\\a")]
        [TestCase("\b", "\\b")]
        [TestCase("\f", "\\f")]
        [TestCase("\v", "\\v")]
        [TestCase("\x0085", "\\x0085", Description = "Next line character")]
        [TestCase("\x2028", "\\x2028", Description = "Line separator character")]
        [TestCase("\x2029", "\\x2029", Description = "Paragraph separator character")]
        public static void EscapeControlCharsTest(string? input, string? expected)
        {
            Assert.That(MsgUtils.EscapeControlChars(input), Is.EqualTo(expected));
        }

        [Test]
        public static void EscapeNullCharInString()
        {
            Assert.That(MsgUtils.EscapeControlChars("\0"), Is.EqualTo("\\0"));
        }

        #endregion
        #region EscapeNullChars
        [TestCase("\n", "\n")]
        [TestCase("\r", "\r")]
        [TestCase("\r\n\r", "\r\n\r")]
        [TestCase("\f", "\f")]
        [TestCase("\b", "\b")]
        public static void DoNotEscapeNonNullControlChars(string input, string expected)
        {
            Assert.That(MsgUtils.EscapeNullCharacters(input), Is.EqualTo(expected));
        }

        [Test]
        public static void EscapesNullControlChars()
        {
            Assert.That(MsgUtils.EscapeNullCharacters("\0"), Is.EqualTo("\\0"));
        }

        #endregion
        #region ClipString

        private const string S52 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        [TestCase(S52, 52, 0, S52, TestName = "NoClippingNeeded")]
        [TestCase(S52, 29, 0, "abcdefghijklmnopqrstuvwxyz...", TestName = "ClipAtEnd")]
        [TestCase(S52, 29, 26, "...ABCDEFGHIJKLMNOPQRSTUVWXYZ", TestName = "ClipAtStart")]
        [TestCase(S52, 28, 26, "...ABCDEFGHIJKLMNOPQRSTUV...", TestName = "ClipAtStartAndEnd")]
        public static void TestClipString(string input, int max, int start, string result)
        {
            System.Console.WriteLine("input=  \"{0}\"", input);
            System.Console.WriteLine("result= \"{0}\"", result);
            Assert.That(MsgUtils.ClipString(input, max, start), Is.EqualTo(result));
        }

        #endregion

        #region ClipExpectedAndActual

        [Test]
        public static void ClipExpectedAndActual_StringsFitInLine()
        {
            string eClip = S52;
            string aClip = "abcde";
            MsgUtils.ClipExpectedAndActual(ref eClip, ref aClip, 52, 5);
            Assert.That(eClip, Is.EqualTo(S52));
            Assert.That(aClip, Is.EqualTo("abcde"));

            eClip = S52;
            aClip = "abcdefghijklmno?qrstuvwxyz";
            MsgUtils.ClipExpectedAndActual(ref eClip, ref aClip, 52, 15);
            Assert.That(eClip, Is.EqualTo(S52));
            Assert.That(aClip, Is.EqualTo("abcdefghijklmno?qrstuvwxyz"));
        }

        [Test]
        public static void ClipExpectedAndActual_StringTailsFitInLine()
        {
            string s1 = S52;
            string s2 = S52.Replace('Z', '?');
            MsgUtils.ClipExpectedAndActual(ref s1, ref s2, 29, 51);
            Assert.That(s1, Is.EqualTo("...ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        }

        [Test]
        public static void ClipExpectedAndActual_StringsDoNotFitInLine()
        {
            string s1 = S52;
            string s2 = "abcdefghij";
            MsgUtils.ClipExpectedAndActual(ref s1, ref s2, 29, 10);
            Assert.That(s1, Is.EqualTo("abcdefghijklmnopqrstuvwxyz..."));
            Assert.That(s2, Is.EqualTo("abcdefghij"));

            s1 = S52;
            s2 = "abcdefghijklmno?qrstuvwxyz";
            MsgUtils.ClipExpectedAndActual(ref s1, ref s2, 25, 15);
            Assert.That(s1, Is.EqualTo("...efghijklmnopqrstuvw..."));
            Assert.That(s2, Is.EqualTo("...efghijklmno?qrstuvwxyz"));
        }

        #endregion
    }
}
