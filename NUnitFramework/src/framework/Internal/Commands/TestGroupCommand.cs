#if UNUSED
using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Commands
{
    ///<summary>
    /// TODO: Documentation needed for class
    ///</summary>
    public class TestGroupCommand : TestCommand
    {
        /// <summary>
        /// TODO: Documentation needed for constructor
        /// </summary>
        /// <param name="test"></param>
        public TestGroupCommand(Test test) : base(test)
        {
        }

        /// <summary>
        /// TODO: Documentation needed for method
        /// </summary>
        /// <param name="testObject"></param>
        /// <returns></returns>
        public override TestResult Execute(object testObject)
        {
            Result.SetResult(ResultState.Success);

            foreach (TestCommand command in Children)
            {
                TestResult childResult = command.Execute(testObject);

                Result.AddResult(childResult);

                if (childResult.ResultState == ResultState.Cancelled)
                    break;
            }

            return Result;
        }
    }
}
#endif
