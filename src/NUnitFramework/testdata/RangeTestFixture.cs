// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.TestData
{
    public static class RangeTestFixture
    {
        [Test]
        public static void MethodWithMultipleRanges([Range(1, 3)] [Range(10, 12)] int x) { }
    }
}
