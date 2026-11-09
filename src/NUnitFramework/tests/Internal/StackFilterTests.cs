// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Linq;
using System.Text;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class StackFilterTests : DefaultStackFilterFixture
    {
        #region Test Data

        private static readonly string ShortTrace_Assert = """
           bei NUnit.Framework.Assert.Fail(String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:linie 56
           bei NUnit.Framework.Assert.That(String label, Object actual, Matcher expectation, String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:linie 50
           bei NUnit.Framework.Assert.That(Object actual, Matcher expectation) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:linie 19
           bei NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:linie 12
        """;

        private static readonly string ShortTrace_Assume = """
           bei NUnit.Framework.Assert.Fail(String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:linie 56
           bei NUnit.Framework.Assume.That(String label, Object actual, Matcher expectation, String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:linie 50
           bei NUnit.Framework.Assume.That(Object actual, Matcher expectation) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:linie 19
           bei NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:linie 12
        """;

        private static readonly string ShortTrace_Result = """
           bei NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:linie 12

        """;

        // NOTE: In most cases, NUnit does not have to deal with traces
        // like this because the InnerException of a TargetInvocationException
        // only includes the methods called from the point of invocation.
        // However, in the compact framework, such long traces may arise.
        private static readonly string LongTrace = """
           at NUnit.Framework.Assert.Fail(String message, Object[] args)
           at MyNamespace.MyAppsTests.AssertFailTest()
           at System.Reflection.RuntimeMethodInfo.InternalInvoke(RuntimeMethodInfo rtmi, Object obj, BindingFlags invokeAttr, Binder binder, Object parameters, CultureInfo culture, Boolean isBinderDefault, Assembly caller, Boolean verifyAccess, StackCrawlMark& stackMark)
           at System.Reflection.RuntimeMethodInfo.InternalInvoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture, Boolean verifyAccess, StackCrawlMark& stackMark)
           at System.Reflection.RuntimeMethodInfo.Invoke(Object obj, BindingFlags invokeAttr, Binder binder, Object[] parameters, CultureInfo culture)
           at System.Reflection.MethodBase.Invoke(Object obj, Object[] parameters)
           at NUnitLite.ProxyTestCase.InvokeMethod(MethodInfo method, Object[] args)
           at NUnit.Framework.TestCase.RunTest()
           at NUnit.Framework.TestCase.RunBare()
           at NUnit.Framework.TestCase.Run(TestResult result, TestListener listener)
           at NUnit.Framework.TestCase.Run(TestListener listener)
           at NUnit.Framework.TestSuite.Run(TestListener listener)
           at NUnit.Framework.TestSuite.Run(TestListener listener)
           at NUnitLite.Runner.TestRunner.Run(ITest test)
           at NUnitLite.Runner.ConsoleUI.Run(ITest test)
           at NUnitLite.Runner.TestRunner.Run(Assembly assembly)
           at NUnitLite.Runner.ConsoleUI.Run()
           at NUnitLite.Runner.ConsoleUI.Main(String[] args)
           at OpenNETCF.Linq.Demo.Program.Main(String[] args)
        """;

        private static readonly string LongTrace_Result = """
           at MyNamespace.MyAppsTests.AssertFailTest()
           at OpenNETCF.Linq.Demo.Program.Main(String[] args)

        """;

        private static readonly string QueueUserItemTrace = """
           at NUnit.Framework.Assert.GetCallerStackTraceWithoutThrowing() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Assert.cs:line 383
           at NUnit.Framework.Assert.GetStackTrace() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Assert.cs:line 374
           at NUnit.Framework.Assert.IssueWarning(String message) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Assert.cs:line 367
           at NUnit.Framework.Assert.Warn(String message) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Assert.cs:line 132
           at NUnit.TestData.WarningFixture.<>c__DisplayClass74_0.<WarningInThreadPoolQueueUserWorkItem>b__0(Object _) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\testdata\WarningFixture.cs:line 536
           at System.Threading.QueueUserWorkItemCallback.Execute()
           at System.Threading.ThreadPoolWorkQueue.Dispatch()
           at System.Threading.PortableThreadPool.WorkerThread.WorkerDoWork(PortableThreadPool threadPoolInstance)
           at System.Threading.PortableThreadPool.WorkerThread.WorkerThreadStart()
           at System.Threading.Thread.StartCallback(Thread* pThread)
        """;

        private static readonly string QueueUserItemTrace_Result = """
           at NUnit.TestData.WarningFixture.<>c__DisplayClass74_0.<WarningInThreadPoolQueueUserWorkItem>b__0(Object _) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\testdata\WarningFixture.cs:line 536

        """;

        // The stack trace for an exception thrown in an async method is different between .NET Framework and .NET Core.
        // The .NET Framework constains evidence of the async state machine, while .NET Core does not.
#if NETFRAMEWORK
        private static readonly string ThrownExceptionInAsyncMethodTrace = """
           at NUnit.TestData.StackFilterFixture.<ThrowExceptionAsync>d__8`1.MoveNext() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\testdata\StackFilterFixture.cs:line 55
        --- End of stack trace from previous location where exception was thrown ---
           at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
           at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
           at System.Runtime.CompilerServices.TaskAwaiter.GetResult()
           at NUnit.TestData.StackFilterFixture.<ThrowExceptionInAsyncUserMethod>d__7.MoveNext() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\testdata\StackFilterFixture.cs:line 62
        --- End of stack trace from previous location where exception was thrown ---
           at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
           at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
           at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
           at NUnit.Framework.Internal.TaskAwaitAdapter.GenericAdapter`1.BlockUntilCompleted() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\TaskAwaitAdapter.cs:line 68
           at NUnit.Framework.Internal.MessagePumpStrategy.NoMessagePumpStrategy.WaitForCompletion(AwaitAdapter awaiter) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\MessagePumpStrategy.cs:line 35
           at NUnit.Framework.Internal.AsyncToSyncAdapter.Await[TResult](TestExecutionContext context, Func`1 invoke) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\AsyncToSyncAdapter.cs:line 38
           at NUnit.Framework.Internal.AsyncToSyncAdapter.Await(TestExecutionContext context, Func`1 invoke) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\AsyncToSyncAdapter.cs:line 25
           at NUnit.Framework.Internal.Commands.TestMethodCommand.RunTestMethod(TestExecutionContext context) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\Commands\TestMethodCommand.cs:line 74
           at NUnit.Framework.Internal.Commands.TestMethodCommand.Execute(TestExecutionContext context) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\Commands\TestMethodCommand.cs:line 50
           at NUnit.Framework.Internal.Execution.SimpleWorkItem.<>c__DisplayClass3_0.<PerformWork>b__0() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\Execution\SimpleWorkItem.cs:line 50
           at NUnit.Framework.Internal.ContextUtils.<>c__DisplayClass1_0`1.<DoIsolated>b__0(Object _) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\ContextUtils.cs:line 18
           at NUnit.Framework.Internal.ContextUtils.<>c__DisplayClass2_1.<DoIsolated>b__0(Object s) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\ContextUtils.cs:line 43
           at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
           at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
           at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
           at NUnit.Framework.Internal.ContextUtils.DoIsolated(ContextCallback callback, Object state) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\ContextUtils.cs:line 34
           at NUnit.Framework.Internal.ContextUtils.DoIsolated[T](Func`1 func) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\ContextUtils.cs:line 18
           at NUnit.Framework.Internal.Execution.SimpleWorkItem.PerformWork() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\Execution\SimpleWorkItem.cs:line 50
        """;
#else
        private static readonly string ThrownExceptionInAsyncMethodTrace = """
           at NUnit.TestData.StackFilterFixture.ThrowExceptionAsync() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\testdata\StackFilterFixture.cs:line 55
           at NUnit.TestData.StackFilterFixture.ThrowExceptionInAsyncUserMethod() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\testdata\StackFilterFixture.cs:line 62
           at NUnit.Framework.Internal.TaskAwaitAdapter.GenericAdapter`1.BlockUntilCompleted() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\TaskAwaitAdapter.cs:line 68
           at NUnit.Framework.Internal.MessagePumpStrategy.NoMessagePumpStrategy.WaitForCompletion(AwaitAdapter awaiter) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\MessagePumpStrategy.cs:line 35
           at NUnit.Framework.Internal.AsyncToSyncAdapter.Await[TResult](TestExecutionContext context, Func`1 invoke) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\AsyncToSyncAdapter.cs:line 38
           at NUnit.Framework.Internal.AsyncToSyncAdapter.Await(TestExecutionContext context, Func`1 invoke) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\AsyncToSyncAdapter.cs:line 25
           at NUnit.Framework.Internal.Commands.TestMethodCommand.RunTestMethod(TestExecutionContext context) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\Commands\TestMethodCommand.cs:line 74
           at NUnit.Framework.Internal.Commands.TestMethodCommand.Execute(TestExecutionContext context) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\Commands\TestMethodCommand.cs:line 50
           at NUnit.Framework.Internal.Execution.SimpleWorkItem.<>c__DisplayClass3_0.<PerformWork>b__0() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\Execution\SimpleWorkItem.cs:line 50
           at NUnit.Framework.Internal.ContextUtils.<>c__DisplayClass1_0`1.<DoIsolated>b__0(Object _) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\ContextUtils.cs:line 18
           at NUnit.Framework.Internal.ContextUtils.<>c__DisplayClass2_1.<DoIsolated>b__0(Object s) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\ContextUtils.cs:line 43
           at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
        --- End of stack trace from previous location ---
           at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
           at NUnit.Framework.Internal.ContextUtils.DoIsolated(ContextCallback callback, Object state) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\ContextUtils.cs:line 34
           at NUnit.Framework.Internal.ContextUtils.DoIsolated[T](Func`1 func) in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\ContextUtils.cs:line 18
           at NUnit.Framework.Internal.Execution.SimpleWorkItem.PerformWork() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Internal\Execution\SimpleWorkItem.cs:line 50
        """;
#endif

        // On any framework, the filtered trace should only contain the user methods that were called.
        private static readonly string ThrownExceptionInAsyncMethodTrace_Result = """
           at NUnit.TestData.StackFilterFixture.ThrowExceptionAsync() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\testdata\StackFilterFixture.cs:line 55
           at NUnit.TestData.StackFilterFixture.ThrowExceptionInAsyncUserMethod() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\testdata\StackFilterFixture.cs:line 62

        """;

        private static readonly string MonoStackTrace = """
          at NUnit.TestData.StackFilterFixture.ThrowException () [0x00001] in <97520dab6bc24aceb801551affd7b633>:0 
          at (wrapper managed-to-native) System.Reflection.RuntimeMethodInfo.InternalInvoke(System.Reflection.RuntimeMethodInfo,object,object[],System.Exception&)
        """;

        private static readonly string MonoStackTrace_Result = """
          at NUnit.TestData.StackFilterFixture.ThrowException () [0x00001] in <97520dab6bc24aceb801551affd7b633>:0 

        """;

        // Mono wrapper frames are removed, except where the exception was thrown and for dynamic-method and synchronized,
        // as those represent user code.
        private static readonly string MonoWrapperStackTrace = """
          at (wrapper castclass) System.Object.__castclass_with_cache(object,intptr,intptr)
          at (wrapper dynamic-method) System.Object.lambda_method(System.Runtime.CompilerServices.Closure,object)
          at (wrapper delegate-invoke) System.Func`2[System.Object,System.String].invoke_TResult_T(object)
          at (wrapper synchronized) MyNamespace.MyAppsTests.ConvertLocked(object)
          at MyNamespace.MyAppsTests.CastTest () [0x00001] in <97520dab6bc24aceb801551affd7b633>:0
          at (wrapper managed-to-native) System.Reflection.RuntimeMethodInfo.InternalInvoke(System.Reflection.RuntimeMethodInfo,object,object[],System.Exception&)
          at (wrapper runtime-invoke) object.runtime_invoke_void__this___object(object,intptr,intptr,intptr)
        """;

        private static readonly string MonoWrapperStackTrace_Result = """
          at (wrapper castclass) System.Object.__castclass_with_cache(object,intptr,intptr)
          at (wrapper dynamic-method) System.Object.lambda_method(System.Runtime.CompilerServices.Closure,object)
          at (wrapper synchronized) MyNamespace.MyAppsTests.ConvertLocked(object)
          at MyNamespace.MyAppsTests.CastTest () [0x00001] in <97520dab6bc24aceb801551affd7b633>:0

        """;

        // On Linux and macOS an async continuation can be posted to the default SynchronizationContext.
        private static readonly string SynchronizationContextPostTrace = """
           at NUnit.Framework.Assert.Warn(String message)
           at NUnit.TestData.WarningFixture.WarningAfterAwaitTaskYield() in /_/src/NUnitFramework/testdata/WarningFixture.cs:line 560
           at System.Threading.SynchronizationContext.<>c.<Post>b__8_0(KeyValuePair`2 s)
           at System.Threading.ThreadPoolWorkQueue.Dispatch()
        """;

        private static readonly string SynchronizationContextPostTrace_Result = """
           at NUnit.TestData.WarningFixture.WarningAfterAwaitTaskYield() in /_/src/NUnitFramework/testdata/WarningFixture.cs:line 560

        """;

        // System frames at the top of the stack show where the exception was thrown and must be kept.
        private static readonly string ThrownBySystemMethodTrace = """
           at System.Threading.Tasks.Task.ThrowIfExceptional(Boolean includeTaskCanceledExceptions)
           at System.Threading.Tasks.Task.Wait(Int32 millisecondsTimeout, CancellationToken cancellationToken)
           at System.Threading.Tasks.Task.Wait()
           at MyNamespace.MyAppsTests.WaitForFaultedTask()
           at System.Reflection.MethodBaseInvoker.InterpretedInvoke_Method(Object obj, IntPtr* args)
           at NUnit.Framework.Internal.Reflect.InvokeMethod(MethodInfo method, Object fixture, Object[] args)
        """;

        private static readonly string ThrownBySystemMethodTrace_Result = """
           at System.Threading.Tasks.Task.ThrowIfExceptional(Boolean includeTaskCanceledExceptions)
           at System.Threading.Tasks.Task.Wait(Int32 millisecondsTimeout, CancellationToken cancellationToken)
           at System.Threading.Tasks.Task.Wait()
           at MyNamespace.MyAppsTests.WaitForFaultedTask()

        """;

        // Everything below the NUnit execution pipeline belongs to the runner or host and is cut off.
        private static readonly string RunnerAndHostTrace = """
           at NUnit.Framework.Assert.Warn(String message)
           at MyNamespace.MyAppsTests.WarnTest()
           at NUnit.Framework.Internal.Reflect.InvokeMethod(MethodInfo method, Object fixture, Object[] args)
           at NUnit.Framework.Internal.Commands.TestMethodCommand.RunTestMethod(TestExecutionContext context)
           at MyNamespace.MyRunnerTests.RunInnerTest()
           at NUnit.Framework.Internal.Commands.TestMethodCommand.RunTestMethod(TestExecutionContext context)
           at Microsoft.VisualStudio.TestPlatform.TestHost.Program.Main(String[] args)
        """;

        private static readonly string RunnerAndHostTrace_Result = """
           at MyNamespace.MyAppsTests.WarnTest()

        """;

        // An exception thrown inside NUnit itself would be filtered away completely, so the raw trace is returned.
        private static readonly string ThrownInsideNUnitTrace = """
           at NUnit.Framework.Internal.TestExecutionContext.get_CurrentResult()
           at NUnit.Framework.Internal.Commands.TestMethodCommand.Execute(TestExecutionContext context)
        """;

        #endregion

        // NOTE: Using individual tests rather than test cases
        // to make the error messages easier to read.

        [Test]
        public void FilterShortTraceWithAssert()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(ShortTrace_Assert), Is.EqualTo(ShortTrace_Result));
        }

        [Test]
        public void FilterShortTraceWithAssume_Trace1()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(ShortTrace_Assume), Is.EqualTo(ShortTrace_Result));
        }

        [Test]
        public void FilterLongTrace()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(LongTrace), Is.EqualTo(LongTrace_Result));
        }

        [Test]
        public void FilterQueueUserWorkItemTrace()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(QueueUserItemTrace), Is.EqualTo(QueueUserItemTrace_Result));
        }

        [Test]
        public void FilterThrownExceptionInAsyncMethodTrace()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(ThrownExceptionInAsyncMethodTrace), Is.EqualTo(ThrownExceptionInAsyncMethodTrace_Result));
        }

