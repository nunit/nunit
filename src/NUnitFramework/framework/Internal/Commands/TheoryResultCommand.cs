// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// TheoryResultCommand adjusts the result of a Theory so that
    /// it fails if all the results were inconclusive.
    /// </summary>
    public class TheoryResultCommand : AfterTestCommand
    {
        /// <summary>
        /// Constructs a TheoryResultCommand 
        /// </summary>
        /// <param name="command">The command to be wrapped by this one</param>
        public TheoryResultCommand(TestCommand command) : base(command)
        {
            AfterTest = (context) =>
            {
                TestResult theoryResult = context.CurrentResult;

                if (theoryResult.ResultState == ResultState.Inconclusive)
                {
                    if (!theoryResult.HasChildren)
                        theoryResult.SetResult(ResultState.Failure, "No test cases were provided");
                    else
                        theoryResult.SetResult(ResultState.Failure, "All test cases were inconclusive");
                }
            };
        }
    }
}
