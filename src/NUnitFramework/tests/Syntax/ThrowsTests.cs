// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.Framework.Syntax
{
    [TestFixture]
    public class ThrowsTests
    {
        [Test]
        public void ThrowsException()
        {
            IResolveConstraint expr = Throws.Exception;
            Assert.AreEqual(
                "<throwsexception>",
                expr.Resolve().ToString());
        }

        [Test]
        public void ThrowsExceptionWithConstraint()
        {
            IResolveConstraint expr = Throws.Exception.With.Property("ParamName").EqualTo("myParam");
            Assert.AreEqual(
                @"<throws <property ParamName <equal ""myParam"">>>",
                expr.Resolve().ToString());
        }

        [Test]
        public void ThrowsExceptionTypeOf()
        {
            IResolveConstraint expr = Throws.Exception.TypeOf(typeof(ArgumentException));
            Assert.AreEqual(
                "<throws <typeof System.ArgumentException>>",
                expr.Resolve().ToString());
        }

        [Test]
        public void ThrowsTypeOf()
        {
            IResolveConstraint expr = Throws.TypeOf(typeof(ArgumentException));
            Assert.AreEqual(
                "<throws <typeof System.ArgumentException>>",
                expr.Resolve().ToString());
        }

        [Test]
        public void ThrowsTypeOfAndConstraint()
        {
            IResolveConstraint expr = Throws.TypeOf(typeof(ArgumentException)).And.Property("ParamName").EqualTo("myParam");
            Assert.AreEqual(
                @"<throws <and <typeof System.ArgumentException> <property ParamName <equal ""myParam"">>>>",
                expr.Resolve().ToString());
        }

        [Test]
        public void ThrowsExceptionTypeOfAndConstraint()
        {
            IResolveConstraint expr = Throws.Exception.TypeOf(typeof(ArgumentException)).And.Property("ParamName").EqualTo("myParam");
            Assert.AreEqual(
                @"<throws <and <typeof System.ArgumentException> <property ParamName <equal ""myParam"">>>>",
                expr.Resolve().ToString());
        }

        [Test]
        public void ThrowsTypeOfWithConstraint()
        {
            IResolveConstraint expr = Throws.TypeOf(typeof(ArgumentException)).With.Property("ParamName").EqualTo("myParam");
            Assert.AreEqual(
                @"<throws <and <typeof System.ArgumentException> <property ParamName <equal ""myParam"">>>>",
                expr.Resolve().ToString());
        }

        [Test]
        public void ThrowsInstanceOf()
        {
            IResolveConstraint expr = Throws.InstanceOf(typeof(ArgumentException));
            Assert.AreEqual(
                "<throws <instanceof System.ArgumentException>>",
                expr.Resolve().ToString());
        }

        [Test]
        public void ThrowsExceptionInstanceOf()
        {
            IResolveConstraint expr = Throws.Exception.InstanceOf(typeof(ArgumentException));
            Assert.AreEqual(
                "<throws <instanceof System.ArgumentException>>",
                expr.Resolve().ToString());
        }

        [Test]
        public void DelegateThrowsException()
        {
            Assert.That(
                delegate { throw new Exception(); },
                Throws.Exception);
        }

        [Test]
        public void ArgumentNullException_ConstraintMatchesThrownArgumentNullException()
        {
            Assert.That(
                TestDelegates.ThrowsArgumentNullException, 
                Throws.ArgumentNullException);
        }

        [Test]
        public void LambdaThrowsException()
        {
            Assert.That(
                () => new MyClass(null),
                Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void LambdaThrowsExceptionWithMessage()
        {
            string expectedExceptionMessage = (new ArgumentNullException()).Message;
            Assert.That(
                () => new MyClass(null),
                Throws.InstanceOf<ArgumentNullException>()
                .And.Message.EqualTo(expectedExceptionMessage));
        }

        [Test]
        public void LambdaThrowsException_TestOutput()
        {
            var ex = CatchException(() =>
                Assert.That(TestDelegates.ThrowsNullReferenceException, Throws.TypeOf<ArgumentException>()));

            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Environment.NewLine));
        }

        [Test]
        public void LambdaThrowsExceptionInstanceOf_TestOutput()
        {
            var ex = CatchException(() =>
                Assert.That(TestDelegates.ThrowsNullReferenceException, Throws.InstanceOf<ArgumentException>()));

            Assert.That(ex.Message, Does.StartWith(
                "  Expected: instance of <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Environment.NewLine));
        }

        private Exception CatchException(TestDelegate del)
        {
            using (new TestExecutionContext.IsolatedContext())
            {
                try
                {
                    del();
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }
        }

        internal class MyClass
        {
            public MyClass(string s)
            {
                if (s == null)
                    throw new ArgumentNullException();
            }
        }
    }
}
