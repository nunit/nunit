// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Globalization;
using System.IO;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Execution;
using NUnit.Framework.Interfaces;
using System.Security.Principal;

#if ASYNC
using System.Threading.Tasks;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Summary description for TestExecutionContextTests.
    /// </summary>
    [TestFixture][Property("Question", "Why?")]
    public class TestExecutionContextTests
    {
        private TestExecutionContext _fixtureContext;
        private TestExecutionContext _setupContext;
        private ResultState _fixtureResult;

        string originalDirectory;

#if !NETCOREAPP1_1
        CultureInfo originalCulture;
        CultureInfo originalUICulture;
        IPrincipal originalPrincipal;
#endif

        readonly DateTime _fixtureCreateTime = DateTime.UtcNow;
        readonly long _fixtureCreateTicks = Stopwatch.GetTimestamp();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _fixtureContext = TestExecutionContext.CurrentContext;
            _fixtureResult = _fixtureContext.CurrentResult.ResultState;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // TODO: We put some tests in one time teardown to verify that
            // the context is still valid. It would be better if these tests
            // were placed in a second-level test, invoked from this test class.
            TestExecutionContext ec = TestExecutionContext.CurrentContext;
            Assert.That(ec.CurrentTest.Name, Is.EqualTo("TestExecutionContextTests"));
            Assert.That(ec.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests"));
            Assert.That(_fixtureContext.CurrentTest.Id, Is.Not.Null.And.Not.Empty);
            Assert.That(_fixtureContext.CurrentTest.Properties.Get("Question"), Is.EqualTo("Why?"));
        }

        [SetUp]
        public void Initialize()
        {
            _setupContext = TestExecutionContext.CurrentContext;

            originalDirectory = Directory.GetCurrentDirectory();

#if !NETCOREAPP1_1
            originalCulture = CultureInfo.CurrentCulture;
            originalUICulture = CultureInfo.CurrentUICulture;
            originalPrincipal = Thread.CurrentPrincipal;
#endif
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.SetCurrentDirectory(originalDirectory);

#if !NETCOREAPP1_1
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
            Thread.CurrentPrincipal = originalPrincipal;
#endif

            Assert.That(
                TestExecutionContext.CurrentContext.CurrentTest.FullName,
                Is.EqualTo(_setupContext.CurrentTest.FullName),
                "Context at TearDown failed to match that saved from SetUp");

            Assert.That(
                TestExecutionContext.CurrentContext.CurrentResult.Name,
                Is.EqualTo(_setupContext.CurrentResult.Name),
                "Cannot access CurrentResult in TearDown");
        }

#region CurrentContext

#if ASYNC
        [Test]
        public async Task CurrentContextFlowsWithAsyncExecution()
        {
            var context = TestExecutionContext.CurrentContext;
            await YieldAsync();
            Assert.AreSame(context, TestExecutionContext.CurrentContext);
        }

        [Test]
        public async Task CurrentContextFlowsWithParallelAsyncExecution()
        {
            var expected = TestExecutionContext.CurrentContext;
            var parallelResult = await WhenAllAsync(YieldAndReturnContext(), YieldAndReturnContext());

            Assert.AreSame(expected, TestExecutionContext.CurrentContext);
            Assert.AreSame(expected, parallelResult[0]);
            Assert.AreSame(expected, parallelResult[1]);
        }
#endif

        [Test]
        public void CurrentContextFlowsToUserCreatedThread()
        {
            TestExecutionContext threadContext = null;

            Thread thread = new Thread(() =>
            {
                threadContext = TestExecutionContext.CurrentContext;
            });

            thread.Start();
            thread.Join();

            Assert.That(threadContext, Is.Not.Null.And.SameAs(TestExecutionContext.CurrentContext));
        }

#endregion

#region CurrentTest

        [Test]
        public void FixtureSetUpCanAccessFixtureName()
        {
            Assert.That(_fixtureContext.CurrentTest.Name, Is.EqualTo("TestExecutionContextTests"));
        }

        [Test]
        public void FixtureSetUpCanAccessFixtureFullName()
        {
            Assert.That(_fixtureContext.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests"));
        }

        [Test]
        public void FixtureSetUpHasNullMethodName()
        {
            Assert.That(_fixtureContext.CurrentTest.MethodName, Is.Null);
        }

        [Test]
        public void FixtureSetUpCanAccessFixtureId()
        {
            Assert.That(_fixtureContext.CurrentTest.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void FixtureSetUpCanAccessFixtureProperties()
        {
            Assert.That(_fixtureContext.CurrentTest.Properties.Get("Question"), Is.EqualTo("Why?"));
        }

        [Test]
        public void SetUpCanAccessTestName()
        {
            Assert.That(_setupContext.CurrentTest.Name, Is.EqualTo("SetUpCanAccessTestName"));
        }

        [Test]
        public void SetUpCanAccessTestFullName()
        {
            Assert.That(_setupContext.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.SetUpCanAccessTestFullName"));
        }

        [Test]
        public void SetUpCanAccessTestMethodName()
        {
            Assert.That(_setupContext.CurrentTest.MethodName,
                Is.EqualTo("SetUpCanAccessTestMethodName"));
        }

        [Test]
        public void SetUpCanAccessTestId()
        {
            Assert.That(_setupContext.CurrentTest.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        [Property("Answer", 42)]
        public void SetUpCanAccessTestProperties()
        {
            Assert.That(_setupContext.CurrentTest.Properties.Get("Answer"), Is.EqualTo(42));
        }

        [Test]
        public void TestCanAccessItsOwnName()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Name, Is.EqualTo("TestCanAccessItsOwnName"));
        }

        [Test]
        public void TestCanAccessItsOwnFullName()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.TestCanAccessItsOwnFullName"));
        }

        [Test]
        public void TestCanAccessItsOwnMethodName()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.MethodName,
                Is.EqualTo("TestCanAccessItsOwnMethodName"));
        }

        [Test]
        public void TestCanAccessItsOwnId()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        [Property("Answer", 42)]
        public void TestCanAccessItsOwnProperties()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Properties.Get("Answer"), Is.EqualTo(42));
        }

        [TestCase(123, "abc")]
        public void TestCanAccessItsOwnArguments(int i, string s)
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Arguments, Is.EqualTo(new object[] {123, "abc"}));
        }

