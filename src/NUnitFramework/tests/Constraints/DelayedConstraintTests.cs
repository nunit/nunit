// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
            ExpectedDescription = "After 500 milliseconds delay";
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
            new TestCaseData(false, "False"),
            new TestCaseData(0, "0"),
            new TestCaseData(null, "null")
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

        [Test, Platform(Exclude = "MACOSX", Reason = "Doesn't seem to work correctly with timing, something to ponder later")]
        public void ThatPollingChecksValueCorrectNumberOfTimes()
        {
            var list = new PretendList();

#pragma warning disable NUnit2044 // Non-delegate actual parameter
            Assert.That(() => Assert.That(list, Has.Count.EqualTo(1).After(1000, 100)),
                        Throws.InstanceOf<AssertionException>());
#pragma warning restore NUnit2044 // Non-delegate actual parameter
            Assert.That(list.PollCount, Is.GreaterThan(5).And.LessThanOrEqualTo(10 + 1));
        }

        private class PretendList
        {
            public int PollCount { get; private set; }

            public int Count
            {
                get
                {
                    PollCount++;
                    return 0;
                }
            }
        }

        [Test, Platform(Exclude = "MACOSX", Reason = "Doesn't seem to work correctly with timing, something to ponder later")]
        public void ThatPollingCallsDelegateCorrectNumberOfTimes()
        {
            int pollCount = 0;
            Assert.That(() => Assert.That(() => ++pollCount, Is.EqualTo(0).After(1000, 100)),
                        Throws.InstanceOf<AssertionException>());
            Assert.That(pollCount, Is.GreaterThan(5).And.LessThanOrEqualTo(10 + 1));
        }

        [Test, Platform(Exclude = "MACOSX", Reason = "Doesn't seem to work correctly with timing, something to ponder later")]
        public void ThatPollingCallsAsyncDelegateCorrectNumberOfTimes()
        {
            int pollCount = 0;
            Assert.That(() => Assert.ThatAsync(() => Task.FromResult(++pollCount), Is.EqualTo(0).After(1000, 100)),
                        Throws.InstanceOf<AssertionException>());
            Assert.That(pollCount, Is.GreaterThan(5).And.LessThanOrEqualTo(10 + 1));
        }

        [Test]
        public void AssertionExpectingAnExceptionWithRetrySucceeds()
        {
            int i = 0;
            void ThrowsAfterRetry()
            {
                if (i++ > 0)
                {
                    throw new InvalidOperationException("Always throws after first attempt.");
                }
            }

            Assert.That(ThrowsAfterRetry, Throws.InvalidOperationException.After(AFTER, POLLING));
        }

        [Test]
        public void AssertionExpectingNoExceptionWithRetrySucceeds()
        {
            int i = 0;
            void DoesNotThrowAfterRetry()
            {
                if (i++ < 3)
                {
                    throw new InvalidOperationException("Only throws before third attempt.");
                }
            }

            Assert.That(DoesNotThrowAfterRetry, Throws.Nothing.After(AFTER, POLLING));
        }

        [Test]
        public void AssertionForDelegateWhichThrowsExceptionUntilRetriedSucceeds()
        {
            int i = 0;
            string DoesNotThrowAfterRetry()
            {
                if (i++ < 3)
                {
                    throw new InvalidOperationException("Only throws before third attempt.");
                }

                return "Success!";
            }

            Assert.That(DoesNotThrowAfterRetry, Is.EqualTo("Success!").After(AFTER, POLLING));
        }

        [Test]
        public void PreservesOriginalResultAdditionalLines()
        {
            var exception = Assert.Throws<AssertionException>(
                () => Assert.That(() => new[] { 1, 2 }, Is.EquivalentTo(new[] { 2, 3 }).After(1)));

            var expectedMessage =
                "  After 1 millisecond delay" + Environment.NewLine +
                "  Expected: equivalent to < 2, 3 >" + Environment.NewLine +
                "  But was:  < 1, 2 >" + Environment.NewLine +
                "  Missing (1): < 3 >" + Environment.NewLine +
                "  Extra (1): < 1 >" + Environment.NewLine;

            Assert.That(exception.Message, Does.Contain(expectedMessage));
        }

        [Test]
        public async Task PreservesUsingPropertiesComparerResultWithPolling()
        {
            await Assert.ThatAsync(FailingDelayedAssertion, Throws.InstanceOf<AssertionException>()
                .With.Message.Contains("After 200 milliseconds delay")
                .With.Message.Contains("Expected: ExpectedType { My = 9.876m, Type = 0.432m, With = <{ Lots = True, Of = 23456, Properties = 1 }> }")
                .With.Message.Contains("But was:  ExpectedType { My = 3.548m, Type = 0.123m, With = <{ Lots = False, Of = 13456, Properties = -1 }> }")
                .With.Message.Contains("at property ExpectedType.My:")
                .With.Message.Contains("Expected: 9.876m")
                .With.Message.Contains("But was:  3.548m"));

            static async Task FailingDelayedAssertion()
            {
                await Assert.ThatAsync(GetExpectedResult, Is.EqualTo(new ExpectedType()
                {
                    My = 9.876m,
                    Type = 0.432m,
                    With = new
                    {
                        Lots = true,
                        Of = "23456",
                        Properties = 1,
                    }
                }).UsingPropertiesComparer().After(200, 100));
            }

            static async Task<ExpectedType> GetExpectedResult()
            {
                await Task.Yield();
                return new ExpectedType()
                {
                    My = 3.548m,
                    Type = 0.123m,
                    With = new
                    {
                        Lots = false,
                        Of = "13456",
                        Properties = -1,
                    }
                };
            }
        }

        private sealed class ExpectedType
        {
            public decimal My { get; set; }
            public decimal Type { get; set; }
            public object? With { get; set; }
        }

        private static int _setValuesDelay;

        private static object MethodReturningValue()
        {
            return _boolValue;
        }

        private static object MethodReturningFalse()
        {
            return false;
        }

        private static object MethodReturningZero()
        {
            return 0;
        }

        private static readonly AutoResetEvent WaitEvent = new AutoResetEvent(false);

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            WaitEvent.Dispose();
        }

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
