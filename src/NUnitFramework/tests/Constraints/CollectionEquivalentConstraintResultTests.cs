// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class CollectionEquivalentConstraintResultTests
    {
        private readonly List<string> _expected = new List<string> { "one", "two" };

        private CollectionEquivalentConstraint _constraint;
        private TextMessageWriter _writer;

        [SetUp]
        public void TestSetup()
        {
            _constraint = new CollectionEquivalentConstraint(_expected);
            _writer = new TextMessageWriter();
        }

        [TearDown]
        public void TestTearDown()
        {
            _writer.Close();
        }

        [Test]
        public void TestOneMissingValue()
        {
            List<object> actualList = new List<object>() { "one" };

            ConstraintResult cr = _constraint.ApplyTo(actualList);
            cr.WriteMessageTo(_writer);

            string expectedMsg =
                "  Expected: equivalent to < \"one\", \"two\" >" + Environment.NewLine +
                "  But was:  < \"one\" >" + Environment.NewLine +
                "  Missing (1): < \"two\" >" + Environment.NewLine;
            Assert.That(_writer.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void TestTwoMissingValues()
        {
            List<object> actualList = new List<object>() { };

            ConstraintResult cr = _constraint.ApplyTo(actualList);
            cr.WriteMessageTo(_writer);

            string expectedMsg =
                "  Expected: equivalent to < \"one\", \"two\" >" + Environment.NewLine +
                "  But was:  <empty>" + Environment.NewLine +
                "  Missing (2): < \"one\", \"two\" >" + Environment.NewLine;
            Assert.That(_writer.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void TestTwoExtraValues()
        {
            List<object> actualList = new List<object>() { "one", "two", "three", "four" };
            ConstraintResult cr = _constraint.ApplyTo(actualList);
            cr.WriteMessageTo(_writer);

            string expectedMsg =
                "  Expected: equivalent to < \"one\", \"two\" >" + Environment.NewLine +
                "  But was:  < \"one\", \"two\", \"three\", \"four\" >" + Environment.NewLine +
                "  Extra (2): < \"three\", \"four\" >" + Environment.NewLine;
            Assert.That(_writer.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void TestOneExtraValue()
        {
            List<object> actualList = new List<object>() { "three", "one", "two" };

            ConstraintResult cr = _constraint.ApplyTo(actualList);
            cr.WriteMessageTo(_writer);

            string expectedMsg =
                "  Expected: equivalent to < \"one\", \"two\" >" + Environment.NewLine +
                "  But was:  < \"three\", \"one\", \"two\" >" + Environment.NewLine +
                "  Extra (1): < \"three\" >" + Environment.NewLine;
            Assert.That(_writer.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void TestOneExtraOneMissing()
        {
            List<object> actualList = new List<object>() { "three", "one", };

            ConstraintResult cr = _constraint.ApplyTo(actualList);
            cr.WriteMessageTo(_writer);

            string expectedMsg =
                "  Expected: equivalent to < \"one\", \"two\" >" + Environment.NewLine +
                "  But was:  < \"three\", \"one\" >" + Environment.NewLine +
                "  Missing (1): < \"two\" >" + Environment.NewLine +
                "  Extra (1): < \"three\" >" + Environment.NewLine;
            Assert.That(_writer.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void TestOneExtraValueDueToUnexpectedRepeatedValue()
        {
            List<object> actualList = new List<object>() { "one", "two", "two" };

            ConstraintResult cr = _constraint.ApplyTo(actualList);
            cr.WriteMessageTo(_writer);

            string expectedMsg =
                "  Expected: equivalent to < \"one\", \"two\" >" + Environment.NewLine +
                "  But was:  < \"one\", \"two\", \"two\" >" + Environment.NewLine +
                "  Extra (1): < \"two\" >" + Environment.NewLine;
            Assert.That(_writer.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void TestOnlyDisplaysUpToTenDifferences()
        {
            List<object> actualList = new List<object>() { "one", "two", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven" };

            ConstraintResult cr = _constraint.ApplyTo(actualList);
            cr.WriteMessageTo(_writer);

            string expectedMsg =
                "  Expected: equivalent to < \"one\", \"two\" >" + Environment.NewLine +
                "  But was:  < \"one\", \"two\", \"one\", \"two\", \"three\", \"four\", \"five\", \"six\", \"seven\", \"eight\"... >" + Environment.NewLine +
                "  Extra (11): < \"one\", \"two\", \"three\", \"four\", \"five\", \"six\", \"seven\", \"eight\", \"nine\", \"ten\"... >" + Environment.NewLine;
            Assert.That(_writer.ToString(), Is.EqualTo(expectedMsg));
        }

        [Test]
        public void TestExceptionThrownWhenCollectionTallyNull()
        {
            List<object> actualList = new List<object>() { "one", "two", "two" };

            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = new CollectionEquivalentConstraintResult(_constraint, null!, actualList, false);
            });
        }
    }
}