#if ASYNC
        [Test]
        public async Task AsyncTestCanAccessItsOwnName()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Name, Is.EqualTo("AsyncTestCanAccessItsOwnName"));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Name, Is.EqualTo("AsyncTestCanAccessItsOwnName"));
        }

        [Test]
        public async Task AsyncTestCanAccessItsOwnFullName()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.AsyncTestCanAccessItsOwnFullName"));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.AsyncTestCanAccessItsOwnFullName"));
        }

        [Test]
        public async Task AsyncTestCanAccessItsOwnMethodName()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.MethodName,
                Is.EqualTo("AsyncTestCanAccessItsOwnMethodName"));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.MethodName,
                Is.EqualTo("AsyncTestCanAccessItsOwnMethodName"));
        }

        [Test]
        public async Task AsyncTestCanAccessItsOwnId()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Id, Is.Not.Null.And.Not.Empty);
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Id, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        [Property("Answer", 42)]
        public async Task AsyncTestCanAccessItsOwnProperties()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Properties.Get("Answer"), Is.EqualTo(42));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Properties.Get("Answer"), Is.EqualTo(42));
        }

        [TestCase(123, "abc")]
        public async Task AsyncTestCanAccessItsOwnArguments(int i, string s)
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Arguments, Is.EqualTo(new object[] {123, "abc"}));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentTest.Arguments, Is.EqualTo(new object[] {123, "abc"}));
        }
#endif

#if PARALLEL
        [Test]
        public void TestHasWorkerWhenParallel()
        {
            var worker = TestExecutionContext.CurrentContext.TestWorker;
            var isRunningUnderTestWorker = TestExecutionContext.CurrentContext.Dispatcher is ParallelWorkItemDispatcher;
            Assert.That(worker != null || !isRunningUnderTestWorker);
        }
#endif

#endregion

