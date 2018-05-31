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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework.Internal;
using NUnit.Tests;
using NUnit.Tests.Assemblies;
using NUnit.Tests.Singletons;
using NUnit.Compatibility;
using NUnit.TestData;

namespace NUnit.Framework.Api
{
    [TestFixture] // [TestFixture] is needed so that ClassesCanBeImpliedToBeFixtures is effective.
    public static class ImpliedFixtureTests
    {
        [Test]
        public static void ClassesCanBeImpliedToBeFixtures()
        {
            var assemblyTest = new DefaultTestAssemblyBuilder().Build(
                typeof(ImpliedFixture).GetTypeInfo().Assembly,
                options: new Dictionary<string, object>());

            Assert.That(
                new[] { assemblyTest }.SelectManyRecursive(test => test.Tests),
                Has.Some.With.Property("Type").EqualTo(typeof(ImpliedFixture)));
        }
    }

    public class DefaultTestAssemblyBuilderTests
    {
        private const string MOCK_ASSEMBLY_FILE = "mock-assembly.dll";

        private string _mockAssemblyPath;
        private DefaultTestAssemblyBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _mockAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, MOCK_ASSEMBLY_FILE);
            _builder = new DefaultTestAssemblyBuilder();
        }

        [TestCase(ExpectedResult = MockAssembly.Tests)]
        [TestCase("NUnit", ExpectedResult = MockAssembly.Tests)]
        [TestCase("NUnit.Tests", ExpectedResult = MockAssembly.Tests)]
        [TestCase("NUnit.Tests.Assemblies", ExpectedResult = MockTestFixture.Tests)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture", ExpectedResult = MockTestFixture.Tests)]
        [TestCase("NUnit.Tests.FixtureWithTestCases", ExpectedResult = FixtureWithTestCases.Tests)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture", "NUnit.Tests.FixtureWithTestCases", ExpectedResult = MockTestFixture.Tests + FixtureWithTestCases.Tests)]
        [TestCase("NUnit.Tests.Singletons", ExpectedResult = OneTestCase.Tests)]
        [TestCase("NUnit.Tests.Singletons.OneTestCase", ExpectedResult = OneTestCase.Tests)]
        [TestCase("NUnit.Tests.Assemblies", "NUnit.Tests.Singletons", ExpectedResult = MockTestFixture.Tests + OneTestCase.Tests)]
        [TestCase("NUnit.Tests.Assemblies", "NUnit.Tests.Singletons", "NUnit.Tests.FixtureWithTestCases", ExpectedResult = MockTestFixture.Tests + FixtureWithTestCases.Tests + OneTestCase.Tests)]
        [TestCase("NUnit.Tests.Assemblies.MockTestFixture", "NUnit.Tests.Assemblies", ExpectedResult = MockTestFixture.Tests)]
        public int LoadWithSinglePreFilter(params string[] filters)
        {
            var settings = new Dictionary<string, object>();
            settings.Add("LOAD", filters);
            var result = _builder.Build(_mockAssemblyPath, settings);

            Assert.That(result.IsSuite);
            Assert.That(result, Is.TypeOf<TestAssembly>());
            Assert.That(result.Name, Is.EqualTo(MOCK_ASSEMBLY_FILE));
            Assert.That(result.RunState, Is.EqualTo(Interfaces.RunState.Runnable));

            return result.TestCaseCount;
        }
    }
}
