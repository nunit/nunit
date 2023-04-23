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
using System.Linq;

namespace NUnit.Framework.Attributes
{
    [TestFixtureSource(nameof(TestFixturesSource))]
    public class TestCaseSourceWithTestFixtureArgumentsTests
    {
        private static string[] FixtureArgumentsSource { get; }
        private const int NUMBER_OF_ARGS = 3;

        static TestCaseSourceWithTestFixtureArgumentsTests()
        {
            string uniqueIdentifier = Guid.NewGuid().ToString("N");

            FixtureArgumentsSource = new string[NUMBER_OF_ARGS];

            for (int i = 0; i < NUMBER_OF_ARGS; i++)
            {
                FixtureArgumentsSource[i] = $"ARG_{i}_{uniqueIdentifier}";
            }
        }

        public static IEnumerable<TestFixtureData> TestFixturesSource
        {
            get
            {
                for (int maxArgs = 0; maxArgs <= NUMBER_OF_ARGS; maxArgs++)
                {
                    string[] args = FixtureArgumentsSource.Take(maxArgs).ToArray();

                    yield return new TestFixtureData(maxArgs, args).SetArgDisplayNames($"{maxArgs}_arguments_test");

                }
            }
        }

        /// <summary>
        /// The actual arguments used to create the fixture.
        /// </summary>
        private string[] FixtureArgs { get; } 
        private int FixtureNumberOfArgs { get; }

        public TestCaseSourceWithTestFixtureArgumentsTests(int numberOfArgs, string[] args)
        {
            FixtureNumberOfArgs = numberOfArgs;
            FixtureArgs = args;
        }



        /// <summary>
        /// Checks that argument from fixture can
        /// be passed to 
        /// </summary>
        [Test]
        [TestCaseSource(nameof(TestCaseDataWithFixtureContext), new object[]{ TestFixtureArgumentRef.Arg0 })]
        public void FixtureArgumentTest(int argumentIndex, int numberOfArguments)
        {
            Assert.AreEqual(FixtureNumberOfArgs, numberOfArguments);
            Assert.AreEqual(FixtureArgs[argumentIndex], FixtureArgumentsSource[argumentIndex]);
        }

        public static IEnumerable<TestCaseData> TestCaseDataWithFixtureContext(int numberOfArgsFixtureWasCreatedWith)
        {
            for (int i = 0; i < numberOfArgsFixtureWasCreatedWith; i++)
            {
                yield return new TestCaseData(i, numberOfArgsFixtureWasCreatedWith)
                    .SetArgDisplayNames($"Number of args : {i + 1} of {numberOfArgsFixtureWasCreatedWith} ");
            }
        }



        /// <summary>
        /// Checks that you can specify index as integer
        /// and cast it to the enum and the correct
        /// argument will be passed to the test case source method.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(TestCaseDataWithFixtureContext2), new object[] { (TestFixtureArgumentRef) 1 })]
        public void FixtureArgumentAsIntegerTest(string argument, int index)
        {
            Assert.AreEqual(FixtureArgs[index], argument);
        }

        public static IEnumerable<TestCaseData> TestCaseDataWithFixtureContext2(string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                yield return new TestCaseData(data[i], i)
                    .SetArgDisplayNames($"Argument {i + 1} of {data.Length} validation.");
            }
        }


        /// <summary>
        /// Checks that integers are not
        /// treated as parameters index.
        /// </summary>
        [Test]
        [TestCaseSource(nameof(PassThroughMethod), new object[]{ 1, 0 })]
        public void IntegerSourceArgumentsStillWorkAsExpected(int a, int b)
        {
            Assert.AreEqual(1, a);
            Assert.AreEqual(0, b);
        }


        public static IEnumerable<TestCaseData> PassThroughMethod(int a, int b)
        {
            yield return new TestCaseData(a, b);
        }

    }
}
