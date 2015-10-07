// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

using System.IO;

using NUnit.ConsoleRunner.Utilities;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    [TestFixture]
    public class TeamCityAssemblyNameProviderTests
    {
        [Test]
        [TestCase("123-dwdwd", true, "123")]
        [TestCase("a23-dwdwd", true, "a23")]
        [TestCase(null, false, null)]
        [TestCase("", false, null)]
        [TestCase("  ", false, null)]
        [TestCase("-32423df", false, null)]
        [TestCase("323-", false, null)]
        [TestCase("-", false, null)]
        [TestCase(" - ", false, null)]
        public void ShouldProvideFlowId(string id, bool expectedResolved, string expectedFlowId)
        {
            // Given

            // When
            string actualFlowId;
            var actualResolved = TeamCityAssemblyNameProvider.TryGetFlowId(id, out actualFlowId);

            // Then
            Assert.AreEqual(expectedResolved, actualResolved);
            Assert.AreEqual(expectedFlowId, actualFlowId);
        }

        [Test]
        [TestCase(@"c:\foo.dll", true, "foo.dll")]
        [TestCase(@"aa\bb\foo.dll", true, "foo.dll")]
        [TestCase(null, false, null)]
        [TestCase("", false, null)]
        [TestCase("bb", false, null)]
        [TestCase("foo.dll", false, null)]
        public void ShouldProvideAssemblyName(string fullName, bool expectedResolved, string expectedAssemblyName)
        {
            // Given
            fullName = NormalizePath(fullName);

            // When
            string actualAssemblyName;
            var actualResolved = TeamCityAssemblyNameProvider.TryGetAssemblyNameFromFullname(fullName, out actualAssemblyName);

            // Then
            Assert.AreEqual(expectedResolved, actualResolved);
            Assert.AreEqual(expectedAssemblyName, actualAssemblyName);
        }

        [Test]
        public void ShouldProvideAssemblyNameWhenAssemblySuiteRegistered()
        {
            // Given
            var resolver = new TeamCityAssemblyNameProvider();

            // When
            resolver.RegisterSuite(@"0-1186", NormalizePath(@"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll"));
            string assemblyName;
            var res = resolver.TryGetAssemblyName("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(true, res);
            Assert.AreEqual("nunit-console.tests.dll", assemblyName);
        }

        [Test]
        public void ShouldNotProvideAssemblyNameWhenHasNoAssemblySuiteWithCorrectFlowIdRegistered()
        {
            // Given
            var resolver = new TeamCityAssemblyNameProvider();

            // When
            resolver.RegisterSuite(@"2-1186", NormalizePath(@"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll"));
            string assemblyName;
            var res = resolver.TryGetAssemblyName("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(false, res);
            Assert.AreEqual(null, assemblyName);
        }

        [Test]
        public void ShouldNotProvideAssemblyNameWhenHasNoAssemblySuiteRegistered()
        {
            // Given
            var resolver = new TeamCityAssemblyNameProvider();

            // When
            string assemblyName;
            var res = resolver.TryGetAssemblyName("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(false, res);
            Assert.AreEqual(null, assemblyName);
        }

        [Test]
        public void ShouldNotProvideAssemblyNameWhenSuiteUnregistered()
        {
            // Given
            var resolver = new TeamCityAssemblyNameProvider();

            // When
            resolver.RegisterSuite(@"2-1186", NormalizePath(@"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll"));
            resolver.UnregisterSuite(@"2-1186", NormalizePath(@"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll"));
            string assemblyName;
            var res = resolver.TryGetAssemblyName("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(false, res);
            Assert.AreEqual(null, assemblyName);
        }

        [Test]
        public void ShouldNotProvideAssemblyNameWhenUnregisteredForOtherAssembly()
        {
            // Given
            var resolver = new TeamCityAssemblyNameProvider();

            // When
            resolver.RegisterSuite(@"0-1186", NormalizePath(@"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll"));
            resolver.UnregisterSuite(@"0-1186", NormalizePath(@"C:\Projects\GitHub\nunit\bin\Debug\foo.dll"));
            string assemblyName;
            var res = resolver.TryGetAssemblyName("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(false, res);
            Assert.AreEqual(null, assemblyName);
        }

        private static string NormalizePath(string fullName)
        {
            if (fullName != null)
            {
                fullName = fullName.Replace('\\', Path.DirectorySeparatorChar);
            }
            return fullName;
        }
    }
}
