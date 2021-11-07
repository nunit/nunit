// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
        /// for the test <see cref="_expectedTest"/>.
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

        public override bool Pass(ITest test, bool negated)
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
