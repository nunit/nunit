// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

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
        internal TestCaseParameters? Parms;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod"/> class.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        public TestMethod(IMethodInfo method) : base(method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestMethod"/> class.
        /// </summary>
        /// <param name="method">The method to be used as a test.</param>
        /// <param name="parentSuite">The suite or fixture to which the new test will be added</param>
        public TestMethod(IMethodInfo method, Test? parentSuite) : base(method)
        {
            // Needed to give proper fullname to test in a parameterized fixture.
            // Without this, the arguments to the fixture are not included.
            if (parentSuite is not null)
                FullName = parentSuite.FullName + "." + Name;
        }

        #endregion

        #region Properties

        internal bool HasExpectedResult => Parms is { HasExpectedResult: true };

        internal object? ExpectedResult => Parms?.ExpectedResult;

        #endregion

        #region Test Overrides

        /// <summary>
        /// Gets a MethodInfo for the method implementing this test.
        /// </summary>
        public new IMethodInfo Method { get => base.Method!; set => base.Method = value; }

        /// <summary>
        /// The arguments to use in executing the test method, or empty array if none are provided.
        /// </summary>
        public override object?[] Arguments => Parms is null ? TestParameters.NoArguments : Parms.Arguments;

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
        public override bool HasChildren => false;

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

            thisNode.AddAttribute("seed", Seed.ToString());

            return thisNode;
        }

        /// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value>A list of child tests</value>
        public override IList<ITest> Tests => Array.Empty<ITest>();

        /// <summary>
        /// Gets the name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public override string XmlElementName => "test-case";

        /// <summary>
        /// Returns the name of the method
        /// </summary>
        public override string MethodName => Method.Name;

        #endregion
    }
}
