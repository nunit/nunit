// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// Marks a test assembly, fixture or method that may be run in parallel.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ParallelizableAttribute : PropertyAttribute, IApplyToContext
    {
        /// <summary>
        /// Construct a ParallelizableAttribute using default ParallelScope.Self.
        /// </summary>
        public ParallelizableAttribute() : this(ParallelScope.Self) { }

        /// <summary>
        /// Construct a ParallelizableAttribute with a specified scope.
        /// </summary>
        /// <param name="scope">The ParallelScope associated with this attribute.</param>
        public ParallelizableAttribute(ParallelScope scope) : base()
        {
            Scope = scope;

            Properties.Set(PropertyNames.ParallelScope, scope);
        }

        /// <summary>
        /// Defines the degree to which this test and its descendants may be run in parallel
        /// </summary>
        public ParallelScope Scope { get; }

        /// <summary>
        /// Overridden to check for invalid combinations of settings
        /// </summary>
        /// <param name="test"></param>
        public override void ApplyToTest(Test test)
        {
            // Adjust property to include ParallelScope.Self if a fixture has ParallelScope.Fixtures on it
            if (test is TestFixture && Scope.HasFlag(ParallelScope.Fixtures))
                Properties.Set(PropertyNames.ParallelScope, Scope | ParallelScope.Self);

            base.ApplyToTest(test);

            if (test.RunState == RunState.NotRunnable)
                return;

            if (Scope.HasFlag(ParallelScope.Self) && Scope.HasFlag(ParallelScope.None))
            {
                test.MakeInvalid("Test may not be both parallel and non-parallel");
            }
            else if (Scope.HasFlag(ParallelScope.Fixtures))
            {
                if (test is TestMethod || test is ParameterizedMethodSuite)
                    test.MakeInvalid("May not specify ParallelScope.Fixtures on a test method");
            }
            else if (Scope.HasFlag(ParallelScope.Children))
            {
                if (test is TestMethod)
                    test.MakeInvalid("May not specify ParallelScope.Children on a non-parameterized test method");
            }
        }

        #region IApplyToContext Interface

        /// <summary>
        /// Modify the context to be used for child tests
        /// </summary>
        /// <param name="context">The current TestExecutionContext</param>
        public void ApplyToContext(TestExecutionContext context)
        {
            // Don't reflect Self in the context, since it will be
            // used for descendant tests.
            context.ParallelScope = Scope & ParallelScope.ContextMask;
        }

        #endregion
    }
}
