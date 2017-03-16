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

using System;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Commands
{
    /// <summary>
    /// OneTimeSetUpCommand runs any one-time setup methods for a suite,
    /// constructing the user test object if necessary.
    /// </summary>
    public class OneTimeSetUpCommand : DelegatingTestCommand
    {
        private readonly TestSuite _suite;
        private readonly ITypeInfo _typeInfo;
        private readonly object[] _arguments;
        private readonly List<SetUpTearDownItem> _setUpTearDown;

        /// <summary>
        /// Constructs a OneTimeSetUpCommand for a suite
        /// </summary>
        /// <param name="innerCommand">The inner command to which the command applies</param>
        /// <param name="setUpTearDown">A SetUpTearDownList for use by the command</param>
        public OneTimeSetUpCommand(TestCommand innerCommand, List<SetUpTearDownItem> setUpTearDown)
            : base(innerCommand) 
        {
            Guard.ArgumentValid(innerCommand.Test is TestSuite, "OneTimeSetUpCommand must reference a TestSuite", "innerCommand");

            _suite = (TestSuite)innerCommand.Test;
            _typeInfo = _suite.TypeInfo;
            _arguments = _suite.Arguments;
            _setUpTearDown = setUpTearDown;
        }

        /// <summary>
        /// Overridden to run the one-time setup for a suite.
        /// </summary>
        /// <param name="context">The TestExecutionContext to be used.</param>
        /// <returns>A TestResult</returns>
        public override TestResult Execute(TestExecutionContext context)
        {
            if (_typeInfo != null)
            {
                // Use pre-constructed fixture if available, otherwise construct it
                if (!_typeInfo.IsStaticClass)
                {
                    context.TestObject = _suite.Fixture ?? _typeInfo.Construct(_arguments);
                    if (_suite.Fixture == null)
                    {
                        _suite.Fixture = context.TestObject;
                    }
                    Test.Fixture = _suite.Fixture;
                }

                for (int i = _setUpTearDown.Count; i > 0; )
                    _setUpTearDown[--i].RunSetUp(context);
            }

            return innerCommand.Execute(context);
        }
    }
}
