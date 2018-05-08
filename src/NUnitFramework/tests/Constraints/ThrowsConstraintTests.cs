// ***********************************************************************
// Copyright (c) 2009 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Internal;
using NUnit.TestUtilities;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class ThrowsConstraintTest_ExactType : ThrowsConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new ThrowsConstraint(
                new ExceptionTypeConstraint(typeof(ArgumentException)));
            ExpectedDescription = "<System.ArgumentException>";
            StringRepresentation = "<throws <typeof System.ArgumentException>>";
        }

        static readonly object[] SuccessData =
        {
            new TestDelegate( TestDelegates.ThrowsArgumentException )
        };

        static readonly object[] FailureData =
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException: my message" + Environment.NewLine ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsSystemException ), "<System.Exception: my message" + Environment.NewLine )
        };
    }

    [TestFixture]
    public class ThrowsConstraintTest_InstanceOfType : ThrowsConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new ThrowsConstraint(
                new InstanceOfTypeConstraint(typeof(TestDelegates.BaseException)));
            ExpectedDescription = "instance of <NUnit.TestUtilities.TestDelegates+BaseException>";
            StringRepresentation = "<throws <instanceof NUnit.TestUtilities.TestDelegates+BaseException>>";
        }

        static object[] SuccessData = new object[]
        {
            new TestDelegate( TestDelegates.ThrowsBaseException ),
            new TestDelegate( TestDelegates.ThrowsDerivedException )
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsArgumentException ), $"<System.ArgumentException: myMessage{Environment.NewLine}Parameter name: myParam" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException: my message" )
        };
    }

    public class ThrowsConstraintTest_WithConstraint : ThrowsConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new ThrowsConstraint(
                new AndConstraint(
                    new ExceptionTypeConstraint(typeof(ArgumentException)),
                    new PropertyConstraint("ParamName", new EqualConstraint("myParam"))));
            ExpectedDescription = @"<System.ArgumentException> and property ParamName equal to ""myParam""";
            StringRepresentation = @"<throws <and <typeof System.ArgumentException> <property ParamName <equal ""myParam"">>>>";
        }

        static readonly object[] SuccessData =
        {
            new TestDelegate( TestDelegates.ThrowsArgumentException )
        };

        static readonly object[] FailureData =
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException: my message" + Environment.NewLine ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsSystemException ), "<System.Exception: my message" + Environment.NewLine )
        };
    }

    public abstract class ThrowsConstraintTestBase : ConstraintTestBaseNoData
    {
        [Test, TestCaseSource("SuccessData")]
        public void SucceedsWithGoodValues(object value)
        {
            var constraintResult = TheConstraint.ApplyTo(value);
            if (!constraintResult.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo(writer);
                Assert.Fail(writer.ToString());
            }
        }

        [Test, TestCaseSource("FailureData"), SetUICulture("en-US")]
        public void FailsWithBadValues(object badValue, string message)
        {
            string NL = Environment.NewLine;

            var constraintResult = TheConstraint.ApplyTo(badValue);
            Assert.IsFalse(constraintResult.IsSuccess);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Does.StartWith(
                TextMessageWriter.Pfx_Expected + ExpectedDescription + NL +
                TextMessageWriter.Pfx_Actual + message));
        }
    }
}
