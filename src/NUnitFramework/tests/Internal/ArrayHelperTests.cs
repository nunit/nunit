// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class ArrayHelperTests
    {
        [Test]
        public void EmptyCachesReturnedResults()
        {
            var first = ArrayHelper.Empty<object>();
            var second = ArrayHelper.Empty<object>();

            Assert.That(first, Is.SameAs(second));
        }
    }
}
