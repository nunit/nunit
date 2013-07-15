// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// SuiteCommand is the abstract base for all SuiteCommands
    /// used by the framework. A SuiteCommand encapsulates the
    /// one-time setup and teardown needed for running a suite.
    /// </summary>
    public abstract class SuiteCommand
    {
        private readonly TestSuite _suite;

        /// <summary>
        /// Construct a SuiteCommand for a TestSuite
        /// </summary>
        /// <param name="suite">The suite associated with this command</param>
        public SuiteCommand(TestSuite suite)
        {
            _suite = suite;
        }

        /// <summary>
        /// Get the TestSuite associated with this command.
        /// </summary>
        public TestSuite Suite
        {
            get { return _suite; }
        }

        /// <summary>
        /// Does the one time set up for a suite command. Broadly defined,
        /// this includes:
        ///   1. Applying changes specified by attributes to the context
        ///   2. Constructing the user fixture instance
        ///   3. Calling the one time setup methods themselves
        /// </summary>
        /// <param name="context">The execution context to use in running the test.</param>
        public abstract void DoOneTimeSetUp(TestExecutionContext context);

        /// <summary>
        /// Does the one time tear down for a suite command.
        /// </summary>
        /// <param name="context">The execution context to use in running the test.</param>
        public abstract void DoOneTimeTearDown(TestExecutionContext context);
    }
}