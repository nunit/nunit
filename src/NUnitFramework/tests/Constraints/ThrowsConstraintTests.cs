// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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
            theConstraint = new ThrowsConstraint(
                new ExceptionTypeConstraint(typeof(ArgumentException)));
            expectedDescription = "<System.ArgumentException>";
            stringRepresentation = "<throws <typeof System.ArgumentException>>";
        }

        static object[] SuccessData = new object[]
        {
            new TestDelegate( TestDelegates.ThrowsArgumentException )
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException: my message" + Env.NewLine ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsSystemException ), "<System.Exception: my message" + Env.NewLine )
        };
    }

    [TestFixture]
    public class ThrowsConstraintTest_InstanceOfType : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new ThrowsConstraint(
                new InstanceOfTypeConstraint(typeof(TestDelegates.BaseException)));
            expectedDescription = "instance of <NUnit.TestUtilities.TestDelegates+BaseException>";
            stringRepresentation = "<throws <instanceof NUnit.TestUtilities.TestDelegates+BaseException>>";
        }

        static object[] SuccessData = new object[]
        {
            new TestDelegate( TestDelegates.ThrowsBaseException ),
            new TestDelegate( TestDelegates.ThrowsDerivedException )
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsArgumentException ), "<System.ArgumentException>" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException>" )
        };
    }

// TODO: Find a different example for use with NETCF - ArgumentException does not have a ParamName member
#if !NETCF && !SILVERLIGHT
    public class ThrowsConstraintTest_WithConstraint : ThrowsConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new ThrowsConstraint(
                new AndConstraint(
                    new ExceptionTypeConstraint(typeof(ArgumentException)),
                    new PropertyConstraint("ParamName", new EqualConstraint("myParam"))));
            expectedDescription = @"<System.ArgumentException> and property ParamName equal to ""myParam""";
            stringRepresentation = @"<throws <and <typeof System.ArgumentException> <property ParamName <equal ""myParam"">>>>";
        }

        static object[] SuccessData = new object[]
        {
            new TestDelegate( TestDelegates.ThrowsArgumentException )
        };

        static object[] FailureData = new object[]
        {
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNullReferenceException ), "<System.NullReferenceException: my message" + Env.NewLine ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsNothing ), "no exception thrown" ),
            new TestCaseData( new TestDelegate( TestDelegates.ThrowsSystemException ), "<System.Exception: my message" + Env.NewLine )
        };
    }
#endif

    public abstract class ThrowsConstraintTestBase : ConstraintTestBaseNoData
    {
        [Test, TestCaseSource("SuccessData")]
        public void SucceedsWithGoodValues(object value)
        {
            var constraintResult = theConstraint.ApplyTo(value);
            if (!constraintResult.IsSuccess)
            {
                MessageWriter writer = new TextMessageWriter();
                constraintResult.WriteMessageTo(writer);
                Assert.Fail(writer.ToString());
            }
        }

        [Test, TestCaseSource("FailureData")]
        public void FailsWithBadValues(object badValue, string message)
        {
            string NL = Env.NewLine;

            var constraintResult = theConstraint.ApplyTo(badValue);
            Assert.IsFalse(constraintResult.IsSuccess);

            TextMessageWriter writer = new TextMessageWriter();
            constraintResult.WriteMessageTo(writer);
            Assert.That(writer.ToString(), Does.StartWith(
                TextMessageWriter.Pfx_Expected + expectedDescription + NL +
                TextMessageWriter.Pfx_Actual + message));
        }
    }
}
