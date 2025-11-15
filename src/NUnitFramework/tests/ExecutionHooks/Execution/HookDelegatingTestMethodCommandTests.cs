// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class HookDelegatingTestMethodCommandTests
    {
        private class Prover
        {
            public bool WasInsideExecute { get; set; }
        }

        private class MockedTestMethodCommand(TestMethod test, Prover prover) : TestMethodCommand(test)
        {
            public override TestResult Execute(TestExecutionContext context)
            {
                prover.WasInsideExecute = true;
                return context.CurrentResult;
            }
        }

        [Test]
        public void InnerCommandIsAlwaysExecuted()
        {
            var prover = new Prover();
            var testMethodDummy = new TestMethod(new MethodWrapper(GetType(), nameof(InnerCommandIsAlwaysExecuted)));
            var mockedTestCommand = new MockedTestMethodCommand(testMethodDummy, prover);
            var hookDelegatingTestMethodCommand = new HookDelegatingTestMethodCommand(mockedTestCommand);

            Assert.That(prover.WasInsideExecute, Is.False);

            TestExecutionContext currentContext = TestExecutionContext.CurrentContext;

            // Emulate what the engine does in order to enable execution hooks
            _ = currentContext.GetOrCreateExecutionHooks();
            hookDelegatingTestMethodCommand.Execute(currentContext);

            Assert.That(prover.WasInsideExecute, Is.True);
        }
    }
}
