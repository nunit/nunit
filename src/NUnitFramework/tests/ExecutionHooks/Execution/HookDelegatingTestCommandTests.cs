// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Tests.Attributes;

namespace NUnit.Framework.Tests.ExecutionHooks.Execution
{
    internal class HookDelegatingTestCommandTests
    {
        private class Prover
        {
            public bool WasInsideExecute { get; set; }
        }

        private class MockedTestCommand(Test test, Prover prover) : TestCommand(test)
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
            var mockedTestCommand = new MockedTestCommand(new TestDummy(), prover);
            var hookDelegatingTestCommandCommand = new HookDelegatingTestCommand(mockedTestCommand);

            Assert.That(prover.WasInsideExecute, Is.False);

            hookDelegatingTestCommandCommand.Execute(TestExecutionContext.CurrentContext);

            Assert.That(prover.WasInsideExecute, Is.True);
        }
    }
}
