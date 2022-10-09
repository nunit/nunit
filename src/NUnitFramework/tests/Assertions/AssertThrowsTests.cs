// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

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

        [Test, SetUICulture("en-US")]
        public void DerivedExceptionThrown()
        {
            var ex = CatchException(() =>
                Assert.Throws<Exception>(TestDelegates.ThrowsArgumentException));

            Assert.That(ex.Message, Does.StartWith(
                "  Expected: <System.Exception>" + Environment.NewLine +
                "  But was:  <System.ArgumentException: myMessage"));

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
