// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;

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
        private readonly List<ITest> _tests = new();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="name">The name of the suite.</param>
        public TestSuite(string name) : base(name)
        {
            Arguments = TestParameters.NoArguments;
            OneTimeSetUpMethods = Array.Empty<IMethodInfo>();
            OneTimeTearDownMethods = Array.Empty<IMethodInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="parentSuiteName">Name of the parent suite.</param>
        /// <param name="name">The name of the suite.</param>
        public TestSuite(string? parentSuiteName, string name)
            : base(parentSuiteName, name)
        {
            Arguments = TestParameters.NoArguments;
            OneTimeSetUpMethods = Array.Empty<IMethodInfo>();
            OneTimeTearDownMethods = Array.Empty<IMethodInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        /// <param name="arguments">Arguments used to instantiate the test fixture, or null if none used.</param>
        public TestSuite(ITypeInfo fixtureType, object?[]? arguments = null)
            : base(fixtureType)
        {
            Arguments = arguments ?? TestParameters.NoArguments;
            OneTimeSetUpMethods = Array.Empty<IMethodInfo>();
            OneTimeTearDownMethods = Array.Empty<IMethodInfo>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestSuite"/> class.
        /// </summary>
        /// <param name="fixtureType">Type of the fixture.</param>
        public TestSuite(Type fixtureType)
            : base(new TypeWrapper(fixtureType))
        {
            Arguments = TestParameters.NoArguments;
            OneTimeSetUpMethods = Array.Empty<IMethodInfo>();
            OneTimeTearDownMethods = Array.Empty<IMethodInfo>();
        }

        /// <summary>
        /// Creates a copy of the given suite with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="suite">The <see cref="TestSuite"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        public TestSuite(TestSuite suite, ITestFilter filter)
            : this(suite.Name)
        {
            FullName = suite.FullName;
            Method = suite.Method;
            RunState = suite.RunState;
            Fixture = suite.Fixture;

            foreach (string key in suite.Properties.Keys)
            {
                foreach (object val in suite.Properties[key])
                    Properties.Add(key, val);
            }

            foreach (var child in suite._tests)
            {
                if (filter.Pass(child))
                {
                    if (child.IsSuite)
                    {
                        TestSuite childSuite = ((TestSuite)child).Copy(filter);
                        childSuite.Parent = this;
                        _tests.Add(childSuite);
                    }
                    else
                    {
                        _tests.Add(child);
                    }
                }
            }
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
                _tests.Sort();

                foreach (Test test in Tests)
                {
                    if (test is TestSuite suite)
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
            _tests.Add(test);
        }

        /// <summary>
        /// Creates a filtered copy of the test suite.
        /// </summary>
        /// <param name="filter">Determines which descendants are copied.</param>
        public virtual TestSuite Copy(ITestFilter filter)
        {
            return new TestSuite(this, filter);
        }

        /// <summary>
        /// Recursively apply the attributes on <paramref name="type"/> to this test suite,
        /// including attributes on nesting types.
        /// </summary>
        /// <param name="type">The </param>
        public void ApplyAttributesToTestSuite(Type type)
        {
            foreach (var t in GetNestedTypes(type).Reverse())
                ApplyAttributesToTestSuite((ICustomAttributeProvider)t);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Modify a newly constructed testSuite by applying any of NUnit's common
        /// attributes, based on a supplied <see cref="ICustomAttributeProvider"/>, which is
        /// usually the reflection element from which the test was constructed,
        /// but may not be in some instances. The attributes retrieved are
        /// saved for use in subsequent operations.
        /// </summary>
        private void ApplyAttributesToTestSuite(ICustomAttributeProvider provider)
        {
            IApplyToTestSuite[] applyToTestSuiteAttributes = provider.GetAttributes<IApplyToTestSuite>(inherit: true);
            ApplyAttributesToTestSuite(applyToTestSuiteAttributes);
        }

        private void ApplyAttributesToTestSuite(IEnumerable<IApplyToTestSuite> attributes)
        {
            foreach (IApplyToTestSuite iApply in attributes)
                iApply.ApplyToTestSuite(this);
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets this test's child tests
        /// </summary>
        /// <value>The list of child tests</value>
        public override IList<ITest> Tests => _tests;

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

                // Use for-loop to avoid allocating the enumerator
                var testsToCheck = Tests;
                for (var i = 0; i < testsToCheck.Count; i++)
                {
                    count += testsToCheck[i].TestCaseCount;
                }

                return count;
            }
        }

        /// <summary>
        /// The arguments to use in creating the fixture, or empty array if none are provided.
        /// </summary>
        public override object?[] Arguments { get; }

        /// <summary>
        /// Set to true to suppress sorting this suite's contents
        /// </summary>
        protected bool MaintainTestOrder { get; set; }

        /// <summary>
        /// OneTimeSetUp methods for this suite
        /// </summary>
        public IMethodInfo[] OneTimeSetUpMethods { get; protected set; }

        /// <summary>
        /// OneTimeTearDown methods for this suite
        /// </summary>
        public IMethodInfo[] OneTimeTearDownMethods { get; protected set; }

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
        public override bool HasChildren => _tests.Count > 0;

        /// <summary>
        /// Gets the name used for the top-level element in the
        /// XML representation of this test
        /// </summary>
        public override string XmlElementName => "test-suite";

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
            thisNode.AddAttribute("type", TestType);

            PopulateTestNode(thisNode, recursive);
            thisNode.AddAttribute("testcasecount", TestCaseCount.ToString());

            if (recursive)
            {
                // Use for-loop to avoid allocating the enumerator
                var testsToAdd = Tests;
                for (var i = 0; i < testsToAdd.Count; i++)
                {
                    var test = testsToAdd[i];
                    test.AddToXml(thisNode, recursive);
                }
            }

            return thisNode;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check that setup and teardown methods marked by certain attributes
        /// meet NUnit's requirements and mark the tests not runnable otherwise.
        /// </summary>
        protected void CheckSetUpTearDownMethods(IMethodInfo[] methods)
        {
            foreach (IMethodInfo method in methods)
            {
                var methodInfo = MethodInfoCache.Get(method);

                if (method.IsAbstract)
                {
                    MakeInvalid("An abstract SetUp and TearDown methods cannot be run: " + method.Name);
                }
                else if (!(method.IsPublic || method.MethodInfo.IsFamily))
                {
                    MakeInvalid("SetUp and TearDown methods must be public or protected: " + method.Name);
                }
                else if (methodInfo.Parameters.Length != 0)
                {
                    MakeInvalid("SetUp and TearDown methods must not have parameters: " + method.Name);
                }
                else if (methodInfo.IsAsyncOperation)
                {
                    if (method.ReturnType.Type == typeof(void))
                        MakeInvalid("SetUp and TearDown methods must not be async void: " + method.Name);
                    else if (!Reflect.IsVoidOrUnit(AwaitAdapter.GetResultType(method.ReturnType.Type)))
                        MakeInvalid("SetUp and TearDown methods must return void or an awaitable type with a void result: " + method.Name);
                }
                else
                {
                    if (!methodInfo.IsVoidOrUnit)
                        MakeInvalid("SetUp and TearDown methods must return void or an awaitable type with a void result: " + method.Name);
                }
            }
        }
        #endregion
    }
}
