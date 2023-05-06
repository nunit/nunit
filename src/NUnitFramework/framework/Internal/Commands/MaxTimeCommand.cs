 // Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Diagnostics;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// <see cref="MaxTimeCommand" /> adjusts the result of a successful test
    /// to a failure if the elapsed time has exceeded the specified maximum
    /// time allowed.
    /// </summary>
    public class MaxTimeCommand : AfterTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxTimeCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="maxTime">The max time allowed in milliseconds</param>
        public MaxTimeCommand(TestCommand innerCommand, int maxTime)
            : base(innerCommand)
        {
            AfterTest = context =>
            {
                // TODO: This command duplicates the calculation of the
                // duration of the test because that calculation is
                // normally performed at a higher level. Most likely,
                // we should move the maxtime calculation to the
                // higher level eventually.

                long tickCount = Stopwatch.GetTimestamp() - context.StartTicks;
                double seconds = (double)tickCount / Stopwatch.Frequency;
                TestResult result = context.CurrentResult;

                result.Duration = seconds;

                if (result.ResultState == ResultState.Success)
                {
                    double elapsedTime = result.Duration * 1000d;

                    if (elapsedTime > maxTime)
                    {
                        result.SetResult(ResultState.Failure,
                            $"Elapsed time of {elapsedTime}ms exceeds maximum of {maxTime}ms");
                    }
                }
            };
        }
    }
}
