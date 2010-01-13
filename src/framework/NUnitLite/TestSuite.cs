// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace NUnitLite
{
    /// <summary>
    /// TestSuite represents a collection of tests
    /// </summary>
    public class TestSuite : ITest
    {
        #region Instance Variables
        private int id;
        private string name;
        private string fullName;

        private RunState runState = RunState.Runnable;
        private string ignoreReason;

        private IDictionary properties = new Hashtable();

        private NUnit.ObjectList tests = new NUnit.ObjectList();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="name">The name of the suite.</param>
        public TestSuite(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="type">The type used to create the suite.</param>
        public TestSuite(Type type)
        {
            this.name = type.Name;
            this.fullName = type.FullName;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the id of the test
        /// </summary>
        /// <value></value>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets the name of the suite.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the full name of the suite.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName
        {
            get { return fullName; }
        }

        /// <summary>
        /// Gets or sets the run state of the suite.
        /// </summary>
        /// <value>The run state.</value>
        public RunState RunState
        {
            get { return runState; }
            set { runState = value; }
        }

        /// <summary>
        /// Gets or sets the ignore reason.
        /// </summary>
        /// <value>The ignore reason.</value>
        public string IgnoreReason
        {
            get { return ignoreReason; }
            set { ignoreReason = value; }
        }

        /// <summary>
        /// Gets the properties collection for this suite.
        /// </summary>
        /// <value>The properties.</value>
        public IDictionary Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Gets the test case count.
        /// </summary>
        /// <value>The test case count.</value>
        public int TestCaseCount
        {
            get
            {
                int count = 0;
                foreach (ITest test in this.tests)
                    count += test.TestCaseCount;
                return count;
            }
        }

        /// <summary>
        /// True if this is a test case
        /// </summary>
        public bool IsTestCase
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the tests.
        /// </summary>
        /// <value>The tests.</value>
        public IList Tests
        {
            get { return tests; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Runs the suite.
        /// </summary>
        /// <returns>A TestResult</returns>
        public TestResult Run()
        {
            return Run(new NullListener());
        }

        /// <summary>
        /// Runs this test
        /// </summary>
        /// <param name="listener">A TestListener to handle test events</param>
        /// <returns>A TestResult</returns>
        public TestResult Run(TestListener listener)
        {
            int count = 0, failures = 0, errors = 0;
            listener.TestStarted(this);
            TestResult result = new TestResult(this);

            switch (this.RunState)
            {
                case RunState.NotRunnable:
                    result.Error(this.IgnoreReason);
                    break;

                case RunState.Ignored:
                    result.Ignore(this.IgnoreReason);
                    break;

                case RunState.Runnable:
                    foreach (ITest test in tests)
                    {
                        ++count;
                        // TODO: Temporary
                        TestResult r = test is TestCase
                            ? ((TestCase)test).Run(listener)
                            : ((TestSuite)test).Run(listener);
                        result.AddResult(r);
                        switch (r.ResultState)
                        {
                            case ResultState.Error:
                                ++errors;
                                break;
                            case ResultState.Failure:
                                ++failures;
                                break;
                            default:
                                break;
                        }
                    }

                    if (count == 0)
                        result.Ignore("Class has no tests");
                    else if (errors > 0 || failures > 0)
                        result.Failure("One or more component tests failed");
                    else
                        result.Success();
                    break;
            }

            listener.TestFinished(result);
            return result;
        }

        /// <summary>
        /// Adds the test.
        /// </summary>
        /// <param name="test">The test.</param>
        public void AddTest(ITest test)
        {
            tests.Add(test);
        }
        #endregion
    }
}
