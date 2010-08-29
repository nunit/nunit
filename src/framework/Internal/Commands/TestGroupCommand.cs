using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    public class TestGroupCommand : TestCommand
    {
        public TestGroupCommand(Test test) : base(test)
        {
        }

        //public TestGroupCommand(TestSuite suite, IList<TestCommand> commands)
        //{
        //    this.commands = commands;
        //}

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
