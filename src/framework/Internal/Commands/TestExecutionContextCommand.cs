using System;

namespace NUnit.Framework.Internal
{
    ///<summary>
    /// TODO: Documentation needed for class
    ///</summary>
    public class TestExecutionContextCommand : DelegatingTestCommand
    {
        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="innerCommand"></param>
        public TestExecutionContextCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
        }

        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="testObject"></param>
        /// <returns></returns>
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
