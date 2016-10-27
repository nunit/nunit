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
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class OrFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.False(filter.IsEmpty);
        }

        [Test]
        public void MatchTest()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));

            Assert.False(filter.Match(_yetAnotherFixture));
        }

        [Test]
        public void PassTest()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));
            Assert.That(filter.Pass(_anotherFixture));
            Assert.That(filter.Pass(_anotherFixture.Tests[0]));

            Assert.False(filter.Pass(_yetAnotherFixture));
        }

        [Test]
        public void ExplicitMatchTest()
        {
            var filter = new OrFilter(new CategoryFilter("Dummy"), new CategoryFilter("Another"));

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.False(filter.IsExplicitMatch(_dummyFixture.Tests[0]));
            Assert.That(filter.IsExplicitMatch(_anotherFixture));
            Assert.False(filter.IsExplicitMatch(_anotherFixture.Tests[0]));

            Assert.False(filter.IsExplicitMatch(_yetAnotherFixture));
        }

        /// <summary>
        /// Generic combination tests for the <see cref="OrFilter"/>. This test checks that the
        /// <see cref="OrFilter"/> correctly combines the results from its sub-filters.
        /// Furthermore it checks that the result from the correct match-function of the
        /// sub-filters is used to calculate the OR combination.
        /// 
        /// The input is an array of booleans (<paramref name="inputBooleans"/>). For each boolean
        /// value a <see cref="MockTestFilter"/> is added to the <see cref="OrFilter"/>
        /// whose match-function (defined through the paramter <paramref name="matchFunction"/>)
        /// evaluates to the boolean value indicated.
        /// Afterwrards the requested match-function (<paramref name="matchFunction"/>) is called
        /// on the <see cref="OrFilter"/> and is compared to the expected result
        /// (<paramref name="expectedResult"/>).
        /// This tests also throws an exception if the match-function call from the
        /// <see cref="OrFilter"/> calls not the same match-function on the
        /// <see cref="MockTestFilter"/>, thus checking that the <see cref="OrFilter"/>
        /// combines the correct results from the sub-filters.
        /// 
        /// See also <see cref="MockTestFilter"/>.
        /// </summary>
        [TestCase(new bool[] { false, false }, false, MockTestFilter.MatchFunction.IsExplicitMatch)]
        [TestCase(new bool[] { true, false }, true, MockTestFilter.MatchFunction.IsExplicitMatch)]
        [TestCase(new bool[] { false, true }, true, MockTestFilter.MatchFunction.IsExplicitMatch)]
        [TestCase(new bool[] { true, true }, true, MockTestFilter.MatchFunction.IsExplicitMatch)]
        [TestCase(new bool[] { false, false }, false, MockTestFilter.MatchFunction.Match)]
        [TestCase(new bool[] { true, false }, true, MockTestFilter.MatchFunction.Match)]
        [TestCase(new bool[] { false, true }, true, MockTestFilter.MatchFunction.Match)]
        [TestCase(new bool[] { true, true }, true, MockTestFilter.MatchFunction.Match)]
        [TestCase(new bool[] { false, false }, false, MockTestFilter.MatchFunction.Pass)]
        [TestCase(new bool[] { true, false }, true, MockTestFilter.MatchFunction.Pass)]
        [TestCase(new bool[] { false, true }, true, MockTestFilter.MatchFunction.Pass)]
        [TestCase(new bool[] { true, true }, true, MockTestFilter.MatchFunction.Pass)]
        public void CombineTest(IEnumerable<bool> inputBooleans, bool expectedResult,
            MockTestFilter.MatchFunction matchFunction)
        {
            var filters = new List<MockTestFilter>();
            foreach (var inputBool in inputBooleans)
            {
                var strictFilter = new MockTestFilter(_dummyFixture, matchFunction, inputBool);
                Assert.AreEqual(inputBool, ExecuteMatchFunction(strictFilter, matchFunction));

                filters.Add(strictFilter);
            }

            var filter = new OrFilter(filters.ToArray());
            bool calculatedResult = ExecuteMatchFunction(filter, matchFunction);
            Assert.AreEqual(expectedResult, calculatedResult);
        }

        /// <summary>
        /// Executes the requested <paramref name="matchFunction"/> on the <paramref name="filter"/>.
        /// </summary>
        private bool ExecuteMatchFunction(
            TestFilter filter, MockTestFilter.MatchFunction matchFunction)
        {
            switch (matchFunction)
            {
                case MockTestFilter.MatchFunction.IsExplicitMatch:
                    return filter.IsExplicitMatch(_dummyFixture);
                case MockTestFilter.MatchFunction.Match:
                    return filter.Match(_dummyFixture);
                case MockTestFilter.MatchFunction.Pass:
                    return filter.Pass(_dummyFixture);
                default:
                    throw new ArgumentException(
                        "Unexpected StrictIdFilterForTests.EqualValueFunction.", "matchFunction");
            }
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                "<filter><or><cat>Dummy</cat><cat>Another</cat></or></filter>");

            Assert.That(filter, Is.TypeOf<OrFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture));
        }
    }
}
