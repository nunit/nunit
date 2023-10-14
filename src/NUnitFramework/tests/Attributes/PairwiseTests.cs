// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class PairwiseTest
    {
        [TestFixture]
        public class LiveTest
        {
            private PairCounter _pairsTested = new PairCounter();

            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
                _pairsTested = new PairCounter();
            }

            [OneTimeTearDown]
            public void OneTimeTearDown()
            {
                Assert.That(_pairsTested, Has.Count.EqualTo(16));
            }

            [Test, Pairwise]
            public void Test(
                [Values("a", "b", "c")] string a,
                [Values("+", "-")] string b,
                [Values("x", "y")] string c)
            {
                //Console.WriteLine("Pairwise: {0} {1} {2}", a, b, c);

                _pairsTested[a + b] = null;
                _pairsTested[a + c] = null;
                _pairsTested[b + c] = null;
            }
        }

        // Test data is taken from various sources. See "Lessons Learned
        // in Software Testing" pp 53-59, for example. For orthogonal cases, see
        // https://web.archive.org/web/20100305233703/www.freequality.org/sites/www_freequality_org/documents/tools/Tagarray_files/tamatrix.htm
        internal static object[] Cases = new object[]
        {
            new TestCaseData(new[] { 2, 4 }, 8, 8).SetName("Test 2x4"),
            new TestCaseData(new[] { 2, 2, 2 }, 4, 4).SetName("Test 2x2x2"),
            new TestCaseData(new[] { 3, 2, 2 }, 6, 6).SetName("Test 3x2x2"),
            new TestCaseData(new[] { 3, 2, 2, 2 }, 6, 6).SetName("Test 3x2x2x2"),
            new TestCaseData(new[] { 3, 2, 2, 2, 2 }, 6, 6).SetName("Test 3x2x2x2x2"),
            new TestCaseData(new[] { 3, 2, 2, 2, 2, 2 }, 8, 8).SetName("Test 3x2x2x2x2x2"),
            new TestCaseData(new[] { 3, 3, 3 }, 9, 9).SetName("Test 3x3x3"),
            new TestCaseData(new[] { 4, 4, 4 }, 17, 16).SetName("Test 4x4x4"),
            new TestCaseData(new[] { 5, 5, 5 }, 25, 25).SetName("Test 5x5x5")
        };

        [Test, TestCaseSource(nameof(Cases))]
        public void Test(int[] dimensions, int bestSoFar, int targetCases)
        {
            int features = dimensions.Length;

            string[][] sources = new string[features][];

            for (int i = 0; i < features; i++)
            {
                string featureName = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(i, 1);

                int n = dimensions[i];
                sources[i] = new string[n];
                for (int j = 0; j < n; j++)
                    sources[i][j] = featureName + j;
            }

            ICombiningStrategy strategy = new PairwiseStrategy();

            PairCounter pairs = new PairCounter();
            int cases = 0;
            foreach (NUnit.Framework.Internal.TestCaseParameters parms in strategy.GetTestCases(sources))
            {
                for (int i = 1; i < features; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        string? a = parms.Arguments[i] as string;
                        string? b = parms.Arguments[j] as string;
                        pairs[a + b] = null;
                    }
                }

                ++cases;
            }

            int expectedPairs = 0;
            for (int i = 1; i < features; i++)
            {
                for (int j = 0; j < i; j++)
                    expectedPairs += dimensions[i] * dimensions[j];
            }

            Assert.That(pairs, Has.Count.EqualTo(expectedPairs), "Number of pairs is incorrect");
            Assert.That(cases, Is.AtMost(bestSoFar), "Regression: Number of test cases exceeded target previously reached");
#if DEBUG
            //Assert.That(cases, Is.AtMost(targetCases), "Number of test cases exceeded target");
#endif
        }

        private class PairCounter : System.Collections.Generic.Dictionary<string, object?>
        {
        }
    }
}
