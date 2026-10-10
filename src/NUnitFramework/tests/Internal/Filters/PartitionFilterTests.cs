// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Extensions;
using NUnit.Framework.Internal.Filters;
using NUnit.Framework.Tests.TestUtilities;
using NUnit.TestData.Filters;

namespace NUnit.Framework.Tests.Internal.Filters
{
    [TestFixtureSource(nameof(GetPartitionFilterTestCases))]
    internal class PartitionFilterTests : TestFilterTests
    {
        private static IEnumerable<object[]> GetPartitionFilterTestCases()
        {
            var fixtureWithMultipleTestsSuite = ApplyDummyParentSuite(TestBuilder.MakeFixture<FixtureWithMultipleTests>());
            var fixtureWithLongTestCaseNamesSuite = ApplyDummyParentSuite(TestBuilder.MakeFixture<FixtureWithLongNames_JestlaquissemperlectusMaurisetligulafringillaiaculisnislsagittistemporliberoSedinterdummagnasitametfeugiatullamcorperlectusjustosollicitudinmagnaasollicitudinmagnaaugueveljustoDonecfacilisisinmassanecmollisVivamussitametnullaultriciesaliquammaurisegetgravidarisusDonecleonunccongueeuelementumsedconvallisatortorClassaptenttacitisociosquadlitoratorquentperconubianostraperinceptoshimenaeosSedidipsumnisiEtiameleifendmassavitaetortordauctoraefficixtsap>());

            yield return new object[]
            {
                new TestPartitionFilter(9, 10),
                fixtureWithMultipleTestsSuite.Tests[0],
                fixtureWithMultipleTestsSuite.Tests[1],
                fixtureWithLongTestCaseNamesSuite.Tests
            };

            fixtureWithMultipleTestsSuite = ApplyDummyParentSuite(TestBuilder.MakeFixture<FixtureWithMultipleTests>());
            fixtureWithLongTestCaseNamesSuite = ApplyDummyParentSuite(TestBuilder.MakeFixture<FixtureWithLongNames_JestlaquissemperlectusMaurisetligulafringillaiaculisnislsagittistemporliberoSedinterdummagnasitametfeugiatullamcorperlectusjustosollicitudinmagnaasollicitudinmagnaaugueveljustoDonecfacilisisinmassanecmollisVivamussitametnullaultriciesaliquammaurisegetgravidarisusDonecleonunccongueeuelementumsedconvallisatortorClassaptenttacitisociosquadlitoratorquentperconubianostraperinceptoshimenaeosSedidipsumnisiEtiameleifendmassavitaetortordauctoraefficixtsap>());

            // Set "matched" and "not matched" partitions to same parent to check descendant matching works correctly
            var dummyFixtureSuite = TestBuilder.MakeFixture<AnotherFixture>();
            ((TestSuite)fixtureWithMultipleTestsSuite.Parent!).Add(dummyFixtureSuite);

            yield return new object[]
            {
                new FixturePartitionFilter(3, 10),
                fixtureWithMultipleTestsSuite,
                dummyFixtureSuite,
                new ITest[] { fixtureWithLongTestCaseNamesSuite }
            };

            static ITest ApplyDummyParentSuite(ITest child)
            {
                if (child is Test test)
                {
                    if (test.Parent is not TestSuite suite)
                    {
                        test.Parent = suite = new TestSuite("MySuite");
                    }

                    suite.Add(test);
                }

                return child;
            }
        }

        private readonly PartitionFilter _filter;
        private readonly ITest _testMatchingPartition;
        private readonly ITest _testNotMatchingPartition;
        private readonly IList<ITest> _testsWithLongNames;

