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
    public class TeamCityAssemblyResolverTests
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
        public void ShouldResolveFlowId(string id, bool expectedResolved, string expectedFlowId)
        {
            // Given

            // When
            string actualFlowId;
            var actualResolved = TeamCityAssemblyResolver.TryGetFlowId(id, out actualFlowId);

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
        public void ShouldResolveAssemblyName(string fullName, bool expectedResolved, string expectedAssemblyName)
        {
            // Given
            if (fullName != null)
            {
                fullName = fullName.Replace('\\', Path.DirectorySeparatorChar);
            }

            // When
            string actualAssemblyName;
            var actualResolved = TeamCityAssemblyResolver.TryGetAssemblyName(fullName, out actualAssemblyName);

            // Then
            Assert.AreEqual(expectedResolved, actualResolved);
            Assert.AreEqual(expectedAssemblyName, actualAssemblyName);
        }

        [Test]
        public void ShouldResolveAssemblyWhenAssemblySuteRegistered()
        {
            // Given
            var resolver = new TeamCityAssemblyResolver();

            // When
            resolver.RegisterSuite(@"0-1186", @"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll");
            string assemblyName;
            var res = resolver.TryResolveAssembly("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(true, res);
            Assert.AreEqual("nunit-console.tests.dll", assemblyName);
        }

        [Test]
        public void ShouldNotResolveAssemblyWhenHasNoAssemblySuteWithCorrectFlowIdRegistered()
        {
            // Given
            var resolver = new TeamCityAssemblyResolver();

            // When
            resolver.RegisterSuite(@"2-1186", @"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll");
            string assemblyName;
            var res = resolver.TryResolveAssembly("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(false, res);
            Assert.AreEqual(null, assemblyName);
        }

        [Test]
        public void ShouldNotResolveAssemblyWhenHasNoAssemblySuteRegistered()
        {
            // Given
            var resolver = new TeamCityAssemblyResolver();

            // When
            string assemblyName;
            var res = resolver.TryResolveAssembly("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(false, res);
            Assert.AreEqual(null, assemblyName);
        }

        [Test]
        public void ShouldNotResolveAssemblyWhenSuteUnregistered()
        {
            // Given
            var resolver = new TeamCityAssemblyResolver();

            // When
            resolver.RegisterSuite(@"2-1186", @"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll");
            resolver.UnregisterSuite(@"2-1186", @"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll");
            string assemblyName;
            var res = resolver.TryResolveAssembly("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(false, res);
            Assert.AreEqual(null, assemblyName);
        }

        [Test]
        public void ShouldNotResolveAssemblyWhenUnregisteredForOtherAssembly()
        {
            // Given
            var resolver = new TeamCityAssemblyResolver();

            // When
            resolver.RegisterSuite(@"0-1186", @"C:\Projects\GitHub\nunit\bin\Debug\nunit-console.tests.dll");
            resolver.UnregisterSuite(@"0-1186", @"C:\Projects\GitHub\nunit\bin\Debug\foo.dll");
            string assemblyName;
            var res = resolver.TryResolveAssembly("0-1004", out assemblyName);

            // Then
            Assert.AreEqual(false, res);
            Assert.AreEqual(null, assemblyName);
        }
    }
}
