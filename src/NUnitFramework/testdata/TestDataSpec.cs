// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

namespace NUnit.TestData
{
    public struct TestDataSpec
    {
        public static TestDataSpec[] Specs => new[]
        {
            new TestDataSpec(new object[] { "val1", "val2" }, null, "(\"val1\",\"val2\")"),
            new TestDataSpec(new object[] { "val1", "val2" }, new string[0], "()"),
            new TestDataSpec(new object[] { "val1", "val2" }, new[] { "display1" }, "(display1)"),
            new TestDataSpec(new object[] { "val1" }, new[] { "display1", "display2" }, "(display1, display2)"),
            new TestDataSpec(new object[] { }, null, "()"),
            new TestDataSpec(new object[] { }, new string[0], "()"),
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
