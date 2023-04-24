// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal.Filters
{
    public class AndFilterTests : TestFilterTests
    {
        [Test]
        public void IsNotEmpty()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(_dummyFixture.Id));

            Assert.That(filter.IsEmpty, Is.False);
        }

        [Test]
        public void MatchTest()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(_dummyFixture.Id));

            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture), Is.False);
        }

        [Test]
        public void PassTest()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(_dummyFixture.Id));

            Assert.That(filter.Pass(_topLevelSuite));
            Assert.That(filter.Pass(_dummyFixture));
            Assert.That(filter.Pass(_dummyFixture.Tests[0]));

            Assert.That(filter.Pass(_anotherFixture), Is.False);
        }

        [Test]
        public void ExplicitMatchTest()
        {
            var filter = new AndFilter(new CategoryFilter("Dummy"), new IdFilter(_dummyFixture.Id));

            Assert.That(filter.IsExplicitMatch(_topLevelSuite));
            Assert.That(filter.IsExplicitMatch(_dummyFixture));
            Assert.That(filter.IsExplicitMatch(_dummyFixture.Tests[0]), Is.False);

            Assert.That(filter.Match(_anotherFixture), Is.False);
        }

        [Test]
        public void ExplicitMatchWithNotTest()
        {
            var filter = new AndFilter(
                new NotFilter(
                    new CategoryFilter("Dummy")),
                new NotFilter(
                    new IdFilter(_dummyFixture.Id)));

            Assert.That(filter.IsExplicitMatch(_topLevelSuite), Is.False);
            Assert.That(filter.IsExplicitMatch(_dummyFixture), Is.False);
            Assert.That(filter.IsExplicitMatch(_dummyFixture.Tests[0]), Is.False);

            Assert.That(filter.Match(_anotherFixture), "4");
        }

        /// <summary>
        /// Generic combination tests for the <see cref="AndFilter"/>. This test checks that the
        /// <see cref="AndFilter"/> correctly combines the results from its sub-filters.
        /// Furthermore it checks that the result from the correct match-function of the
        /// sub-filters is used to calculate the AND combination.
        /// 
        /// The input is an array of booleans (<paramref name="inputBooleans"/>). For each boolean
        /// value a <see cref="MockTestFilter"/> is added to the <see cref="AndFilter"/>
        /// whose match-function (defined through the parameter <paramref name="matchFunction"/>)
        /// evaluates to the boolean value indicated.
        /// Afterwards the requested match-function (<paramref name="matchFunction"/>) is called
        /// on the <see cref="AndFilter"/> and is compared to the expected result
        /// (<paramref name="expectedResult"/>).
        /// This tests also throws an exception if the match-function call from the
        /// <see cref="AndFilter"/> calls not the same match-function on the
        /// <see cref="MockTestFilter"/>, thus checking that the <see cref="AndFilter"/>
        /// combines the correct results from the sub-filters.
        /// 
        /// See also <see cref="MockTestFilter"/>.
        /// </summary>
        [TestCase(new[] { false, false}, false, MockTestFilter.MatchFunction.IsExplicitMatch)]
        [TestCase(new[] { true, false }, false, MockTestFilter.MatchFunction.IsExplicitMatch)]
        [TestCase(new[] { false, true }, false, MockTestFilter.MatchFunction.IsExplicitMatch)]
        [TestCase(new[] { true, true }, true, MockTestFilter.MatchFunction.IsExplicitMatch)]
        [TestCase(new[] { false, false }, false, MockTestFilter.MatchFunction.Match)]
        [TestCase(new[] { true, false }, false, MockTestFilter.MatchFunction.Match)]
        [TestCase(new[] { false, true }, false, MockTestFilter.MatchFunction.Match)]
        [TestCase(new[] { true, true }, true, MockTestFilter.MatchFunction.Match)]
        [TestCase(new[] { false, false }, false, MockTestFilter.MatchFunction.Pass)]
        [TestCase(new[] { true, false }, false, MockTestFilter.MatchFunction.Pass)]
        [TestCase(new[] { false, true }, false, MockTestFilter.MatchFunction.Pass)]
        [TestCase(new[] { true, true }, true, MockTestFilter.MatchFunction.Pass)]
        public void CombineTest(IEnumerable<bool> inputBooleans, bool expectedResult,
            MockTestFilter.MatchFunction matchFunction)
        {
            var filters = new List<MockTestFilter>();
            foreach (var inputBool in inputBooleans)
            {
                var strictFilter = new MockTestFilter(_dummyFixture, matchFunction, inputBool);
                Assert.That(ExecuteMatchFunction(strictFilter, matchFunction), Is.EqualTo(inputBool));

                filters.Add(strictFilter);
            }

            var filter = new AndFilter(filters.ToArray());
            bool calculatedResult = ExecuteMatchFunction(filter, matchFunction);
            Assert.That(calculatedResult, Is.EqualTo(expectedResult));
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
                        "Unexpected StrictIdFilterForTests.EqualValueFunction.", nameof(matchFunction));
            }
        }

        [Test]
        public void BuildFromXml()
        {
            TestFilter filter = TestFilter.FromXml(
                $"<filter><and><cat>Dummy</cat><id>{_dummyFixture.Id}</id></and></filter>");

            Assert.That(filter, Is.TypeOf<AndFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture), Is.False);
        }

        [Test]
        public void BuildFromXml_TopLevelDefaultsToAnd()
        {
            TestFilter filter = TestFilter.FromXml(
                $"<filter><cat>Dummy</cat><id>{_dummyFixture.Id}</id></filter>");

            Assert.That(filter, Is.TypeOf<AndFilter>());
            Assert.That(filter.Match(_dummyFixture));
            Assert.That(filter.Match(_anotherFixture), Is.False);
        }
    }
}
