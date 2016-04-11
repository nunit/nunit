// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

namespace NUnit.Framework.Internal.Filters
{
    /// <summary>
    /// Mocks a <see cref="TestFilter"/>. Checks that only one specific match-function 
    /// (<see cref=MatchFunction"/> is called and only for a specific <see cref="ITest"/>.
    /// 
    /// For the specific test and match-function one could set the return value of the function.
    /// Furthermore one could read out the number of valid calls to the match-function through
    /// <see cref="NumberOfMatchCalls"/>.
    /// 
    /// It would be better to use a Mocking-Framework for this.
    /// </summary>
    [Serializable]
    public class MockTestFilter : TestFilter
    {
        public enum MatchFunction
        {
            Match,
            IsExplicitMatch,
            Pass
        }

        /// <summary>
        /// The test, for which the match-function is allowed to be called.
        /// </summary>
        private readonly ITest _expectedTest;

        /// <summary>
        /// The one and only match-function, which is allowed to be called.
        /// </summary>
        private readonly MatchFunction _expectedFunctionToBeCalled;

        /// <summary>
        /// The result of the match-function <see cref="_expectedFunctionToBeCalled"/>
        /// for the teste <see cref="_expectedTest"/>.
        /// </summary>
        private readonly bool _matchFunctionResult;

        /// <summary>
        /// Gets the number of valid calls of the match-function that have been executed.
        /// </summary>
        public int NumberOfMatchCalls { get; private set; }

        /// <summary>
        /// Construct an <see cref="MockTestFilter"/>.
        /// </summary>
        /// <param name="expectedTest">The test for which calling the match-function is allowed.</param>
        /// <param name="expectedMatchFunction">The matching function that should be called.</param>
        /// <param name="matchFunctionResult">The result of the match function for the expected
        /// test.</param>
        public MockTestFilter(
            ITest expectedTest, MatchFunction expectedMatchFunction, bool matchFunctionResult) : base()
        {
            _expectedTest = expectedTest;
            _expectedFunctionToBeCalled = expectedMatchFunction;
            _matchFunctionResult = matchFunctionResult;
            NumberOfMatchCalls = 0;
        }

        public override bool Match(ITest test)
        {
            return AssertAndGetEquality(test, MatchFunction.Match);
        }

        public override bool IsExplicitMatch(ITest test)
        {
            return AssertAndGetEquality(test, MatchFunction.IsExplicitMatch);
        }

        public override bool Pass(ITest test)
        {
            return AssertAndGetEquality(test, MatchFunction.Pass);
        }

        private bool AssertAndGetEquality(ITest test, MatchFunction calledFunction)
        {
            Assert.AreSame(_expectedTest, test);
            Assert.AreEqual(_expectedFunctionToBeCalled, calledFunction);
            NumberOfMatchCalls += 1;
            return _matchFunctionResult;
        }

        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            throw new NotImplementedException();
        }
    }
}
