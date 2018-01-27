// ***********************************************************************
// Copyright (c) 2008-2016 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Assertions
{
    [TestFixture]
    public class AssertThrowsTests
    {
        [Test]
        public void ThrowsSucceedsWithDelegate()
        {
            Assert.Throws(typeof(ArgumentException), delegate { throw new ArgumentException(); });
        }

        [Test]
        public void AssertThrowsDoesNotDiscardOutput()
        {
            Console.WriteLine(1);
            Assert.Throws<Exception>(() =>
            {
                Console.WriteLine(2);
                TestContext.WriteLine(3);
                throw new Exception("test");
            });
            Console.WriteLine(4);

            var NL = Environment.NewLine;
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.Output,
                Is.EqualTo($"1{NL}2{NL}3{NL}4{NL}"));
        }

        [Test]
        public void ThrowsConstraintDoesNotDiscardOutput()
        {
            Console.WriteLine(1);
            Assert.That(
                () => { Console.WriteLine(2); TestContext.WriteLine(3); throw new Exception("test"); }, 
                Throws.Exception);
            Console.WriteLine(4);

            var NL = Environment.NewLine;
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.Output,
                Is.EqualTo($"1{NL}2{NL}3{NL}4{NL}"));
        }

        [Test]
        public void GenericThrowsSucceedsWithDelegate()
        {
            Assert.Throws<ArgumentException>(
                delegate { throw new ArgumentException(); });
        }

        [Test]
        public void ThrowsConstraintSucceedsWithDelegate()
        {
            // Without cast, delegate is ambiguous before C# 3.0.
            Assert.That((TestDelegate)delegate { throw new ArgumentException(); },
                    Throws.Exception.TypeOf<ArgumentException>());
        }

#if !NET20
        [Test]
        public void ThrowsSucceedsWithLambda()
        {
            Assert.Throws(typeof(ArgumentException), () => { throw new ArgumentException(); });
        }

        [Test]
        public void GenericThrowsSucceedsWithLambda()
        {
            Assert.Throws<ArgumentException>(() => { throw new ArgumentException(); });
        }

        [Test]
        public void ThrowsConstraintSucceedsWithLambda()
        {
            Assert.That(() => { throw new ArgumentException(); }, 
                Throws.Exception.TypeOf<ArgumentException>());
        }
#endif

        [Test]
        public void GenericThrowsReturnsCorrectException()
        {
            var ex = Assert.Throws<ArgumentException>(
                delegate { throw new ArgumentException("myMessage", "myParam"); }) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void ThrowsReturnsCorrectException()
        { 
            var ex = Assert.Throws(typeof(ArgumentException), 
                delegate { throw new ArgumentException("myMessage", "myParam"); } ) as ArgumentException;

            Assert.IsNotNull(ex, "No ArgumentException thrown");
            Assert.That(ex.Message, Does.StartWith("myMessage"));
            Assert.That(ex.ParamName, Is.EqualTo("myParam"));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void NoExceptionThrown()
        {
            var ex = CatchException(() => 
                Assert.Throws<ArgumentException>(TestDelegates.ThrowsNothing));

            Assert.That(ex.Message, Is.EqualTo(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  null" + Environment.NewLine));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void UnrelatedExceptionThrown()
        {
            var ex = CatchException(() => 
                Assert.Throws<ArgumentException>(TestDelegates.ThrowsNullReferenceException));

            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.NullReferenceException: my message" + Environment.NewLine ));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void BaseExceptionThrown()
        {
            var ex = CatchException(() => 
                Assert.Throws<ArgumentException>(TestDelegates.ThrowsSystemException));

            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.ArgumentException>" + Environment.NewLine +
                "  But was:  <System.Exception: my message" + Environment.NewLine ));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void DerivedExceptionThrown()
        {
            var ex = CatchException(() => 
                Assert.Throws<Exception>(TestDelegates.ThrowsArgumentException));

            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Environment.NewLine +
                "  But was:  <System.ArgumentException: myMessage" + Environment.NewLine + "Parameter name: myParam" + Environment.NewLine ));

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void AssertThrowsWrappingAssertFail()
        {
            Assert.Throws<AssertionException>(() => Assert.Fail());

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void ThrowsConstraintWrappingAssertFail()
        {
            Assert.That(() => { Assert.Fail(); }, 
                Throws.Exception.TypeOf<AssertionException>());

            CheckForSpuriousAssertionResults();
        }

        [Test]
        public void AssertDoesNotThrowSucceeds()
        {
            Assert.DoesNotThrow(TestDelegates.ThrowsNothing);
        }

        [Test]
        public void AssertDoesNotThrowFails()
        {
            var ex = CatchException(() => 
                Assert.DoesNotThrow(TestDelegates.ThrowsArgumentException));

            Assert.That(ex, Is.Not.Null.With.TypeOf<AssertionException>());

            CheckForSpuriousAssertionResults();
        }

        private static void CheckForSpuriousAssertionResults()
        {
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            Assert.That(result.AssertionResults.Count, Is.EqualTo(0),
                "Spurious result left by Assert.Fail()");
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
    }
}
