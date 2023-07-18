// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Tests.Assertions
{
    /// <summary>
    /// This class is for testing issue #1301 where ToString() is called on
    /// a class to create the description of the constraint even where that
    /// description is not used because the test passes.
    /// </summary>
    internal sealed class ThrowsIfToStringIsCalled
    {
        private readonly int _x;

        public ThrowsIfToStringIsCalled(int x)
        {
            _x = x;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ThrowsIfToStringIsCalled other)
                return false;

            return _x == other._x;
        }

        public override int GetHashCode()
        {
            return _x;
        }

        public override string ToString()
        {
            Assert.Fail("Should not call ToString() if Assert does not fail");
            return base.ToString()!;
        }
    }
}
