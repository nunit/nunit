// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// EmptyTestCommand is a TestCommand that does nothing. It simply
    /// returns the current result from the context when executed. We
    /// use it to avoid testing for null when executing a chain of
    /// DelegatingTestCommands.
    /// </summary>
    public class EmptyTestCommand : TestCommand
    {
        /// <summary>
        /// Construct a NullCommand for a test
        /// </summary>
        public EmptyTestCommand(Test test) : base(test) { }

        /// <summary>
        /// Execute the command
        /// </summary>
        public override Task<TestResult> Execute(TestExecutionContext context)
        {
            return Task.FromResult(context.CurrentResult);
        }
    }
}
