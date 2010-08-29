// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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

using System;
using NUnit.Framework.Api;
#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestCommand is the base class for all test commands
    /// </summary>
    public abstract class TestCommand : ITestCommand
    {
        private Test test;
        private TestResult result;
#if CLR_2_0 || CLR_4_0
        private IList<ITestCommand> children;
#else
        private IList children;
#endif

        public TestCommand(Test test)
        {
            this.test = test;
        }

        #region Public Properties and Methods

        public Test Test
        {
            get { return test; }
        }

        /// <summary>
        /// Gets any child TestCommands of this command
        /// </summary>
        /// <value>A list of child TestCommands</value>
#if CLR_2_0 || CLR_4_0
        public IList<ITestCommand> Children
        {
            get 
            { 
                if (children == null)
                    children = new List<ITestCommand>();

                return children;
            }
        }
#else
        public IList Children
        {
            get 
            { 
                if (children == null)
                    children = new ArrayList();

                return children;
            }
        }
#endif

        /// <summary>
        /// Runs the test, returning a TestResult.
        /// </summary>
        /// <param name="testObject">The object on which the test should run.</param>
        /// <returns>A TestResult</returns>
        public abstract TestResult Execute(object testObject);

        #endregion

        #region Protected Properties

        public virtual TestResult Result
        {
            get
            {
                return TestExecutionContext.CurrentContext.CurrentResult;
            }
            set
            {
                TestExecutionContext.CurrentContext.CurrentResult = value;
            }
        }

        /// <summary>
        /// Gets the result which is currently being built for this test.
        /// Returns null if accessed before the test begins execution.
        /// </summary>
        //protected TestResult CurrentResult
        //{
        //    get
        //    {
        //        return TestExecutionContext.CurrentContext.CurrentResult;
        //    }
        //    set
        //    {
        //        TestExecutionContext.CurrentContext.CurrentResult = value;
        //    }
        //}

        /// <summary>
        /// Gets the test for which this command was created. Since the
        /// value is stored in the TestExecutionContext, it may only be
        /// used during execution of the command. At other times, such
        /// as when the command is being constructed, it returns null,
        /// so any constructors that need the test should take it as
        /// an argument.
        /// </summary>
        //protected Test CurrentTest
        //{
        //    get
        //    {
        //        return TestExecutionContext.CurrentContext.CurrentTest;
        //    }
        //}

        #endregion
    }
}
