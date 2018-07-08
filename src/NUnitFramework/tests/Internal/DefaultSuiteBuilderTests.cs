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

using NUnit.TestData;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    using Builders;

    // NOTE: Because this fixture tests the IImplyFIxture interface, the attribute must be
    // present. Otherwise, if implied fixture recognition did not work, the tests would not run.
    [TestFixture]
    public static class DefaultSuiteBuilderTests
    {
        [Test]
        public static void CanBuildFromReturnsTrueForImpliedFixture()
        {
            Assert.That(new DefaultSuiteBuilder().CanBuildFrom(typeof(ImpliedFixture)));
        }

        [Test]
        public static void BuildFromReturnsImpliedFixture()
        {
            var suite = new DefaultSuiteBuilder().BuildFrom(typeof(ImpliedFixture)) as TestFixture;

            Assert.NotNull(suite, "No test fixture was built");
            Assert.That(suite.Name, Is.EqualTo(nameof(ImpliedFixture)));
        }
    }
}
