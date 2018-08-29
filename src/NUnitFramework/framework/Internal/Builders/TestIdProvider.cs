using System.Threading;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// All members are thread-safe.
    /// </summary>
    public sealed class TestIdProvider
    {
        private readonly string _prefix;
        private long _nextNumber;

        public TestIdProvider(string prefix, long startNumber)
        {
            _prefix = prefix;
            _nextNumber = unchecked(startNumber - 1);
        }

        public string CreateId()
        {
            return _prefix + Interlocked.Increment(ref _nextNumber);
        }
    }
}