        public PartitionFilterTests(PartitionFilter filter, ITest testMatchingPartition, ITest testNotMatchingPartition, IList<ITest> testsWithLongNames)
        {
            _filter = filter;
            _testMatchingPartition = testMatchingPartition;
            _testNotMatchingPartition = testNotMatchingPartition;
            _testsWithLongNames = testsWithLongNames;
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
            Assert.That(_filter.ComputePartitionNumber(_testMatchingPartition), Is.EqualTo(_filter.PartitionNumber));
            Assert.That(_filter.ComputePartitionNumber(_testNotMatchingPartition), Is.Not.EqualTo(_filter.PartitionNumber));

            // Assert
            Assert.That(_filter.Match(_testMatchingPartition), Is.True);
            Assert.That(_filter.Match(_testNotMatchingPartition), Is.False);
        }

        [Test]
        public void PassTest()
        {
            // Validate that our matching and non-matching tests return the correct Pass result
            Assert.That(_filter.Pass(_testMatchingPartition), Is.True);
            Assert.That(_filter.Pass(_testNotMatchingPartition), Is.False);

            // This test fixture contains both one matching and one non-matching test
            // The fixture should therefore pass as True because one of the child tests are a match
            Assert.That(_filter.Pass(_testMatchingPartition.Parent!), Is.True);

            // This other test fixture has no matching tests for this partition number
            Assert.That(_filter.Pass(SpecialFixtureSuite), Is.False);
        }

        [Test]
        public void ExplicitMatchTest()
        {
            // Assert
            Assert.That(_filter.IsExplicitMatch(_testMatchingPartition), Is.True);
            Assert.That(_filter.IsExplicitMatch(_testNotMatchingPartition), Is.False);

            // Top level TestFixture should always Pass
            Assert.That(_filter.IsExplicitMatch(_testMatchingPartition.Parent!), Is.True);
        }

        [Test]
        public async Task ComputePartitionNumberThreadSafe()
        {
            var tests = Enumerable.Range(0, 10).Select(i => _testMatchingPartition).ToArray();
            var expected = tests.Select(test => _filter.ComputePartitionNumber(test)).ToArray();

            var tasks = tests.Select(test => Task.Run(() => _filter.ComputePartitionNumber(test))).ToArray();
            await Task.WhenAll(tasks);

            Assert.That(expected, Is.EqualTo(tasks.Select(t => t.Result)));
        }

        [Test]
        public void ComputeParitionNumberHandlesLongTestCaseName()
        {
            foreach (var test in _testsWithLongNames)
            {
                Assert.DoesNotThrow(() => _filter.ComputePartitionNumber(test));
            }
        }
    }

    [TestFixture]
    public static class PartitionFilterParsingTests
    {
        [TestCaseSource(nameof(FromXmlTestCases))]
        public static void FromXml(string xml, TestFilter expected)
        {
            var filter = TestFilter.FromXml($@"<filter>{xml}</filter>");

            Assert.That(filter, Is.TypeOf(expected.GetType()));
            Assert.That(filter, Is.EqualTo(expected).UsingPropertiesComparer());
        }

        private static readonly TestCaseData[] FromXmlTestCases =
        [
            TestCaseData.Create(@"<partition>7/10</partition>", new TestPartitionFilter(7, 10)),
            TestCaseData.Create(@"<partition>7/10:test</partition>", new TestPartitionFilter(7, 10)),
            TestCaseData.Create(@"<partition>7/10:TEST</partition>", new TestPartitionFilter(7, 10)),
            TestCaseData.Create(@"<partition>7/10:fixture</partition>", new FixturePartitionFilter(7, 10)),
            TestCaseData.Create(@"<partition>7/10:Fixture</partition>", new FixturePartitionFilter(7, 10))
        ];

        [TestCaseSource(nameof(ToXmlTestCases))]
        public static string ToXml(TestFilter filter) => filter.ToXml(false).OuterXml;

        private static readonly TestCaseData[] ToXmlTestCases =
        [
            TestCaseData.Create(new TestPartitionFilter(7, 10)).Returns(@"<partition>7/10</partition>"),
            TestCaseData.Create(new FixturePartitionFilter(7, 10)).Returns(@"<partition>7/10:fixture</partition>")
        ];