#region CurrentResult

        [Test]
        public void CanAccessResultName()
        {
            Assert.That(_fixtureContext.CurrentResult.Name, Is.EqualTo("TestExecutionContextTests"));
            Assert.That(_setupContext.CurrentResult.Name, Is.EqualTo("CanAccessResultName"));
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.Name, Is.EqualTo("CanAccessResultName"));
        }

        [Test]
        public void CanAccessResultFullName()
        {
            Assert.That(_fixtureContext.CurrentResult.FullName, Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests"));
            Assert.That(_setupContext.CurrentResult.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.CanAccessResultFullName"));
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.CanAccessResultFullName"));
        }

        [Test]
        public void CanAccessResultTest()
        {
            Assert.That(_fixtureContext.CurrentResult.Test, Is.SameAs(_fixtureContext.CurrentTest));
            Assert.That(_setupContext.CurrentResult.Test, Is.SameAs(_setupContext.CurrentTest));
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.Test, Is.SameAs(TestExecutionContext.CurrentContext.CurrentTest));
        }

        [Test]
        public void CanAccessResultState()
        {
            // This is copied in setup because it can change if any test fails
            Assert.That(_fixtureResult, Is.EqualTo(ResultState.Success));
            Assert.That(_setupContext.CurrentResult.ResultState, Is.EqualTo(ResultState.Inconclusive));
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.ResultState, Is.EqualTo(ResultState.Inconclusive));
        }

#if ASYNC
        [Test]
        public async Task CanAccessResultName_Async()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.Name, Is.EqualTo("CanAccessResultName_Async"));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.Name, Is.EqualTo("CanAccessResultName_Async"));
        }

        [Test]
        public async Task CanAccessResultFullName_Async()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.CanAccessResultFullName_Async"));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.FullName,
                Is.EqualTo("NUnit.Framework.Internal.TestExecutionContextTests.CanAccessResultFullName_Async"));
        }

        [Test]
        public async Task CanAccessResultTest_Async()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.Test,
                Is.SameAs(TestExecutionContext.CurrentContext.CurrentTest));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.Test,
                Is.SameAs(TestExecutionContext.CurrentContext.CurrentTest));
        }

        [Test]
        public async Task CanAccessResultState_Async()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.ResultState, Is.EqualTo(ResultState.Inconclusive));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentResult.ResultState, Is.EqualTo(ResultState.Inconclusive));
        }
#endif

#endregion

#region StartTime

        [Test]
        public void CanAccessStartTime()
        {
            Assert.That(_fixtureContext.StartTime, Is.GreaterThan(DateTime.MinValue).And.LessThanOrEqualTo(_fixtureCreateTime));
            Assert.That(_setupContext.StartTime, Is.GreaterThanOrEqualTo(_fixtureContext.StartTime));
            Assert.That(TestExecutionContext.CurrentContext.StartTime, Is.GreaterThanOrEqualTo(_setupContext.StartTime));
        }

#if ASYNC
        [Test]
        public async Task CanAccessStartTime_Async()
        {
            var startTime = TestExecutionContext.CurrentContext.StartTime;
            Assert.That(startTime, Is.GreaterThanOrEqualTo(_setupContext.StartTime));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.StartTime, Is.EqualTo(startTime));
        }
#endif

#endregion

#region StartTicks

        [Test]
        public void CanAccessStartTicks()
        {
            Assert.That(_fixtureContext.StartTicks, Is.LessThanOrEqualTo(_fixtureCreateTicks));
            Assert.That(_setupContext.StartTicks, Is.GreaterThanOrEqualTo(_fixtureContext.StartTicks));
            Assert.That(TestExecutionContext.CurrentContext.StartTicks, Is.GreaterThanOrEqualTo(_setupContext.StartTicks));
        }

#if ASYNC
        [Test]
        public async Task AsyncTestCanAccessStartTicks()
        {
            var startTicks = TestExecutionContext.CurrentContext.StartTicks;
            Assert.That(startTicks, Is.GreaterThanOrEqualTo(_setupContext.StartTicks));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.StartTicks, Is.EqualTo(startTicks));
        }
#endif

#endregion

