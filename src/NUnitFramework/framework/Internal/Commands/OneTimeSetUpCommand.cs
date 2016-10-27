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
    public class OneTimeSetUpCommand : TestCommand
    {
        private readonly TestSuite _suite;
        private readonly ITypeInfo _typeInfo;
        private readonly object[] _arguments;
        private readonly List<SetUpTearDownItem> _setUpTearDown;
        private readonly List<TestActionItem> _actions;

        /// <summary>
        /// Constructs a OneTimeSetUpCommand for a suite
        /// </summary>
        /// <param name="suite">The suite to which the command applies</param>
        /// <param name="setUpTearDown">A SetUpTearDownList for use by the command</param>
        /// <param name="actions">A List of TestActionItems to be run after Setup</param>
        public OneTimeSetUpCommand(TestSuite suite, List<SetUpTearDownItem> setUpTearDown, List<TestActionItem> actions)
            : base(suite) 
        {
            _suite = suite;
            _typeInfo = suite.TypeInfo;
            _arguments = suite.Arguments;
            _setUpTearDown = setUpTearDown;
            _actions = actions;
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


            for (int i = 0; i < _actions.Count; i++)
                _actions[i].BeforeTest(Test);

            return context.CurrentResult;
        }
    }
}
