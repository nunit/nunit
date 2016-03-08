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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

#if NET_4_0 || NET_4_5 || PORTABLE
using System.Threading.Tasks;
#endif

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// TestSuite represents a composite test, which contains other tests.
    /// </summary>
    public class TestSuite : Test
    {
        #region Fields

        /// <summary>
        /// Our collection of child tests
        /// </summary>
        private List<ITest> tests = new List<ITest>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="name">The name of the suite.</param>
        public TestSuite(string name) : base(name)
        {
            Arguments = new object[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="parentSuiteName">Name of the parent suite.</param>
        /// <param name="name">The name of the suite.</param>
        public TestSuite(string parentSuiteName, string name)
            : base(parentSuiteName, name)
        {
            Arguments = new object[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        public TestSuite(ITypeInfo fixtureType)
            : base(fixtureType)
        {
            Arguments = new object[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        public TestSuite(Type fixtureType)
            : base(new TypeWrapper(fixtureType))
        {
            Arguments = new object[0];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sorts tests under this suite.
        /// </summary>
        public void Sort()
        {
            if (!MaintainTestOrder)
            {
                this.tests.Sort();

                foreach (Test test in Tests)
                {
                    TestSuite suite = test as TestSuite;
                    if (suite != null)
                        suite.Sort();
                }
            }
        }

#if false
        /// <summary>
        /// Sorts tests under this suite using the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public void Sort(IComparer comparer)
        {
            this.tests.Sort(comparer);

            foreach( Test test in Tests )
            {
                TestSuite suite = test as TestSuite;
                if ( suite != null )
                    suite.Sort(comparer);
            }
        }
#endif

        /// <summary>
        /// Adds a test to the suite.
        /// </summary>
        /// <param name="test">The test.</param>
        public void Add(Test test)
        {
            test.Parent = this;
            tests.Add(test);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value>The list of child tests</value>
        public override IList<ITest> Tests
        {
            get { return tests; }
        }

        /// <summary>
        /// Gets a count of test cases represented by
        /// or contained under this test.
        /// </summary>
        /// <value></value>
        public override int TestCaseCount
        {
            get
            {
                int count = 0;

                foreach (Test test in Tests)
                {
                    count += test.TestCaseCount;
                }
                return count;
            }
        }

        /// <summary>
        /// The arguments to use in creating the fixture
        /// </summary>
        public object[] Arguments { get; internal set; }

        /// <summary>
        /// Set to true to suppress sorting this suite's contents
        /// </summary>
        protected bool MaintainTestOrder { get; set; }

        #endregion

        #region Test Overrides

        /// <summary>
        /// Overridden to return a TestSuiteResult.
        /// </summary>
        /// <returns>A TestResult for this test.</returns>
        public override TestResult MakeTestResult()
        {
            return new TestSuiteResult(this);
        }

        /// <summary>
        /// Gets a bool indicating whether the current test
        /// has any descendant tests.
        /// </summary>
        public override bool HasChildren
        {
            get
            {
                return tests.Count > 0;
            }
        }

        /// <summary>
        /// Gets the name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public override string XmlElementName
        {
            get { return "test-suite"; }
        }

        /// <summary>
        /// Returns an XmlNode representing the current result after
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode thisNode = parentNode.AddElement("test-suite");
            thisNode.AddAttribute("type", this.TestType);

            PopulateTestNode(thisNode, recursive);
            thisNode.AddAttribute("testcasecount", this.TestCaseCount.ToString());


            if (recursive)
                foreach (Test test in this.Tests)
                    test.AddToXml(thisNode, recursive);

            return thisNode;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check that setup and teardown methods marked by certain attributes
        /// meet NUnit's requirements and mark the tests not runnable otherwise.
        /// </summary>
        /// <param name="attrType">The attribute type to check for</param>
        protected void CheckSetUpTearDownMethods(Type attrType)
        {
            foreach (MethodInfo method in Reflect.GetMethodsWithAttribute(TypeInfo.Type, attrType, true))
                if (method.IsAbstract ||
                     !method.IsPublic && !method.IsFamily ||
                     method.GetParameters().Length > 0 ||
                     method.ReturnType != typeof(void)
#if NET_4_0 || NET_4_5 || PORTABLE
                     &&
                     method.ReturnType != typeof(Task)
#endif
                    )
                {
                    this.Properties.Set(
                        PropertyNames.SkipReason,
                        string.Format("Invalid signature for SetUp or TearDown method: {0}", method.Name));
                    this.RunState = RunState.NotRunnable;
                    break;
                }
        }

        #endregion
    }
}
