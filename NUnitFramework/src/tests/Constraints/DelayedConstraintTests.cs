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
using System.ComponentModel;
using System.Threading;
using ActualValueDelegate = NUnit.Framework.Constraints.ActualValueDelegate<object>;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class DelayedConstraintTests : ConstraintTestBase
    {
        private static bool value;

        [SetUp]
        public void SetUp()
        {
            theConstraint = new DelayedConstraint(new EqualConstraint(true), 500);
            expectedDescription = "True after 500 millisecond delay";
            stringRepresentation = "<after 500 <equal True>>";

            value = false;
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
            SetValueTrueAfterDelay(300);
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
            SetValueTrueAfterDelay(500);
            Assert.That(DelegateReturningValue, new DelayedConstraint(new EqualConstraint(true), 5000, 200));
        }

        [Test]
        public void SimpleTestUsingReference()
        {
            SetValueTrueAfterDelay(500);
            Assert.That(ref value, new DelayedConstraint(new EqualConstraint(true), 5000, 200));
        }

        [Test]
        public void ThatOverload_ZeroDelayIsAllowed()
        {
            Assert.That(DelegateReturningZero, new DelayedConstraint(new EqualConstraint(0), 0));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ThatOverload_DoesNotAcceptNegativeDelayValues()
        {
            Assert.That(DelegateReturningZero, new DelayedConstraint(new EqualConstraint(0), -1));
        }

        [Test]
        public void CanTestContentsOfList()
        {
            BackgroundWorker worker = new BackgroundWorker();
            List<int> list = new System.Collections.Generic.List<int>();
            worker.RunWorkerCompleted += delegate { list.Add(1); };
            worker.DoWork += delegate { Thread.Sleep(1); };
            worker.RunWorkerAsync();
            Assert.That(list, Has.Count.EqualTo(1).After(5000, 100));
        }

        [Test]
        public void CanTestContentsOfRefList()
        {
            BackgroundWorker worker = new BackgroundWorker();
            List<int> list = new List<int>();
            worker.RunWorkerCompleted += delegate { list.Add(1); };
            worker.DoWork += delegate { Thread.Sleep(1); };
            worker.RunWorkerAsync();
            Assert.That(ref list, Has.Count.EqualTo(1).After(5000, 100));
        }

        [Test]
        public void CanTestContentsOfDelegateReturningList()
        {
            var worker = new BackgroundWorker();
            var list = new List<int>();
            worker.RunWorkerCompleted += delegate { list.Add(1); };
            worker.DoWork += delegate { Thread.Sleep(1); };
            worker.RunWorkerAsync();
            Assert.That(() => list, Has.Count.EqualTo(1).After(5000, 100));
        }
		
		[Test]
		public void CanTestInitiallyNullReference()
		{
			string statusString = null; // object starts off as null
			
			BackgroundWorker worker = new BackgroundWorker();
			worker.RunWorkerCompleted += delegate { statusString = "finished"; /* object non-null after work */ };
			worker.DoWork += delegate { Thread.Sleep(TimeSpan.FromSeconds(1)); /* simulate work */ };
			worker.RunWorkerAsync();
			
			Assert.That(ref statusString, Has.Length.GreaterThan(0).After(3000, 100));
		}
		
		[Test]
		public void CanTestInitiallyNullDelegate()
		{
			string statusString = null; // object starts off as null
			
			BackgroundWorker worker = new BackgroundWorker();
			worker.RunWorkerCompleted += delegate { statusString = "finished"; /* object non-null after work */ };
			worker.DoWork += delegate { Thread.Sleep(TimeSpan.FromSeconds(1)); /* simulate work */ };
			worker.RunWorkerAsync();
			
			Assert.That(() => statusString, Has.Length.GreaterThan(0).After(3000, 100));
		}

        private static int setValueTrueDelay;

        private void SetValueTrueAfterDelay(int delay)
        {
            setValueTrueDelay = delay;
            Thread thread = new Thread(SetValueTrueDelegate);
            thread.Start();
        }

        private static void MethodReturningVoid() { }
        private static TestDelegate InvalidDelegate = new TestDelegate(MethodReturningVoid);

        private static object MethodReturningValue() { return value; }
        private static ActualValueDelegate DelegateReturningValue = new ActualValueDelegate(MethodReturningValue);

        private static object MethodReturningFalse() { return false; }
        private static ActualValueDelegate DelegateReturningFalse = new ActualValueDelegate(MethodReturningFalse);

        private static object MethodReturningZero() { return 0; }
        private static ActualValueDelegate DelegateReturningZero = new ActualValueDelegate(MethodReturningZero);

        private static void MethodSetsValueTrue()
        {
            Thread.Sleep(setValueTrueDelay);
            value = true;
        }
        private ThreadStart SetValueTrueDelegate = new ThreadStart(MethodSetsValueTrue);
    }
}