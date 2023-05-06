// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters {
    public class DeMorganOnFiltersTest : TestFilterTests
    {
        private static readonly List<TestFilter[]> FilterPairs;

        static DeMorganOnFiltersTest()
        {
            var filterParts = new TestFilter[]
            {
                new CategoryFilter("Dummy"),
                new MethodNameFilter("Test1"),
                new PropertyFilter("Priority", "High"),
                new PropertyFilter("Priority", "Low")
            };

            FilterPairs = new List<TestFilter[]>();
            foreach (var part1 in filterParts)
            foreach (var part2 in filterParts)
            {
                var and = new AndFilter(part1, new NotFilter(part2));
                var or = new OrFilter(new NotFilter(part1), part2);
                FilterPairs.Add(new TestFilter[2] {new NotFilter(and), or});
                FilterPairs.Add(new TestFilter[2] {and, new NotFilter(or)});
            }
        }

        private IEnumerable<ITest> GetTests()
        {
            var q = new Queue<ITest>();
            q.Enqueue(TopLevelSuite);
            while (q.Count > 0)
            {
                var test = q.Dequeue();
                foreach (var child in test.Tests)
                    q.Enqueue(child);
                yield return test;
            }
        }

        private string CaseErrorMessage(string method, TestFilter andFilter, TestFilter orFilter)
        {
            return $"Tests on which the {method} methods of the following filters fails to agree:\n{andFilter.ToXml(true).OuterXml}\n{orFilter.ToXml(true).OuterXml}";
        }

        [Test]
        [TestCaseSource(nameof(FilterPairs))]
        public void DeMorganPassTests(TestFilter andFilter, TestFilter orFilter)
        {

            var disagreements = new List<string>();
            foreach (var test in GetTests())
            {
                if (andFilter.Pass(test) != orFilter.Pass(test))
                    disagreements.Add(test.FullName);
            }
            var message = CaseErrorMessage("Pass", andFilter, orFilter);
            Assert.That(disagreements, Is.Empty, message);
        }

        [Test]
        [TestCaseSource(nameof(FilterPairs))]
        public void DeMorganMatchTests(TestFilter andFilter, TestFilter orFilter)
        {
            var disagreements = new List<string>();
            foreach (var test in GetTests())
            {
                if (andFilter.Match(test) != orFilter.Match(test))
                    disagreements.Add(test.FullName);
            }
            var message = CaseErrorMessage("Match", andFilter, orFilter);
            Assert.That(disagreements, Is.Empty, message);
        }
    }
}
