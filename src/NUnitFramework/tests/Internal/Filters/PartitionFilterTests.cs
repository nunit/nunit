// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Filters;

namespace NUnit.Framework.Tests.Internal.Filters
{
    public class PartitionFilterTests : TestFilterTests
    {
        private PartitionFilter _filter;
        private ITest _testMatchingPartition;
        private ITest _testNotMatchingPartition;
        private ITest _testFixtureWithWithLongTestCaseNames;

        [SetUp]
        public void CreateFilter()
        {
            // Configure a new PartitionFilter with the provided partition count and number
            _filter = new PartitionFilter(6, 10);

            _testMatchingPartition = FixtureWithMultipleTestsSuite.Tests[0];
            _testNotMatchingPartition = FixtureWithMultipleTestsSuite.Tests[1];
            _testFixtureWithWithLongTestCaseNames = FixtureWithLongTestCaseNamesSuite.Tests[0];
        }

        [Test]
        public void IsNotEmpty()
        {
            Assert.That(_filter.IsEmpty, Is.False);
        }

        [Test]
        public void MatchTest()
        {
            // Validate
            Assert.That(_filter.ComputePartitionNumber(_testMatchingPartition), Is.EqualTo(6));
            Assert.That(_filter.ComputePartitionNumber(_testNotMatchingPartition), Is.EqualTo(9));

            // Assert
            Assert.That(_filter.Match(_testMatchingPartition), Is.True);
            Assert.That(_filter.Match(_testNotMatchingPartition), Is.False);
        }

        [Test]
        public void PassTest()
        {
            // This test fixture contains both one matching and one non-matching test
            // The fixture should therefore pass as True because one of the child tests are a match
            Assert.That(_filter.Pass(FixtureWithMultipleTestsSuite), Is.True);

            // Validate that our matching and non-matching tests return the correct Pass result
            Assert.That(_filter.Pass(_testMatchingPartition), Is.True);
            Assert.That(_filter.Pass(_testNotMatchingPartition), Is.False);

            // This other test fixture has no matching tests for this partition number
            Assert.That(_filter.Pass(SpecialFixtureSuite), Is.False);
        }

        [Test]
        public void ExplicitMatchTest()
        {
            // Top level TestFixture should always Pass
            Assert.That(_filter.IsExplicitMatch(FixtureWithMultipleTestsSuite));

            // Assert
            Assert.That(_filter.IsExplicitMatch(_testMatchingPartition), Is.True);
            Assert.That(_filter.IsExplicitMatch(_testNotMatchingPartition), Is.False);
        }

        [Test]
        public void FromXml()
        {
            TestFilter filter = TestFilter.FromXml(@"<filter><partition>7/10</partition></filter>");

            Assert.That(filter, Is.TypeOf<PartitionFilter>());

            var partitionFilter = (PartitionFilter)filter;
            Assert.That(partitionFilter.PartitionNumber, Is.EqualTo(7));
            Assert.That(partitionFilter.PartitionCount, Is.EqualTo(10));
        }

        [Test]
        public void ToXml()
        {
            Assert.That(_filter.ToXml(false).OuterXml, Is.EqualTo(@"<partition>6/10</partition>"));
        }

        [Test]
        public async Task ComputePartitionNumberThreadSafe()
        {
            var tests = Enumerable.Range(0, 10).Select(i => FixtureWithMultipleTestsSuite.Tests[i % 2]).ToArray();
            var expected = tests.Select(test => _filter.ComputePartitionNumber(test)).ToArray();

            var tasks = tests.Select(test => Task.Run(() => _filter.ComputePartitionNumber(test))).ToArray();
            await Task.WhenAll(tasks);

            Assert.That(expected, Is.EqualTo(tasks.Select(t => t.Result)));
        }

        [Test]
        public void ComputeParitionNumberHandlesLongTestCaseName()
        {
            Assert.DoesNotThrow(() => _filter.ComputePartitionNumber(_testFixtureWithWithLongTestCaseNames.Tests[0]));
            Assert.DoesNotThrow(() => _filter.ComputePartitionNumber(_testFixtureWithWithLongTestCaseNames.Tests[1]));
        }

        [TestCase("1/1n")]
        [TestCase("1")]
        public void TryCreateFailure(string input)
        {
            Assert.That(PartitionFilter.TryCreate(input, out var filter), Is.False);
        }

        [TestCase("1/2")]
        [TestCase(" 1/2")]
        [TestCase("1/2 ")]
        public void TryCreateSuccess(string input)
        {
            var result = PartitionFilter.TryCreate(input, out var filter);

            Assert.That(result, Is.True);
            Assert.That(filter!.PartitionNumber, Is.EqualTo(1));
            Assert.That(filter.PartitionCount, Is.EqualTo(2));
        }
    }
}
