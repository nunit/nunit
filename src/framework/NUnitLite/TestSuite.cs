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

namespace NUnitLite
{
    public class TestSuite : ITest, IComparable
    {
        #region Instance Variables
        private string name;
        private string fullName;

        private RunState runState = RunState.Runnable;
        private string ignoreReason;

        private IDictionary properties = new Hashtable();

        private ArrayList tests = new ArrayList(10);
        #endregion

        #region Constructors
        public TestSuite(string name)
        {
            this.name = name;
        }

        public TestSuite(Type type)
        {
            this.name = type.Name;
            this.fullName = type.FullName;
        }
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        public RunState RunState
        {
            get { return runState; }
            set { runState = value; }
        }

        public string IgnoreReason
        {
            get { return ignoreReason; }
            set { ignoreReason = value; }
        }

        public IDictionary Properties
        {
            get { return properties; }
        }

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

        public IList Tests
        {
            get { return tests; }
        }
        #endregion

        #region Public Methods
        public TestResult Run()
        {
            return Run(new NullListener());
        }

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
                    result.NotRun(this.IgnoreReason);
                    break;

                case RunState.Runnable:
                    foreach (ITest test in tests)
                    {
                        ++count;
                        TestResult r = test.Run(listener);
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
                        result.NotRun("Class has no tests");
                    else if (errors > 0 || failures > 0)
                        result.Failure("One or more component tests failed");
                    else
                        result.Success();
                    break;
            }

            listener.TestFinished(result);
            return result;
        }

        public void AddTest(ITest test)
        {
            tests.Add(test);
        }

        public void Sort()
        {
            tests.Sort();
            foreach (ITest test in tests)
            {
                TestSuite suite = test as TestSuite;
                if (suite != null)
                    suite.Sort();
            }
        }
        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            ITest other = obj as ITest;
            if (other == null)
                return -1;

            return this.Name.CompareTo(other.Name);
        }

        #endregion
    }
}
