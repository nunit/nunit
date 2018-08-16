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
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// SetUpTearDownItem holds the setup and teardown methods
    /// for a single level of the inheritance hierarchy.
    /// </summary>
    public class SetUpTearDownItem
    {
        private readonly IList<MethodInfo> _setUpMethods;
        private readonly IList<MethodInfo> _tearDownMethods;
        private bool _setUpWasRun;

        /// <summary>
        /// Construct a SetUpTearDownNode
        /// </summary>
        /// <param name="setUpMethods">A list of setup methods for this level</param>
        /// <param name="tearDownMethods">A list teardown methods for this level</param>
        public SetUpTearDownItem(IList<MethodInfo> setUpMethods, IList<MethodInfo> tearDownMethods)
        {
            _setUpMethods = setUpMethods;
            _tearDownMethods = tearDownMethods;
        }

        /// <summary>
        ///  Returns true if this level has any methods at all.
        ///  This flag is used to discard levels that do nothing.
        /// </summary>
        public bool HasMethods
        {
            get { return _setUpMethods.Count > 0 || _tearDownMethods.Count > 0; }
        }

        /// <summary>
        /// Run SetUp on this level.
        /// </summary>
        /// <param name="context">The execution context to use for running.</param>
        public void RunSetUp(TestExecutionContext context)
        {
            _setUpWasRun = true;

            foreach (MethodInfo setUpMethod in _setUpMethods)
                RunSetUpOrTearDownMethod(context, setUpMethod);
        }

        /// <summary>
        /// Run TearDown for this level.
        /// </summary>
        /// <param name="context"></param>
        public void RunTearDown(TestExecutionContext context)
        {
            // As of NUnit 3.0, we will only run teardown at a given
            // inheritance level if we actually ran setup at that level.
            if (_setUpWasRun)
                try
                {
                    // Count of assertion results so far
                    var oldCount = context.CurrentResult.AssertionResults.Count;

                    // Even though we are only running one level at a time, we
                    // run the teardowns in reverse order to provide consistency.
                    int index = _tearDownMethods.Count;
                    while (--index >= 0)
                        RunSetUpOrTearDownMethod(context, _tearDownMethods[index]);

                    // If there are new assertion results here, they are warnings issued
                    // in teardown. Redo test completion so they are listed properly.
                    if (context.CurrentResult.AssertionResults.Count > oldCount)
                        context.CurrentResult.RecordTestCompletion();
                }
                catch (Exception ex)
                {
                    context.CurrentResult.RecordTearDownException(ex);
                }
        }

        private static void RunSetUpOrTearDownMethod(TestExecutionContext context, MethodInfo method)
        {
            Guard.ArgumentNotAsyncVoid(method, nameof(method));
#if ASYNC
            if (AsyncToSyncAdapter.IsAsyncOperation(method))
                AsyncToSyncAdapter.Await(() => InvokeMethod(method, context));
            else
#endif
                InvokeMethod(method, context);
        }

        private static object InvokeMethod(MethodInfo method, TestExecutionContext context)
        {
            return Reflect.InvokeMethod(method, method.IsStatic ? null : context.TestObject);
        }
    }
}