#region OutWriter

        [Test]
        public void CanAccessOutWriter()
        {
            Assert.That(_fixtureContext.OutWriter, Is.Not.Null);
            Assert.That(_setupContext.OutWriter, Is.Not.Null);
            Assert.That(TestExecutionContext.CurrentContext.OutWriter, Is.SameAs(_setupContext.OutWriter));
        }

#if ASYNC
        [Test]
        public async Task AsyncTestCanAccessOutWriter()
        {
            var outWriter = TestExecutionContext.CurrentContext.OutWriter;
            Assert.That(outWriter, Is.SameAs(_setupContext.OutWriter));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.OutWriter, Is.SameAs(outWriter));
        }
#endif

#endregion

#region TestObject

        [Test]
        public void CanAccessTestObject()
        {
            Assert.That(_fixtureContext.TestObject, Is.Not.Null.And.TypeOf(GetType()));
            Assert.That(_setupContext.TestObject, Is.SameAs(_fixtureContext.TestObject));
            Assert.That(TestExecutionContext.CurrentContext.TestObject, Is.SameAs(_setupContext.TestObject));
        }

#if ASYNC
        [Test]
        public async Task CanAccessTestObject_Async()
        {
            var testObject = TestExecutionContext.CurrentContext.TestObject;
            Assert.That(testObject, Is.SameAs(_setupContext.TestObject));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.TestObject, Is.SameAs(testObject));
        }
#endif

#endregion

#region StopOnError

        [Test]
        public void CanAccessStopOnError()
        {
            Assert.That(_setupContext.StopOnError, Is.EqualTo(_fixtureContext.StopOnError));
            Assert.That(TestExecutionContext.CurrentContext.StopOnError, Is.EqualTo(_setupContext.StopOnError));
        }

#if ASYNC
        [Test]
        public async Task CanAccessStopOnError_Async()
        {
            var stop = TestExecutionContext.CurrentContext.StopOnError;
            Assert.That(stop, Is.EqualTo(_setupContext.StopOnError));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.StopOnError, Is.EqualTo(stop));
        }
#endif

#endregion

#region Listener

        [Test]
        public void CanAccessListener()
        {
            Assert.That(_fixtureContext.Listener, Is.Not.Null);
            Assert.That(_setupContext.Listener, Is.SameAs(_fixtureContext.Listener));
            Assert.That(TestExecutionContext.CurrentContext.Listener, Is.SameAs(_setupContext.Listener));
        }

#if ASYNC
        [Test]
        public async Task CanAccessListener_Async()
        {
            var listener = TestExecutionContext.CurrentContext.Listener;
            Assert.That(listener, Is.SameAs(_setupContext.Listener));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.Listener, Is.SameAs(listener));
        }
#endif

#endregion

#region Dispatcher

        [Test]
        public void CanAccessDispatcher()
        {
            Assert.That(_fixtureContext.RandomGenerator, Is.Not.Null);
            Assert.That(_setupContext.Dispatcher, Is.SameAs(_fixtureContext.Dispatcher));
            Assert.That(TestExecutionContext.CurrentContext.Dispatcher, Is.SameAs(_setupContext.Dispatcher));
        }

#if ASYNC
        [Test]
        public async Task CanAccessDispatcher_Async()
        {
            var dispatcher = TestExecutionContext.CurrentContext.Dispatcher;
            Assert.That(dispatcher, Is.SameAs(_setupContext.Dispatcher));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.Dispatcher, Is.SameAs(dispatcher));
        }
#endif

#endregion

#region ParallelScope

        [Test]
        public void CanAccessParallelScope()
        {
            var scope = _fixtureContext.ParallelScope;
            Assert.That(_setupContext.ParallelScope, Is.EqualTo(scope));
            Assert.That(TestExecutionContext.CurrentContext.ParallelScope, Is.EqualTo(scope));
        }

#if ASYNC
        [Test]
        public async Task CanAccessParallelScope_Async()
        {
            var scope = TestExecutionContext.CurrentContext.ParallelScope;
            Assert.That(scope, Is.EqualTo(_setupContext.ParallelScope));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.ParallelScope, Is.EqualTo(scope));
        }
#endif

