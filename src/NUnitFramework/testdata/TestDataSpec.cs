// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;

namespace NUnit.TestData
{
    public struct TestDataSpec
    {
        public static TestDataSpec[] Specs => new[]
        {
            new TestDataSpec(new object[] { "val1", "val2" }, null, "(\"val1\",\"val2\")"),
            new TestDataSpec(new object[] { "val1", "val2" }, Array.Empty<string>(), "()"),
            new TestDataSpec(new object[] { "val1", "val2" }, new[] { "display1" }, "(display1)"),
            new TestDataSpec(new object[] { "val1" }, new[] { "display1", "display2" }, "(display1, display2)"),
            new TestDataSpec(new object[] { }, null, "()"),
            new TestDataSpec(new object[] { }, Array.Empty<string>(), "()"),
            new TestDataSpec(new object[] { }, new[] { "display1" }, "(display1)"),
            new TestDataSpec(new object[] { }, new[] { ",", " ", "", null, "\r\n" }, "(,,  , , , \r\n)"),
        };

        public object[] Arguments { get; }
        public string[] ArgDisplayNames { get; }
        private readonly string _expectedArgumentListDisplay;

        public TestDataSpec(object[] arguments, string[] argDisplayNames, string expectedArgumentListDisplay)
        {
            Arguments = arguments;
            ArgDisplayNames = argDisplayNames;
            _expectedArgumentListDisplay = expectedArgumentListDisplay;
        }

        public string GetTestCaseName(string methodName)
        {
            return methodName + _expectedArgumentListDisplay;
        }

        public string GetFixtureName(string fixtureName)
        {
            return _expectedArgumentListDisplay == "()" ? fixtureName :
                fixtureName + _expectedArgumentListDisplay;
        }
    }
}
