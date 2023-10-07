// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;

namespace NUnit.TestData
{
    public class TextOutputFixture
    {
        private const string ERROR_TEXT = "Written directly to console";

        [Test]
        public void ConsoleErrorWrite()
        {
            Console.Error.Write(ERROR_TEXT);
        }

        [Test]
        public void ConsoleErrorWriteLine()
        {
            Console.Error.WriteLine(ERROR_TEXT);
        }

        [Test]
        public void TestContextErrorWriteLine()
        {
            TestContext.Error.WriteLine(ERROR_TEXT);
        }

        [Test]
        public void TestContextProgressWriteLine()
        {
            TestContext.Progress.WriteLine(ERROR_TEXT);
        }
    }
}
