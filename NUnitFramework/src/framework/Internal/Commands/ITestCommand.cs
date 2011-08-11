using System;
using NUnit.Framework.Api;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The ITestCommand interface is implemented by an
    /// object that knows how to run a test.
    /// </summary>
    public interface ITestCommand
    {
        /// <summary>
        /// Gets the Test to which this command applies.
        /// </summary>
        Test Test { get; }

        /// <summary>
        /// Gets any child TestCommands of this command
        /// </summary>
        /// <value>A list of child TestCommands</value>
#if CLR_2_0 || CLR_4_0
        System.Collections.Generic.IList<ITestCommand> Children { get; }
#else
        System.Collections.IList Children { get; }
#endif

        /// <summary>
        /// Runs the test, returning a TestResult.
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <param name="arguments">The arguments to be used in running the test or null.</param>
        /// <returns>A TestResult</returns>
        TestResult Execute(object testObject, ITestListener listener);
    }
}
