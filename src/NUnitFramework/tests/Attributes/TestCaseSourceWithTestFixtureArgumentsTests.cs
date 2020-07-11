// ***********************************************************************
// Copyright (c) 2020 Charlie Poole, Lukasz Skomial
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

using System;
using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Attributes
{
    [TestFixtureSource(nameof(Fixtures))]
    public class TestCaseSourceWithTestFixtureArgumentsTests
    {
        private const string FIXTURE_ARG1 = "fixture_arg_1";
        private const string FIXTURE_ARG2 = "fixture_arg_2";
        private const string FIXTURE_ARG3 = "fixture_arg_3";

        private string[] FixtureArgs { get; } 
        
        public TestCaseSourceWithTestFixtureArgumentsTests(params string[] args)
        {
            FixtureArgs = args;
        }

        
        [TestCaseSource(nameof(TestCaseDataWithFixtureContext), 
            new object?[]{ FIXTURE_ARG1, TestFixtureArgumentPlaceholder.Arg0 })]
        [TestCaseSource(nameof(TestCaseDataWithFixtureContext),
            new object?[] { FIXTURE_ARG3, TestFixtureArgumentPlaceholder.Arg2 })]
        [TestCaseSource(nameof(TestCaseDataWithFixtureContext),
            new object?[] { "11", (TestFixtureArgumentPlaceholder) 10 })]
        [TestCaseSource(nameof(TestCaseDataWithFixtureContext),
            new object?[] { "failure...", (TestFixtureArgumentPlaceholder) 1000})]
        public void TestFixtureArgument(string expectedValue, string actualValue)
        {

        }



        
        [TestCaseSource(nameof(TestCaseDataWithFixtureContextNumeric),
            new object?[] { 0, 0 })]
        [TestCaseSource(nameof(TestCaseDataWithFixtureContextNumeric),
            new object?[] { 10, 10 })]
        public void TestFixtureArgument2(int expectedValue, int actualValue)
        {

        }

        [Test]
        public void DummyTest()
        {

        }


        public static IEnumerable<TestCaseData> TestCaseDataWithFixtureContextNumeric(int expectedValue, int actualValue)
        {
            yield return new TestCaseData(expectedValue, actualValue);
        }

        public static IEnumerable<TestCaseData> TestCaseDataWithFixtureContext(string expectedValue, string actualValue)
        {
            yield return new TestCaseData(expectedValue, actualValue);
        }


        public static IEnumerable<TestFixtureData> Fixtures
        {
            get
            {
                //yield return new TestFixtureData(FIXTURE_ARG1, FIXTURE_ARG2, FIXTURE_ARG3);
                yield return new TestFixtureData(FIXTURE_ARG1, FIXTURE_ARG2, FIXTURE_ARG3, "4", "5", "6", "7", "8", "9", "10", "11");
            }
        }
    }
}
