using System;

namespace NUnit.Framework.Internal
{
    public class TestExecutionContextCommand : DelegatingTestCommand
    {
        public TestExecutionContextCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
        }

        public override TestResult Execute(object testObject)
        {
            TestExecutionContext.Save();

            TestExecutionContext.CurrentContext.CurrentTest = this.Test;
            TestExecutionContext.CurrentContext.CurrentResult = new TestResult(this.Test);

            try
            {
                return innerCommand.Execute(testObject);
            }
                // TODO: Ensure no exceptions escape
            finally
            {
                Result.AssertCount = TestExecutionContext.CurrentContext.AssertCount;
                this.Test.Fixture = null;

                TestExecutionContext.Restore();
            }
        }
    }
}
