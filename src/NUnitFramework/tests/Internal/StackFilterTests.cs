// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using System.Text;
using NUnit.Framework.Internal;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class StackFilterTests
    {
        #region Test Data

        private static readonly string ShortTrace_Assert = """
           at NUnit.Framework.Assert.Fail(String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 56
           at NUnit.Framework.Assert.That(String label, Object actual, Matcher expectation, String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 50
           at NUnit.Framework.Assert.That(Object actual, Matcher expectation) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 19
           at NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:line 12
        """;

        private static readonly string ShortTrace_Assume = """
           at NUnit.Framework.Assert.Fail(String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 56
           at NUnit.Framework.Assume.That(String label, Object actual, Matcher expectation, String message) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 50
           at NUnit.Framework.Assume.That(Object actual, Matcher expectation) in D:\Dev\NUnitLite\NUnitLite\Framework\Assert.cs:line 19
           at NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:line 12
        """;

        private static readonly string ShortTrace_Result = """
           at NUnit.Tests.GreaterThanMatcherTest.MatchesGoodValue() in D:\Dev\NUnitLite\NUnitLiteTests\GreaterThanMatcherTest.cs:line 12

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
           at System.Environment.get_StackTrace()
           at NUnit.Framework.Assert.GetEnvironmentStackTraceWithoutThrowing() in D:\Development\3rd\NUnit\nunit\src\NUnitFramework\framework\Assert.cs:line 383
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

        [TestCase(nameof(StackFilterFixture.AssertThatFailure))]
        [TestCase(nameof(StackFilterFixture.AssertThatFailureInUserMethod), nameof(StackFilterFixture.AssertThatFailure), nameof(StackFilterFixture.AssertThatFailureInUserMethod))]
        [TestCase(nameof(StackFilterFixture.AssertThatFailureAsync))]
        [TestCase(nameof(StackFilterFixture.AssertThatFailureInAsyncUserMethod), nameof(StackFilterFixture.AssertThatFailureAsync))]
        [TestCase(nameof(StackFilterFixture.ThrowException))]
        [TestCase(nameof(StackFilterFixture.ThrowExceptionInUserMethod), nameof(StackFilterFixture.ThrowException), nameof(StackFilterFixture.ThrowExceptionInUserMethod))]
        [TestCase(nameof(StackFilterFixture.ThrowExceptionAsync))]
        [TestCase(nameof(StackFilterFixture.ThrowExceptionInAsyncUserMethod), nameof(StackFilterFixture.ThrowSpecifiedExceptionAsync), nameof(StackFilterFixture.ThrowExceptionInAsyncUserMethod))]

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

            // On .NET Framework async method are called from the state machine, so the actual test method is not on the stack.
            // On .NET Core this is corrected by the framework, so the actual test method is on the stack.
            // The actual test method only shows up on synchronous methods.
            // Similarly on all framework using a separate Thread or Task to run the warning, the actual test method is not on the stack.
            if (stackTrace.Contains(nameof(StackTracesAreFiltered)))
                expectedStackTraceLines++;

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

            Assert.That(numberOfStackTraceLines, Is.EqualTo(expectedStackTraceLines),
                "Mismatch in number of method(s) in stacktrace");

            foreach (var otherMethod in methodsOnStack)
            {
                Assert.That(stackTrace, Does.Contain(otherMethod));
            }
        }
    }
}