#endregion

#region TestWorker

#if PARALLEL
        [Test]
        public void CanAccessTestWorker()
        {
            if (TestExecutionContext.CurrentContext.Dispatcher is ParallelWorkItemDispatcher)
            {
                Assert.That(_fixtureContext.TestWorker, Is.Not.Null);
                Assert.That(_setupContext.TestWorker, Is.SameAs(_fixtureContext.TestWorker));
                Assert.That(TestExecutionContext.CurrentContext.TestWorker, Is.SameAs(_setupContext.TestWorker));
            }
        }

#if ASYNC
        [Test]
        public async Task CanAccessTestWorker_Async()
        {
            var worker = TestExecutionContext.CurrentContext.TestWorker;
            Assert.That(worker, Is.SameAs(_setupContext.TestWorker));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.TestWorker, Is.SameAs(worker));
        }
#endif
#endif

#endregion

#region RandomGenerator

        [Test]
        public void CanAccessRandomGenerator()
        {
            Assert.That(_fixtureContext.RandomGenerator, Is.Not.Null);
            Assert.That(_setupContext.RandomGenerator, Is.Not.Null);
            Assert.That(TestExecutionContext.CurrentContext.RandomGenerator, Is.SameAs(_setupContext.RandomGenerator));
        }

#if ASYNC
        [Test]
        public async Task CanAccessRandomGenerator_Async()
        {
            var random = TestExecutionContext.CurrentContext.RandomGenerator;
            Assert.That(random, Is.SameAs(_setupContext.RandomGenerator));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.RandomGenerator, Is.SameAs(random));
        }
#endif

#endregion

#region AssertCount

        [Test]
        public void CanAccessAssertCount()
        {
            Assert.That(_fixtureContext.AssertCount, Is.EqualTo(0));
            Assert.That(_setupContext.AssertCount, Is.EqualTo(1));
            Assert.That(TestExecutionContext.CurrentContext.AssertCount, Is.EqualTo(2));
            Assert.That(2 + 2, Is.EqualTo(4));
            Assert.That(TestExecutionContext.CurrentContext.AssertCount, Is.EqualTo(4));
        }

#if ASYNC
        [Test]
        public async Task CanAccessAssertCount_Async()
        {
            Assert.That(2 + 2, Is.EqualTo(4));
            Assert.That(TestExecutionContext.CurrentContext.AssertCount, Is.EqualTo(1));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.AssertCount, Is.EqualTo(2));
            Assert.That(TestExecutionContext.CurrentContext.AssertCount, Is.EqualTo(3));
        }
#endif

#endregion

#region MultipleAssertLevel

        [Test]
        public void CanAccessMultipleAssertLevel()
        {
            Assert.That(_fixtureContext.MultipleAssertLevel, Is.EqualTo(0));
            Assert.That(_setupContext.MultipleAssertLevel, Is.EqualTo(0));
            Assert.That(TestExecutionContext.CurrentContext.MultipleAssertLevel, Is.EqualTo(0));
            Assert.Multiple(() =>
            {
                Assert.That(TestExecutionContext.CurrentContext.MultipleAssertLevel, Is.EqualTo(1));
            });
        }

#if ASYNC
        [Test]
        public async Task CanAccessMultipleAssertLevel_Async()
        {
            Assert.That(TestExecutionContext.CurrentContext.MultipleAssertLevel, Is.EqualTo(0));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.MultipleAssertLevel, Is.EqualTo(0));
            Assert.Multiple(() =>
            {
                Assert.That(TestExecutionContext.CurrentContext.MultipleAssertLevel, Is.EqualTo(1));
            });
        }
#endif

#endregion

#region TestCaseTimeout

        [Test]
        public void CanAccessTestCaseTimeout()
        {
            var timeout = _fixtureContext.TestCaseTimeout;
            Assert.That(_setupContext.TestCaseTimeout, Is.EqualTo(timeout));
            Assert.That(TestExecutionContext.CurrentContext.TestCaseTimeout, Is.EqualTo(timeout));
        }

