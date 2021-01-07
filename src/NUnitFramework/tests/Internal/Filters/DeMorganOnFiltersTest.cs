// ***********************************************************************
// Copyright (c) 2019 Charlie Poole, Rob Prouse
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

using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Filters {
    public class DeMorganOnFiltersTest : TestFilterTests
    {
        private static List<TestFilter[]> filterPairs;

        static DeMorganOnFiltersTest()
        {
            var filterParts = new TestFilter[]
            {
                new CategoryFilter("Dummy"),
                new MethodNameFilter("Test1"),
                new PropertyFilter("Priority", "High"),
                new PropertyFilter("Priority", "Low")
            };

            filterPairs = new List<TestFilter[]>();
            foreach (var part1 in filterParts)
            foreach (var part2 in filterParts)
            {
                var and = new AndFilter(part1, new NotFilter(part2));
                var or = new OrFilter(new NotFilter(part1), part2);
                filterPairs.Add(new TestFilter[2] {new NotFilter(and), or});
                filterPairs.Add(new TestFilter[2] {and, new NotFilter(or)});
            }
        }

        private IEnumerable<ITest> GetTests()
        {
            var q = new Queue<ITest>();
            q.Enqueue(_topLevelSuite);
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
        [TestCaseSource(nameof(filterPairs))]
        public void DeMorganPassTests(TestFilter andFilter, TestFilter orFilter)
        {

            var disagreements = new List<string>();
            foreach (var test in GetTests())
            {
                if (andFilter.Pass(test) != orFilter.Pass(test))
                    disagreements.Add(test.FullName);
            }
            var message = CaseErrorMessage("Pass", andFilter, orFilter);
            Assert.IsEmpty(disagreements, message);
        }

        [Test]
        [TestCaseSource(nameof(filterPairs))]
        public void DeMorganMatchTests(TestFilter andFilter, TestFilter orFilter)
        {
            var disagreements = new List<string>();
            foreach (var test in GetTests())
            {
                if (andFilter.Match(test) != orFilter.Match(test))
                    disagreements.Add(test.FullName);
            }
            var message = CaseErrorMessage("Match", andFilter, orFilter);
            Assert.IsEmpty(disagreements, message);
        }
    }
}
