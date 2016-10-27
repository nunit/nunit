// ***********************************************************************
// Copyright (c) 2012 Charlie Poole
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

using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// The TestMethod class represents a Test implemented as a method.
    /// </summary>
    public class TestMethod : Test
    {
        #region Fields

        /// <summary>
        /// The ParameterSet used to create this test method
        /// </summary>
        internal TestCaseParameters parms;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod"/> class.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        public TestMethod(IMethodInfo method) : base (method) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod"/> class.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        /// <param name="parentSuite">The suite or fixture to which the new test will be added</param>
        public TestMethod(IMethodInfo method, Test parentSuite) : base(method ) 
        {
            // Needed to give proper fullname to test in a parameterized fixture.
            // Without this, the arguments to the fixture are not included.
            if (parentSuite != null)
                FullName = parentSuite.FullName + "." + Name;
        }

        #endregion

        #region Properties

        internal bool HasExpectedResult
        {
            get { return parms != null && parms.HasExpectedResult; }
        }

        internal object ExpectedResult
        {
            get { return parms != null ? parms.ExpectedResult : null; }
        }

        internal object[] Arguments
        {
            get { return parms != null ? parms.Arguments : null; }
        }

        #endregion

        #region Test Overrides

        /// <summary>
        /// Overridden to return a TestCaseResult.
        /// </summary>
        /// <returns>A TestResult for this test.</returns>
        public override TestResult MakeTestResult()
        {
            return new TestCaseResult(this);
        }

        /// <summary>
        /// Gets a bool indicating whether the current test
        /// has any descendant tests.
        /// </summary>
        public override bool HasChildren
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a TNode representing the current result after
        /// adding it as a child of the supplied parent node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="recursive">If true, descendant results are included</param>
        /// <returns></returns>
        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode thisNode = parentNode.AddElement(XmlElementName);

            PopulateTestNode(thisNode, recursive);

            thisNode.AddAttribute("seed", this.Seed.ToString());

            return thisNode;
        }

        /// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value>A list of child tests</value>
        public override IList<ITest> Tests
        {
            get { return new ITest[0]; }
        }

        /// <summary>
        /// Gets the name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public override string XmlElementName
        {
            get { return "test-case"; }
        }

        /// <summary>
        /// Returns the name of the method
        /// </summary>
        public override string MethodName
        {
            get { return Method.Name; }
        }

        #endregion
    }
}
