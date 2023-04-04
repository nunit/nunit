// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Attributes;

namespace NUnit.TestData
{
    [TestFixture]
    public class RetryUntilFailureAttributeFixture
    {
        private int _counter = 0;

        [Test, RetryUntilFailure, Ignore("Test should fail when run")]
        public void RunUntilFailure()
        {
            // Act + Assert
            Assert.IsTrue(_counter != 10);
            _counter++;
        }

        [Test, RetryUntilFailure(20), Ignore("Test should fail when run")]
        public void HitFailureBeforeMaxRetries()
        {
            // Act + Assert
            Assert.IsTrue(_counter != 10);
            _counter++;
        }

        [Test, RetryUntilFailure(5)]
        public void HitMaxRetriesBeforeFailure()
        {
            // Act + Assert
            Assert.IsTrue(_counter != 10);
            _counter++;
        }
    }
}