#if NETFRAMEWORK
        // Mono writes nested types with '+' and includes generic parameters, unlike .NET Framework.
        private static readonly string MonoThrownExceptionInAsyncMethodTrace = """
          at NUnit.TestData.StackFilterFixture+<ThrowExceptionAsync>d__8`1[T].MoveNext () [0x0001e] in <97520dab6bc24aceb801551affd7b633>:0
        --- End of stack trace from previous location where exception was thrown ---
          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw () [0x00011] in <12b418a7818c4ca0893feeaaf67f1e7f>:0
          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess (System.Threading.Tasks.Task task) [0x0003e] in <12b418a7818c4ca0893feeaaf67f1e7f>:0
          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification (System.Threading.Tasks.Task task) [0x00028] in <12b418a7818c4ca0893feeaaf67f1e7f>:0
          at System.Runtime.CompilerServices.TaskAwaiter.GetResult () [0x00006] in <12b418a7818c4ca0893feeaaf67f1e7f>:0
          at NUnit.TestData.StackFilterFixture+<ThrowExceptionInAsyncUserMethod>d__7.MoveNext () [0x0004c] in <97520dab6bc24aceb801551affd7b633>:0
        --- End of stack trace from previous location where exception was thrown ---
          at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw () [0x00011] in <12b418a7818c4ca0893feeaaf67f1e7f>:0
          at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess (System.Threading.Tasks.Task task) [0x0003e] in <12b418a7818c4ca0893feeaaf67f1e7f>:0
          at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification (System.Threading.Tasks.Task task) [0x00028] in <12b418a7818c4ca0893feeaaf67f1e7f>:0
          at System.Runtime.CompilerServices.TaskAwaiter`1[TResult].GetResult () [0x00000] in <12b418a7818c4ca0893feeaaf67f1e7f>:0
          at NUnit.Framework.Internal.TaskAwaitAdapter+GenericAdapter`1[T].BlockUntilCompleted () [0x00000] in <4c62b2b2a4b04e9d8b6c5a8c2d9e4f3a>:0
          at NUnit.Framework.Internal.AsyncToSyncAdapter.Await (NUnit.Framework.Internal.TestExecutionContext context, System.Func`1[TResult] invoke) [0x0002d] in <4c62b2b2a4b04e9d8b6c5a8c2d9e4f3a>:0
          at NUnit.Framework.Internal.Commands.TestMethodCommand.RunTestMethod (NUnit.Framework.Internal.TestExecutionContext context) [0x00041] in <4c62b2b2a4b04e9d8b6c5a8c2d9e4f3a>:0
          at NUnit.Framework.Internal.Commands.TestMethodCommand.Execute (NUnit.Framework.Internal.TestExecutionContext context) [0x00000] in <4c62b2b2a4b04e9d8b6c5a8c2d9e4f3a>:0
        """;

        private static readonly string MonoThrownExceptionInAsyncMethodTrace_Result = """
          at NUnit.TestData.StackFilterFixture.ThrowExceptionAsync () [0x0001e] in <97520dab6bc24aceb801551affd7b633>:0
          at NUnit.TestData.StackFilterFixture.ThrowExceptionInAsyncUserMethod () [0x0004c] in <97520dab6bc24aceb801551affd7b633>:0

        """;

        [Test]
        public void FilterThrownExceptionInAsyncMethodTraceUnderMono()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(MonoThrownExceptionInAsyncMethodTrace), Is.EqualTo(MonoThrownExceptionInAsyncMethodTrace_Result));
        }
