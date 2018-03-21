namespace NUnit.Framework.Crosscutting
{
    public sealed class CharComparisons : CrosscuttingComparisonTests
    {
        [Test]
        public void EqualChars()
        {
            AssertEquality((char)4, (char)4);
        }

        [Test]
        public void CharEqualToInt()
        {
            AssertEquality((char)4, 4);
        }

        [Test]
        public void CharGreaterThanInt()
        {
            AssertFirstIsGreater((char)4, 2);
        }

        [Test]
        public void IntGreaterThanChar()
        {
            AssertFirstIsGreater(4, (char)2);
        }
    }
}
