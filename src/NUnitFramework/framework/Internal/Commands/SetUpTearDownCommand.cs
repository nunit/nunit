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
using System.Threading;

namespace NUnit.Framework.Internal.Commands
{
    using Execution;
    using Interfaces;

    /// <summary>
    /// SetUpTearDownCommand runs SetUp methods for a suite,
    /// runs the test and then runs TearDown methods.
    /// </summary>
    public class SetUpTearDownCommand : BeforeAndAfterTestCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetUpTearDownCommand"/> class.
        /// </summary>
        /// <param name="innerCommand">The inner command.</param>
        /// <param name="setUpTearDown">List of setup/teardown items</param>
        public SetUpTearDownCommand(TestCommand innerCommand, SetUpTearDownItem setUpTearDown)
            : base(innerCommand)
        {
            Guard.ArgumentValid(innerCommand.Test is TestMethod, "SetUpTearDownCommand may only apply to a TestMethod", nameof(innerCommand));
            Guard.OperationValid(Test.Type != null, "TestMethod must have a non-null fixture type");
            Guard.ArgumentNotNull(setUpTearDown, nameof(setUpTearDown));

            BeforeTest = (context) =>
            {
                setUpTearDown.RunSetUp(context);
            };

            AfterTest = (context) =>
            {
                setUpTearDown.RunTearDown(context);
            };
        }
    }
}