#if ASYNC
        [Test]
        public async Task CanAccessTestCaseTimeout_Async()
        {
            var timeout = TestExecutionContext.CurrentContext.TestCaseTimeout;
            Assert.That(timeout, Is.EqualTo(_setupContext.TestCaseTimeout));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.TestCaseTimeout, Is.EqualTo(timeout));
        }
#endif

#endregion

#region UpstreamActions

        [Test]
        public void CanAccessUpstreamActions()
        {
            var actions = _fixtureContext.UpstreamActions;
            Assert.That(_setupContext.UpstreamActions, Is.EqualTo(actions));
            Assert.That(TestExecutionContext.CurrentContext.UpstreamActions, Is.EqualTo(actions));
        }

#if ASYNC
        [Test]
        public async Task CanAccessUpstreamAcxtions_Async()
        {
            var actions = TestExecutionContext.CurrentContext.UpstreamActions;
            Assert.That(actions, Is.SameAs(_setupContext.UpstreamActions));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.UpstreamActions, Is.SameAs(actions));
        }
#endif

#endregion

#region CurrentCulture and CurrentUICulture

#if !NETCOREAPP1_1
        [Test]
        public void CanAccessCurrentCulture()
        {
            Assert.That(_fixtureContext.CurrentCulture, Is.EqualTo(CultureInfo.CurrentCulture));
            Assert.That(_setupContext.CurrentCulture, Is.EqualTo(CultureInfo.CurrentCulture));
            Assert.That(TestExecutionContext.CurrentContext.CurrentCulture, Is.EqualTo(CultureInfo.CurrentCulture));
        }

        [Test]
        public void CanAccessCurrentUICulture()
        {
            Assert.That(_fixtureContext.CurrentUICulture, Is.EqualTo(CultureInfo.CurrentUICulture));
            Assert.That(_setupContext.CurrentUICulture, Is.EqualTo(CultureInfo.CurrentUICulture));
            Assert.That(TestExecutionContext.CurrentContext.CurrentUICulture, Is.EqualTo(CultureInfo.CurrentUICulture));
        }

#if ASYNC
        [Test]
        public async Task CanAccessCurrentCulture_Async()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentCulture, Is.EqualTo(CultureInfo.CurrentCulture));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentCulture, Is.EqualTo(CultureInfo.CurrentCulture));
        }

        [Test]
        public async Task CanAccessCurrentUICulture_Async()
        {
            Assert.That(TestExecutionContext.CurrentContext.CurrentUICulture, Is.EqualTo(CultureInfo.CurrentUICulture));
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentUICulture, Is.EqualTo(CultureInfo.CurrentUICulture));
        }
#endif

        [Test]
        public void SetAndRestoreCurrentCulture()
        {
            var context = new TestExecutionContext(_setupContext);

            try
            {
                CultureInfo otherCulture =
                    new CultureInfo(originalCulture.Name == "fr-FR" ? "en-GB" : "fr-FR");
                context.CurrentCulture = otherCulture;
                Assert.AreEqual(otherCulture, CultureInfo.CurrentCulture, "Culture was not set");
                Assert.AreEqual(otherCulture, context.CurrentCulture, "Culture not in new context");
                Assert.AreEqual(_setupContext.CurrentCulture, originalCulture, "Original context should not change");
            }
            finally
            {
                _setupContext.EstablishExecutionEnvironment();
            }

            Assert.AreEqual(CultureInfo.CurrentCulture, originalCulture, "Culture was not restored");
            Assert.AreEqual(_setupContext.CurrentCulture, originalCulture, "Culture not in final context");
        }

        [Test]
        public void SetAndRestoreCurrentUICulture()
        {
            var context = new TestExecutionContext(_setupContext);

            try
            {
                CultureInfo otherCulture =
                    new CultureInfo(originalUICulture.Name == "fr-FR" ? "en-GB" : "fr-FR");
                context.CurrentUICulture = otherCulture;
                Assert.AreEqual(otherCulture, CultureInfo.CurrentUICulture, "UICulture was not set");
                Assert.AreEqual(otherCulture, context.CurrentUICulture, "UICulture not in new context");
                Assert.AreEqual(_setupContext.CurrentUICulture, originalUICulture, "Original context should not change");
            }
            finally
            {
                _setupContext.EstablishExecutionEnvironment();
            }

            Assert.AreEqual(CultureInfo.CurrentUICulture, originalUICulture, "UICulture was not restored");
            Assert.AreEqual(_setupContext.CurrentUICulture, originalUICulture, "UICulture not in final context");
        }
