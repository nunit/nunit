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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Compatibility;
using ActualValueDelegate = NUnit.Framework.Constraints.ActualValueDelegate<object>;

namespace NUnit.Framework.Constraints
{
    [TestFixture, NonParallelizable]
    public class DelayedConstraintTests : ConstraintTestBase
    {
        // NOTE: This class tests the functioning of the DelayConstraint,
        // not the After syntax. The AfterTests class tests our syntax,
        // assuring that the proper constraint is generated. Here,we
        // set up constraints in the simplest way possible, often by
        // constructing the constraint class, and verify that they work.

        private const int DELAY = 100;
        private const int AFTER = 300;
        private const int POLLING = 50;
        private const int MIN = AFTER - 10;

        private static bool boolValue;
        private static List<int> list;
        private static string statusString;

        [SetUp]
        public void SetUp()
        {
            theConstraint = new DelayedConstraint(new EqualConstraint(true), 500);
            expectedDescription = "True after 500 milliseconds delay";
            stringRepresentation = "<after 500 <equal True>>";

            boolValue = false;
            list = new List<int>();
            statusString = null;
            //SetValueTrueAfterDelay(300);
        }

        static object[] SuccessData = new object[] { true };
        static object[] FailureData = new object[] {
            new TestCaseData( false, "False" ),
            new TestCaseData( 0, "0" ),
            new TestCaseData( null, "null" ) };

        static readonly ActualValueDelegate DelegateReturningValue;
        static readonly ActualValueDelegate DelegateReturningFalse;
        static readonly ActualValueDelegate DelegateReturningZero;

        static ActualValueDelegate<object>[] SuccessDelegates;
        static ActualValueDelegate<object>[] FailureDelegates;

        // Initialize static fields that are sensitive to order of initialization.
        // Most compilers would probably initialize these in lexical order but it
        // may not be guaranteed in all cases so we do it directly.
        static DelayedConstraintTests()
        {
            DelegateReturningValue = new ActualValueDelegate(MethodReturningValue);
            DelegateReturningFalse = new ActualValueDelegate(MethodReturningFalse);
            DelegateReturningZero = new ActualValueDelegate(MethodReturningZero);

            SuccessDelegates = new ActualValueDelegate<object>[] { DelegateReturningValue };
            FailureDelegates = new ActualValueDelegate<object>[] { DelegateReturningFalse, DelegateReturningZero };
        }

        [Test, TestCaseSource(nameof(SuccessDelegates))]
        public void SucceedsWithGoodDelegates(ActualValueDelegate<object> del)
        {
            SetValuesAfterDelay(DELAY);
            Assert.That(theConstraint.ApplyTo(del).IsSuccess);
        }

        [Test, TestCaseSource(nameof(FailureDelegates))]
        public void FailsWithBadDelegates(ActualValueDelegate<object> del)
        {
            Assert.IsFalse(theConstraint.ApplyTo(del).IsSuccess);
        }

        [Test]
        public void SimpleTest()
        {
            SetValuesAfterDelay(DELAY);
            Assert.That(DelegateReturningValue, new DelayedConstraint(new EqualConstraint(true), AFTER, POLLING));
        }

        [Test]
        public void SimpleTestUsingBoolean()
        {
            SetValuesAfterDelay(DELAY);
            Assert.That(() => boolValue, new DelayedConstraint(new EqualConstraint(true), AFTER, POLLING));
        }

        [Test]
        public void ThatOverload_ZeroDelayIsAllowed()
        {
            Assert.That(DelegateReturningZero, new DelayedConstraint(new EqualConstraint(0), 0));
        }

        [Test]
        public void ThatOverload_DoesNotAcceptNegativeDelayValues()
        {
            Assert.Throws<ArgumentException>(
                () => Assert.That(DelegateReturningZero, new DelayedConstraint(new EqualConstraint(0), -1)));
        }

        [Test]
        public void CanTestContentsOfList()
        {
            SetValuesAfterDelay(1);
            Assert.That(list, Has.Count.EqualTo(1).After(AFTER, POLLING));
        }

        [Test]
        public void CanTestContentsOfDelegateReturningList()
        {
            SetValuesAfterDelay(1);
            Assert.That(() => list, Has.Count.EqualTo(1).After(AFTER, POLLING));
        }

        [Test]
        public void CanTestInitiallyNullDelegate()
        {
            SetValuesAfterDelay(DELAY);
            Assert.That(() => statusString, Is.Not.Null.And.Length.GreaterThan(0).After(AFTER, POLLING));
        }

        [Test]
        public void ThatBlockingDelegateWhichSucceedsWithoutPolling_ReturnsAfterDelay()
        {
            var watch = new Stopwatch();
            watch.Start();

            Assert.That(() =>
            {
                Delay(DELAY);
                return true;
            }, Is.True.After(AFTER));

            watch.Stop();
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(MIN));
        }

        [Test]
        public void ThatBlockingDelegateWhichSucceedsWithPolling_ReturnsEarly()
        {
            var watch = new Stopwatch();
            watch.Start();

            Assert.That(() =>
            {
                Delay(DELAY);
                return true;
            }, Is.True.After(AFTER, POLLING));

            watch.Stop();
            Assert.That(watch.ElapsedMilliseconds, Is.InRange(DELAY, AFTER));
        }

        [Test]
        public void ThatBlockingDelegateWhichFailsWithoutPolling_FailsAfterDelay()
        {
            var watch = new Stopwatch();
            watch.Start();

            Assert.Throws<AssertionException>(() => Assert.That(() =>
            {
                Delay(DELAY);
                return false;
            }, Is.True.After(AFTER)));

            watch.Stop();
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(MIN));
        }

        [Test]
        public void ThatBlockingDelegateWhichFailsWithPolling_FailsAfterDelay()
        {
            var watch = new Stopwatch();
            watch.Start();

            Assert.Throws<AssertionException>(() => Assert.That(() =>
            {
                Delay(DELAY);
                return false;
            }, Is.True.After(AFTER, POLLING)));

            watch.Stop();
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(MIN));
        }

        [Test]
        public void ThatBlockingDelegateWhichThrowsWithoutPolling_FailsAfterDelay()
        {
            var watch = new Stopwatch();
            watch.Start();

            Assert.Throws<AssertionException>(() => Assert.That(() =>
            {
                Delay(DELAY);
                throw new InvalidOperationException();
            }, Is.True.After(AFTER)));

            watch.Stop();
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(MIN));
        }

        [Test]
        public void ThatBlockingDelegateWhichThrowsWithPolling_FailsAfterDelay()
        {
            var watch = new Stopwatch();
            watch.Start();

            Assert.Throws<AssertionException>(() => Assert.That(() =>
            {
                Delay(DELAY);
                throw new InvalidOperationException();
            }, Is.True.After(AFTER, POLLING)));

            watch.Stop();
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(AFTER));
        }

        private static int setValuesDelay;

        private static void MethodReturningVoid() { }

        private static object MethodReturningValue() { return boolValue; }

        private static object MethodReturningFalse() { return false; }

        private static object MethodReturningZero() { return 0; }

        private static readonly AutoResetEvent waitEvent = new AutoResetEvent(false);

        private static void Delay(int delay)
        {
            waitEvent.WaitOne(delay);
        }

        private static void MethodSetsValues()
        {
            Delay(setValuesDelay);
            boolValue = true;
            list.Add(1);
            statusString = "Finished";
        }

        private void SetValuesAfterDelay(int delayInMilliSeconds)
        {
            setValuesDelay = delayInMilliSeconds;
            Thread thread = new Thread(MethodSetsValues);
            thread.Start();
        }
    }
}
