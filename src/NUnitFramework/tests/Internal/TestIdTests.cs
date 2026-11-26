// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Threading.Tasks;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    public class TestIdTests
    {
        [Test]
        public void IdsAreUniqueWhenTestsAreCreatedInParallel()
        {
            var ids = new string[100_000];

            Parallel.For(0, ids.Length, i => ids[i] = new TestSuite("Suite").Id);

            Assert.That(ids, Is.Unique);
        }
    }
}
