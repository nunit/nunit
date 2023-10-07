// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestData
{
    public class SendMessageFixture
    {
        private const string DESTINATION = "destination";
        private const string MESSAGE = "message";

        [Test]
        public void TestWithMessage()
        {
            TestExecutionContext.CurrentContext.SendMessage(DESTINATION, MESSAGE);
        }
    }
}