#endif

        [Test]
        public void FilterThrownExceptionUnderMono()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(MonoStackTrace), Is.EqualTo(MonoStackTrace_Result));
        }

        [Test]
        public void FilterMonoWrapperFrames()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(MonoWrapperStackTrace), Is.EqualTo(MonoWrapperStackTrace_Result));
        }

        [Test]
        public void FilterSynchronizationContextPostTrace()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(SynchronizationContextPostTrace), Is.EqualTo(SynchronizationContextPostTrace_Result));
        }

        [Test]
        public void FilterKeepsSystemFramesWhereExceptionWasThrown()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(ThrownBySystemMethodTrace), Is.EqualTo(ThrownBySystemMethodTrace_Result));
        }

        [Test]
        public void FilterCutsOffRunnerAndHostFrames()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(RunnerAndHostTrace), Is.EqualTo(RunnerAndHostTrace_Result));
        }

        [Test]
        public void CreateWithoutNUnitPatternKeepsNUnitFrames()
        {
            var filter = StackFilter.Create(nunitStackPattern: null);

            Assert.That(filter.Filter(ShortTrace_Assert), Is.EqualTo(ShortTrace_Assert + System.Environment.NewLine));
        }

        [Test]
        public void CreateWithoutRunnerPatternDoesNotCutOffTrace()
        {
            var filter = StackFilter.Create(nunitStackPattern: @"^ +\w+:? NUnit\.", systemStackPattern: null, runnerStackPattern: null);

            Assert.That(filter.Filter(RunnerAndHostTrace), Is.EqualTo("""
                   at MyNamespace.MyAppsTests.WarnTest()
                   at MyNamespace.MyRunnerTests.RunInnerTest()
                   at Microsoft.VisualStudio.TestPlatform.TestHost.Program.Main(String[] args)

                """));
        }

        [Test]
        [Obsolete("Tests the obsolete constructor")]
        public void ObsoleteConstructorCutsOffTraceAtBottomOfStackPattern()
        {
            var filter = new StackFilter(@"^ +\w+:? NUnit\.", @"^ +\w+:? Microsoft\.VisualStudio\.");

            Assert.That(filter.Filter(RunnerAndHostTrace), Is.EqualTo("""
                   at MyNamespace.MyAppsTests.WarnTest()
                   at MyNamespace.MyRunnerTests.RunInnerTest()

                """));
        }

        [Test]
        public void FilterReturnsRawTraceIfNothingIsLeft()
        {
            Assert.That(StackFilter.DefaultFilter.Filter(ThrownInsideNUnitTrace), Is.EqualTo(ThrownInsideNUnitTrace + Environment.NewLine));
        }

        [Test]
        public void FilterDoesNotAddSecondNewLineToRawTrace()
        {
            string rawTrace = ThrownInsideNUnitTrace + Environment.NewLine;

            Assert.That(StackFilter.DefaultFilter.Filter(rawTrace), Is.EqualTo(rawTrace));
        }

        [TestCase(nameof(StackFilterFixture.AssertThatFailure))]
        [TestCase(nameof(StackFilterFixture.AssertThatFailureInUserMethod), nameof(StackFilterFixture.AssertThatFailure), nameof(StackFilterFixture.AssertThatFailureInUserMethod))]
        [TestCase(nameof(StackFilterFixture.AssertThatFailureAsync))]
        [TestCase(nameof(StackFilterFixture.AssertThatFailureInAsyncUserMethod), nameof(StackFilterFixture.AssertThatFailureAsync))]
        [TestCase(nameof(StackFilterFixture.ThrowException))]
        [TestCase(nameof(StackFilterFixture.ThrowExceptionInUserMethod), nameof(StackFilterFixture.ThrowException), nameof(StackFilterFixture.ThrowExceptionInUserMethod))]
        [TestCase(nameof(StackFilterFixture.ThrowExceptionAsync))]
        [TestCase(nameof(StackFilterFixture.ThrowExceptionInAsyncUserMethod), nameof(StackFilterFixture.ThrowSpecifiedExceptionAsync), nameof(StackFilterFixture.ThrowExceptionInAsyncUserMethod))]
        // The type-qualified names are only present if the async state machine frames are rewritten.
        [TestCase(nameof(StackFilterFixture.ThrowExceptionInNestedClassAsyncMethod),
            nameof(StackFilterFixture.NestedClass) + "." + nameof(StackFilterFixture.NestedClass.ThrowSpecifiedExceptionAsync),
            nameof(StackFilterFixture.NestedClass) + "." + nameof(StackFilterFixture.NestedClass.ThrowExceptionInAsyncMethod),
            nameof(StackFilterFixture) + "." + nameof(StackFilterFixture.ThrowExceptionInNestedClassAsyncMethod))]
        public static void StackTracesAreFiltered(string methodName, params string[] methodsOnStack)
        {
            var result = TestBuilder.RunTestCase(typeof(StackFilterFixture), methodName);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.FailCount, Is.EqualTo(1));
                Assert.That(result.AssertionResults, Has.Count.EqualTo(1));
            }

            if (methodsOnStack.Length == 0)
                methodsOnStack = [methodName];

            var stackTrace = result.AssertionResults[0].StackTrace;
            Assert.That(stackTrace, Is.Not.Null);
            int numberOfStackTraceLines = stackTrace.Count(x => x == '\n');
            int expectedStackTraceLines = methodsOnStack.Length;

            if (numberOfStackTraceLines != expectedStackTraceLines)
            {
                StringBuilder sb = new();
                sb.AppendLine($"Expected {expectedStackTraceLines} method(s) on stack, but found {numberOfStackTraceLines}.");
                sb.AppendLine("Stacktrace:");
                sb.AppendLine(stackTrace);
                sb.AppendLine("Expected methods on stack:");
                sb.AppendLine(string.Join(System.Environment.NewLine, methodsOnStack));
                Assert.Fail(sb.ToString());
            }

            foreach (var otherMethod in methodsOnStack)
            {
                Assert.That(stackTrace, Does.Contain(otherMethod));
            }
        }
    }
}
