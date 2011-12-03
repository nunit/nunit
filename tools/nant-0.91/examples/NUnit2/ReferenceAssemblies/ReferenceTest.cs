using NUnit.Framework;
using Helpers;

namespace NAnt.NUnit2.Tests {
    [TestFixture]
    public class ReferenceTests {
        [Test]
        public void LogTest () {
            Log.Debug ("whatever");
        }
    }
}