#endif

#endregion

#region CurrentPrincipal

#if !NETCOREAPP1_1
        [Test]
        public void CanAccessCurrentPrincipal()
        {
            var expectedInstance = Thread.CurrentPrincipal;
            Assert.That(_fixtureContext.CurrentPrincipal, Is.SameAs(expectedInstance), "Fixture");
            Assert.That(_setupContext.CurrentPrincipal, Is.SameAs(expectedInstance), "SetUp");
            Assert.That(TestExecutionContext.CurrentContext.CurrentPrincipal, Is.SameAs(expectedInstance), "Test");
        }

#if ASYNC
        [Test]
        public async Task CanAccessCurrentPrincipal_Async()
        {
            var expectedInstance = Thread.CurrentPrincipal;
            Assert.That(TestExecutionContext.CurrentContext.CurrentPrincipal, Is.SameAs(expectedInstance), "Before yield");
            await YieldAsync();
            Assert.That(TestExecutionContext.CurrentContext.CurrentPrincipal, Is.SameAs(expectedInstance), "After yield");
        }
#endif

        [Test]
        public void SetAndRestoreCurrentPrincipal()
        {
            var context = new TestExecutionContext(_setupContext);

            try
            {
                GenericIdentity identity = new GenericIdentity("foo");
                context.CurrentPrincipal = new GenericPrincipal(identity, new string[0]);
                Assert.AreEqual("foo", Thread.CurrentPrincipal.Identity.Name, "Principal was not set");
                Assert.AreEqual("foo", context.CurrentPrincipal.Identity.Name, "Principal not in new context");
                Assert.AreEqual(_setupContext.CurrentPrincipal, originalPrincipal, "Original context should not change");
            }
            finally
            {
                _setupContext.EstablishExecutionEnvironment();
            }

            Assert.AreEqual(Thread.CurrentPrincipal, originalPrincipal, "Principal was not restored");
            Assert.AreEqual(_setupContext.CurrentPrincipal, originalPrincipal, "Principal not in final context");
        }
#endif

#endregion

#region ValueFormatter

        [Test]
        public void SetAndRestoreValueFormatter()
        {
            var context = new TestExecutionContext(_setupContext);
            var originalFormatter = context.CurrentValueFormatter;

            try
            {
                ValueFormatter f = val => "dummy";
                context.AddFormatter(next => f);
                Assert.That(context.CurrentValueFormatter, Is.EqualTo(f));

                context.EstablishExecutionEnvironment();
                Assert.That(MsgUtils.FormatValue(123), Is.EqualTo("dummy"));
            }
            finally
            {
                _setupContext.EstablishExecutionEnvironment();
            }

            Assert.That(TestExecutionContext.CurrentContext.CurrentValueFormatter, Is.EqualTo(originalFormatter));
            Assert.That(MsgUtils.FormatValue(123), Is.EqualTo("123"));
        }

#endregion

#region SingleThreaded

        [Test]
        public void SingleThreadedDefaultsToFalse()
        {
            Assert.False(new TestExecutionContext().IsSingleThreaded);
        }

        [Test]
        public void SingleThreadedIsInherited()
        {
            var parent = new TestExecutionContext();
            parent.IsSingleThreaded = true;
            Assert.True(new TestExecutionContext(parent).IsSingleThreaded);
        }

#endregion