        [TestCase(@"<partition>7/10</partition>")]
        [TestCase(@"<partition>7/10:fixture</partition>")]
        public static void RoundTripXml(string xml)
        {
            var filter1 = TestFilter.FromXml($@"<filter>{xml}</filter>");
            string xml2 = filter1.ToXml(false).OuterXml;
            var filter2 = TestFilter.FromXml($@"<filter>{xml2}</filter>");

            Assert.That(xml, Is.EqualTo(xml2));
            Assert.That(filter1, Is.TypeOf(filter2.GetType()));
            Assert.That(filter1, Is.EqualTo(filter2).UsingPropertiesComparer());
        }

        [TestCase("1 /1n")]
        [TestCase("1")]
        [TestCase("1/2:")]
        [TestCase("1//2:")]
        [TestCase("1/2:No")]
        [TestCase("1/2::")]
        [TestCase("1/2::fixture")]
        [TestCase("1:2/fixture")]
        [TestCase("1:2:fixture")]
        [TestCase("1:/2:fixture")]
        public static void TryCreateFailure(string input)
        {
            Assert.That(PartitionFilter.TryCreate(input, out _), Is.False);
        }

        [TestCase("1/2")]
        [TestCase(" 1/2")]
        [TestCase("1/2 ")]
        [TestCase("1/2:fixture")]
        [TestCase("1/2:test")]
        public static void TryCreateSuccess(string input)
        {
            var result = PartitionFilter.TryCreate(input, out var filter);

            Assert.That(result, Is.True);
            Assert.That(filter, Is.Not.Null);

            Assert.That(filter.PartitionNumber, Is.EqualTo(1));
            Assert.That(filter.PartitionCount, Is.EqualTo(2));
        }
    }

    [TestFixture]
    public static class PartitionNumberDiscoveryTests
    {
        [TestCase(@"<partition>1/2</partition>", 1u)]
        [TestCase(@"<partition>7/10:test</partition>", 7u)]
        [TestCase(@"<partition>4/5:fixture</partition>", 4u)]
        [TestCase(@"<partition>3000000000/4000000000</partition>", 3000000000u)]
        [TestCase(@"<and><partition>3/9</partition><cat>SomeCategory</cat></and>", 3u)]
        [TestCase(@"<or><partition>2/3</partition><cat>SomeCategory</cat></or>", 2u)]
        [TestCase(@"<or><not><cat>SomeCategory</cat></not><partition>2/3</partition></or>", 2u)]
        [TestCase(@"<or><partition>2/3</partition><partition>2/3</partition></or>", 2u)]
        [TestCase(@"<and><partition>1/2</partition><not><partition>2/3</partition></not></and>", 1u)]
        public static void GetPartitionNumberFindsPartition(string xml, uint expectedPartitionNumber)
        {
            var filter = TestFilter.FromXml($@"<filter>{xml}</filter>");

            Assert.That(filter.GetPartitionNumber(), Is.EqualTo(expectedPartitionNumber));
        }

        [TestCase(@"<cat>SomeCategory</cat>")]
        [TestCase(@"<not><partition>1/2</partition></not>")]
        public static void GetPartitionNumberIsNullWhenItIsNotASinglePartition(string xml)
        {
            var filter = TestFilter.FromXml($@"<filter>{xml}</filter>");

            Assert.That(filter.GetPartitionNumber(), Is.Null);
        }

        [TestCase(@"<or><partition>1/3</partition><partition>2/3</partition></or>", 1u)]
        [TestCase(@"<and><partition>1/3</partition><partition>2/3</partition></and>", 1u)]
        [TestCase(@"<or><partition>1/3</partition><and><partition>2/3</partition><cat>SomeCategory</cat></and></or>", 1u)]
        public static void GetPartitionNumberTakesTheFirstWhenMoreThanOneIsSelected(string xml, uint expectedPartitionNumber)
        {
            var filter = TestFilter.FromXml($@"<filter>{xml}</filter>");

            Assert.That(filter.GetPartitionNumber(), Is.EqualTo(expectedPartitionNumber));
        }
    }
}
