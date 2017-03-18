// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
    using Execution;
    using Interfaces;

    /// <summary>
    /// SetUpTearDownCommand runs any SetUp methods for a suite,
    /// runs the test and then runs any TearDown methods.
    /// </summary>
    public class SetUpTearDownCommand : BeforeAndAfterTestCommand
    {
        private IList<SetUpTearDownItem> _setUpTearDownItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpTearDownCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        public SetUpTearDownCommand(TestCommand innerCommand)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestMethod, "SetUpTearDownCommand may only apply to a TestMethod", "innerCommand");
            Guard.OperationValid(Test.TypeInfo != null, "TestMethod must have a non-null TypeInfo");

            _setUpTearDownItems = CommandBuilder.BuildSetUpTearDownList(Test.TypeInfo.Type, typeof(SetUpAttribute), typeof(TearDownAttribute));
        }

        /// <summary>
        /// Overridden to call RunSetUp on all items
        /// </summary>
        protected override void BeforeTest(TestExecutionContext context)
        {
            for (int i = _setUpTearDownItems.Count; i > 0;)
                _setUpTearDownItems[--i].RunSetUp(context);
        }

        /// <summary>
        /// Overridden to call RunTearDown on all items
        /// </summary>
        protected override void AfterTest(TestExecutionContext context)
        {
            for (int i = 0; i < _setUpTearDownItems.Count; i++)
                _setUpTearDownItems[i].RunTearDown(context);
        }
    }
}