#region ExecutionStatus

        [Test]
        public void ExecutionStatusIsPushedToHigherContext()
        {
            var topContext = new TestExecutionContext();
            var bottomContext = new TestExecutionContext(new TestExecutionContext(new TestExecutionContext(topContext)));

            bottomContext.ExecutionStatus = TestExecutionStatus.StopRequested;

            Assert.That(topContext.ExecutionStatus, Is.EqualTo(TestExecutionStatus.StopRequested));
        }

        [Test]
        public void ExecutionStatusIsPulledFromHigherContext()
        {
            var topContext = new TestExecutionContext();
            var bottomContext = new TestExecutionContext(new TestExecutionContext(new TestExecutionContext(topContext)));

            topContext.ExecutionStatus = TestExecutionStatus.AbortRequested;

            Assert.That(bottomContext.ExecutionStatus, Is.EqualTo(TestExecutionStatus.AbortRequested));
        }

        [Test]
        public void ExecutionStatusIsPromulgatedAcrossBranches()
        {
            var topContext = new TestExecutionContext();
            var leftContext = new TestExecutionContext(new TestExecutionContext(new TestExecutionContext(topContext)));
            var rightContext = new TestExecutionContext(new TestExecutionContext(new TestExecutionContext(topContext)));

            leftContext.ExecutionStatus = TestExecutionStatus.StopRequested;

            Assert.That(rightContext.ExecutionStatus, Is.EqualTo(TestExecutionStatus.StopRequested));
        }

#endregion

#region Cross-domain Tests

#if NET20 || NET35 || NET40 || NET45
        [Test, Platform(Exclude="Mono", Reason="Intermittent failures")]
        public void CanCreateObjectInAppDomain()
        {
            AppDomain domain = AppDomain.CreateDomain(
                "TestCanCreateAppDomain",
                AppDomain.CurrentDomain.Evidence,
                AssemblyHelper.GetDirectoryName(Assembly.GetExecutingAssembly()),
                null,
                false);

            var obj = domain.CreateInstanceAndUnwrap("nunit.framework.tests", "NUnit.Framework.Internal.TestExecutionContextTests+TestClass");

            Assert.NotNull(obj);
        }

        [Serializable]
        private class TestClass
        {
        }
#endif

        #endregion

#region CurrentRepeatCount Tests
        [Test]
        public void CanAccessCurrentRepeatCount()
        {
            Assert.That(_fixtureContext.CurrentRepeatCount, Is.EqualTo(0), "expected value to default to zero");
            _fixtureContext.CurrentRepeatCount++;
            Assert.That(_fixtureContext.CurrentRepeatCount, Is.EqualTo(1), "expected value to be able to be incremented from the TestExecutionContext");
        }
#endregion

#region Helper Methods

#if ASYNC
        private async Task YieldAsync()
        {
#if NET40
            await TaskEx.Yield();
#else
            await Task.Yield();
#endif
        }

        private Task<T[]> WhenAllAsync<T>(params Task<T>[] tasks)
        {
#if NET40
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        private async Task<TestExecutionContext> YieldAndReturnContext()
        {
            await YieldAsync();
            return TestExecutionContext.CurrentContext;
        }
#endif

#endregion
    }

#if NET20 || NET35 || NET40 || NET45
    [TestFixture, Platform(Exclude="Mono", Reason="Intermittent failures")]
    public class TextExecutionContextInAppDomain
    {
        private RunsInAppDomain _runsInAppDomain;

        [SetUp]
        public void SetUp()
        {
            var domain = AppDomain.CreateDomain("TestDomain", null, AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath, false);
            _runsInAppDomain = domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
                "NUnit.Framework.Internal.RunsInAppDomain") as RunsInAppDomain;
            Assert.That(_runsInAppDomain, Is.Not.Null);
        }

        [Test]
        [Description("Issue 71 - NUnit swallows console output from AppDomains created within tests")]
        public void CanWriteToConsoleInAppDomain()
        {
            _runsInAppDomain.WriteToConsole();
        }

        [Test]
        [Description("Issue 210 - TestContext.WriteLine in an AppDomain causes an error")]
        public void CanWriteToTestContextInAppDomain()
        {
            _runsInAppDomain.WriteToTestContext();
        }
    }

    internal class RunsInAppDomain : MarshalByRefObject
    {
        public void WriteToConsole()
        {
            Console.WriteLine("RunsInAppDomain.WriteToConsole");
        }

        public void WriteToTestContext()
        {
            TestContext.WriteLine("RunsInAppDomain.WriteToTestContext");
        }
    }
#endif
}
