// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// ParameterizedMethodSuite holds a collection of individual
    /// TestMethods with their arguments applied.
    /// </summary>
    public class ParameterizedMethodSuite : TestSuite
    {
        private readonly bool _isTheory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedMethodSuite"/> class.
        /// </summary>
        public ParameterizedMethodSuite(IMethodInfo method)
            : base(method.TypeInfo.FullName, method.Name)
        {
            base.Method = method;
            _isTheory = method.IsDefined<TheoryAttribute>(true);
            MaintainTestOrder = true;
        }

        /// <summary>
        /// Creates a copy of the given suite with only the descendants that pass the specified filter.
        /// </summary>
        /// <param name="suite">The <see cref="ParameterizedMethodSuite"/> to copy.</param>
        /// <param name="filter">Determines which descendants are copied.</param>
        public ParameterizedMethodSuite(ParameterizedMethodSuite suite, ITestFilter filter)
            : base(suite, filter)
        {
        }

        /// <summary>
        /// Gets a MethodInfo for the method implementing this test.
        /// </summary>
        public new IMethodInfo Method => base.Method!;

        /// <summary>
        /// Gets a string representing the type of test
        /// </summary>
        public override string TestType
        {
            get
            {
                if (_isTheory)
                    return "Theory";

                if (Method.ContainsGenericParameters)
                    return "GenericMethod";

                return "ParameterizedMethod";
            }
        }

        /// <summary>
        /// Creates a filtered copy of the test suite.
        /// </summary>
        /// <param name="filter">Determines which descendants are copied.</param>
        public override TestSuite Copy(ITestFilter filter)
        {
            return new ParameterizedMethodSuite(this, filter);
        }
    }
}
