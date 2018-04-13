// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
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
            Assert.AreEqual(expectedMsg, _writer.ToString());
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
            Assert.AreEqual(expectedMsg, _writer.ToString());
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
            Assert.AreEqual(expectedMsg, _writer.ToString());
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
            Assert.AreEqual(expectedMsg, _writer.ToString());
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
            Assert.AreEqual(expectedMsg, _writer.ToString());
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
            Assert.AreEqual(expectedMsg, _writer.ToString());
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
            Assert.AreEqual(expectedMsg, _writer.ToString());
        }

        [Test]
        public void TestExceptionThrownWhenCollectionTallyNull()
        {
            List<object> actualList = new List<object>() { "one", "two", "two" };

            Assert.Throws<ArgumentNullException>(() =>
            {
                #pragma warning disable CS0219
                CollectionEquivalentConstraintResult cr = new CollectionEquivalentConstraintResult(_constraint, null, actualList, false);
            });
        }
    }
}
