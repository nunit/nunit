using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Describes the action taken by an <see cref="ITestBuilder"/>.
    /// Can describe creating a suite of zero or more child tests, or having no effect.
    /// If no builder specifies a suite, the test will be a single test.
    /// </summary>
    public struct TestBuilderAction
    {
        private readonly ICollection<TestMethod> _suiteTests;

        /// <summary>
        /// The tests which the suite comprises, if any.
        /// </summary>
        public ICollection<TestMethod> SuiteTests
        {
            get { return _suiteTests ?? new TestMethod[0]; }
        }

        /// <summary>
        /// Indicates whether a suite should be created.
        /// If true, <see cref="SuiteTests"/> may have zero or more tests.
        /// If false, <see cref="SuiteTests"/> is empty.
        /// </summary>
        public bool IsSuite { get; }

        private TestBuilderAction(ICollection<TestMethod> suiteTests, bool isSuite)
        {
            _suiteTests = suiteTests;
            IsSuite = isSuite;
        }

        /// <summary>
        /// Converts the test to a suite and optionally adds test cases.
        /// </summary>
        /// <param name="tests">Zero or more test cases to be added.</param>
        public static TestBuilderAction Suite(IEnumerable<TestMethod> tests)
        {
            return new TestBuilderAction(tests.ToList(), true);
        }

        /// <summary>
        /// Has no effect on the building process.
        /// </summary>
        public static readonly TestBuilderAction None = default(TestBuilderAction);
    }
}