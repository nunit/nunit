// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework.Constraints;
using ActualValueDelegate = NUnit.Framework.Constraints.ActualValueDelegate<object>;

namespace NUnit.Framework.Tests.Constraints
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
        private const int AFTER = 500;
        private const int POLLING = 50;
        private const int MIN = AFTER - 10;

        private static bool _boolValue;
        private static List<int> _list;
        private static string? _statusString;

        protected override Constraint TheConstraint { get; } = new DelayedConstraint(new EqualConstraint(true), 500);

        [SetUp]
        public void SetUp()
        {
            ExpectedDescription = "True after 500 milliseconds delay";
            StringRepresentation = "<after 500 <equal True>>";

            _boolValue = false;
            _list = new List<int>();
            _statusString = null;
            //SetValueTrueAfterDelay(300);
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = new object[] { true };
        private static readonly object[] FailureData = new object[]
        {
            new TestCaseData( false, "False" ),
            new TestCaseData( 0, "0" ),
            new TestCaseData( null, "null" )
        };
#pragma warning restore IDE0052 // Remove unread private members

        private static readonly ActualValueDelegate DelegateReturningValue;
        private static readonly ActualValueDelegate DelegateReturningFalse;
        private static readonly ActualValueDelegate DelegateReturningZero;
        private static readonly ActualValueDelegate<object>[] SuccessDelegates;
        private static readonly ActualValueDelegate<object>[] FailureDelegates;

        // Initialize static fields that are sensitive to order of initialization.
        // Most compilers would probably initialize these in lexical order but it
        // may not be guaranteed in all cases so we do it directly.
        static DelayedConstraintTests()
        {
            DelegateReturningValue = new ActualValueDelegate(MethodReturningValue);
            DelegateReturningFalse = new ActualValueDelegate(MethodReturningFalse);
            DelegateReturningZero = new ActualValueDelegate(MethodReturningZero);

            SuccessDelegates = new[] { DelegateReturningValue };
            FailureDelegates = new[] { DelegateReturningFalse, DelegateReturningZero };
        }

        [Test, TestCaseSource(nameof(SuccessDelegates))]
        public void SucceedsWithGoodDelegates(ActualValueDelegate<object> del)
        {
            SetValuesAfterDelay(DELAY);
            Assert.That(TheConstraint.ApplyTo(del).IsSuccess);
        }

        [Test, TestCaseSource(nameof(FailureDelegates))]
        public void FailsWithBadDelegates(ActualValueDelegate<object> del)
        {
            Assert.That(TheConstraint.ApplyTo(del).IsSuccess, Is.False);
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
            Assert.That(() => _boolValue, new DelayedConstraint(new EqualConstraint(true), AFTER, POLLING));
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

            // https://github.com/nunit/nunit.analyzers/issues/431
            // It was decided to keep to analyzer warning
#pragma warning disable NUnit2044 // Non-delegate actual parameter
            Assert.That(_list, Has.Count.EqualTo(1).After(AFTER, POLLING));
#pragma warning restore NUnit2044 // Non-delegate actual parameter
        }

        [Test]
        public void CanTestContentsOfDelegateReturningList()
        {
            SetValuesAfterDelay(1);
            Assert.That(() => _list, Has.Count.EqualTo(1).After(AFTER, POLLING));
        }

        [Test]
        public void CanTestInitiallyNullDelegate()
        {
            SetValuesAfterDelay(DELAY);
            Assert.That(() => _statusString, Is.Not.Null.And.Length.GreaterThan(0).After(AFTER, POLLING));
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

        [Test]
        public void PreservesOriginalResultAdditionalLines()
        {
            var exception = Assert.Throws<AssertionException>(
                () => Assert.That(() => new[] { 1, 2 }, Is.EquivalentTo(new[] { 2, 3 }).After(1)));

            var expectedMessage =
                "  Expected: equivalent to < 2, 3 > after 1 millisecond delay" + Environment.NewLine +
                "  But was:  < 1, 2 >" + Environment.NewLine +
                "  Missing (1): < 3 >" + Environment.NewLine +
                "  Extra (1): < 1 >" + Environment.NewLine;

            Assert.That(exception.Message, Does.Contain(expectedMessage));
        }

        private static int _setValuesDelay;

        private static object MethodReturningValue() { return _boolValue; }

        private static object MethodReturningFalse() { return false; }

        private static object MethodReturningZero() { return 0; }

        private static readonly AutoResetEvent WaitEvent = new AutoResetEvent(false);

        private static void Delay(int delay)
        {
            WaitEvent.WaitOne(delay);
        }

        private static void MethodSetsValues()
        {
            Delay(_setValuesDelay);
            _boolValue = true;
            _list.Add(1);
            _statusString = "Finished";
        }

        private static void SetValuesAfterDelay(int delayInMilliSeconds)
        {
            _setValuesDelay = delayInMilliSeconds;
            Thread thread = new Thread(MethodSetsValues);
            thread.Start();
        }
    }
}
