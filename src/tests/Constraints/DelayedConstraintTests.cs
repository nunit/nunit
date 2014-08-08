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
using System.Collections.Generic;
using System.Threading;
using ActualValueDelegate = NUnit.Framework.Constraints.ActualValueDelegate<object>;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class DelayedConstraintTests : ConstraintTestBase
    {
        private const int SLEEP = 100;
        private const int AFTER = 300;
        private const int POLLING = 50;
        private const int MIN = (int)(AFTER * 0.9);
        private const int MAX = AFTER * 2;

        private static bool boolValue;
        private static List<int> list;
        private static string statusString;

        [SetUp]
        public void SetUp()
        {
            theConstraint = new DelayedConstraint(new EqualConstraint(true), 500);
            expectedDescription = "True after 500 millisecond delay";
            stringRepresentation = "<after 500 <equal True>>";

            boolValue = false;
            list = new List<int>();
            statusString = null;
            //SetValueTrueAfterDelay(300);
        }

        object[] SuccessData = new object[] { true };
        object[] FailureData = new object[] { 
            new TestCaseData( false, "False" ),
            new TestCaseData( 0, "0" ),
            new TestCaseData( null, "null" ) };

        object[] InvalidData = new object[] { InvalidDelegate };

        ActualValueDelegate<object>[] SuccessDelegates = new ActualValueDelegate<object>[] { DelegateReturningValue };
        ActualValueDelegate<object>[] FailureDelegates = new ActualValueDelegate<object>[] { DelegateReturningFalse, DelegateReturningZero };

        [Test, TestCaseSource("SuccessDelegates")]
        public void SucceedsWithGoodDelegates(ActualValueDelegate<object> del)
        {
            SetValuesAfterDelay(SLEEP);
            Assert.That(theConstraint.ApplyTo(del).IsSuccess);
        }

        [Test, TestCaseSource("FailureDelegates")]
        public void FailsWithBadDelegates(ActualValueDelegate<object> del)
        {
            Assert.IsFalse(theConstraint.ApplyTo(del).IsSuccess);
        }

        [Test]
        public void SimpleTest()
        {
            SetValuesAfterDelay(SLEEP);
            Assert.That(DelegateReturningValue, new DelayedConstraint(new EqualConstraint(true), AFTER, POLLING));
        }

        [Test]
        public void SimpleTestUsingReference()
        {
            SetValuesAfterDelay(SLEEP);
            Assert.That(ref boolValue, new DelayedConstraint(new EqualConstraint(true), AFTER, POLLING));
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
        public void CanTestContentsOfRefList()
        {
            SetValuesAfterDelay(1);
            Assert.That(ref list, Has.Count.EqualTo(1).After(AFTER, POLLING));
        }

        [Test]
        public void CanTestContentsOfDelegateReturningList()
        {
            SetValuesAfterDelay(1);
            Assert.That(() => list, Has.Count.EqualTo(1).After(AFTER, POLLING));
        }
        
        [Test]
        public void CanTestInitiallyNullReference()
        {
            SetValuesAfterDelay(SLEEP);
            Assert.That(ref statusString, Has.Length.GreaterThan(0).After(AFTER, POLLING));
        }
        
        [Test]
        public void CanTestInitiallyNullDelegate()
        {
            SetValuesAfterDelay(SLEEP);
            Assert.That(() => statusString, Has.Length.GreaterThan(0).After(AFTER, POLLING));
        }

        [Test]
        public void ThatBlockingDelegateWhichSucceedsWithoutPolling_ReturnsAfterDelay()
        {
            var start = DateTime.UtcNow;
            Assert.That(() =>
            {
                Thread.Sleep(SLEEP);
                return true;
            }, Is.True.After(AFTER));

            var elapsed = DateTime.UtcNow - start;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo(MIN)); // wiggle room due to timer resolution
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(MAX));
        }

        [Test]
        public void ThatBlockingDelegateWhichSucceedsWithPolling_ReturnsEarly()
        {
            var start = DateTime.UtcNow;
            Assert.That(() =>
            {
                Thread.Sleep(SLEEP);
                return true;
            }, Is.True.After(AFTER, POLLING));

            var elapsed = DateTime.UtcNow - start;
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(SLEEP + POLLING * 2));
        }

        [Test]
        public void ThatBlockingDelegateWhichFailsWithoutPolling_FailsAfterDelay()
        {
            var start = DateTime.UtcNow;
            Assert.Throws<AssertionException>(() => Assert.That(() =>
            {
                Thread.Sleep(SLEEP);
                return false;
            }, Is.True.After(AFTER)));

            var elapsed = DateTime.UtcNow - start;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo(MIN));
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(MAX));
        }

        [Test]
        public void ThatBlockingDelegateWhichFailsWithPolling_FailsAfterDelay()
        {
            var start = DateTime.UtcNow;
            Assert.Throws<AssertionException>(() => Assert.That(() =>
            {
                Thread.Sleep(SLEEP);
                return false;
            }, Is.True.After(AFTER, POLLING)));

            var elapsed = DateTime.UtcNow - start;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo(MIN));
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(MAX));
        }

        [Test]
        public void ThatBlockingDelegateWhichThrowsWithoutPolling_FailsAfterDelay()
        {
            var start = DateTime.UtcNow;
            Assert.Throws<AssertionException>(() => Assert.That(() =>
            {
                Thread.Sleep(SLEEP);
                throw new InvalidOperationException();
            }, Is.True.After(AFTER)));

            var elapsed = DateTime.UtcNow - start;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo(MIN));
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(MAX));
        }

        [Test]
        public void ThatBlockingDelegateWhichThrowsWithPolling_FailsAfterDelay()
        {
            var start = DateTime.UtcNow;
            Assert.Throws<AssertionException>(() => Assert.That(() =>
            {
                Thread.Sleep(SLEEP);
                throw new InvalidOperationException();
            }, Is.True.After(AFTER, POLLING)));

            var elapsed = DateTime.UtcNow - start;
            Assert.That(elapsed.TotalMilliseconds, Is.GreaterThanOrEqualTo(MIN));
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(MAX));
        }

        private static int setValuesDelay;

        private static void MethodReturningVoid() { }
        private static TestDelegate InvalidDelegate = new TestDelegate(MethodReturningVoid);

        private static object MethodReturningValue() { return boolValue; }
        private static ActualValueDelegate DelegateReturningValue = new ActualValueDelegate(MethodReturningValue);

        private static object MethodReturningFalse() { return false; }
        private static ActualValueDelegate DelegateReturningFalse = new ActualValueDelegate(MethodReturningFalse);

        private static object MethodReturningZero() { return 0; }
        private static ActualValueDelegate DelegateReturningZero = new ActualValueDelegate(MethodReturningZero);

        private static void MethodSetsValues()
        {
            Thread.Sleep(setValuesDelay);
            boolValue = true;
            list.Add(1);
            statusString = "Finished";
        }
        private ThreadStart SetValuesDelegate = new ThreadStart(MethodSetsValues);

        private void SetValuesAfterDelay(int delay)
        {
            setValuesDelay = delay;
            Thread thread = new Thread(SetValuesDelegate);
            thread.Start();
        }
    }
}