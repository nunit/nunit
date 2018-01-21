// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;

namespace NUnit.Framework
{
    /// <summary>
    /// ParallelizableAttribute is used to mark tests that may be run in parallel.
    /// </summary>
    [AttributeUsage( AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false, Inherited=true )]
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
