// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class UniqueTestIdTests
    {
        private readonly ConcurrentDictionary<string, string> _idHolder = new ConcurrentDictionary<string, string>();
        private static string ID => TestContext.CurrentContext.Test.ID;

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void TestCase(int testCase)
        {
            Assert.That(_idHolder, Does.Not.ContainKey(ID));
            _idHolder.AddOrUpdate(ID, ID, (s, s1) => ID);
        }

        [TestCaseSource(nameof(SomeStrings))]
        public void TestCaseSource(string testCaseSourceData)
        {
            Assert.That(_idHolder, Does.Not.ContainKey(ID));
            _idHolder.AddOrUpdate(ID, ID, (s, s1) => ID);
        }

        [Repeat(5)]
        [Test]
        public void Repeat()
        {
            Assert.That(_idHolder, Does.Not.ContainKey(ID));
            _idHolder.AddOrUpdate(ID, ID, (s, s1) => ID);
        }

        public static IEnumerable<string> SomeStrings => new List<string>
        {
            "1", "2", "3", "4"
        };
    }
}
