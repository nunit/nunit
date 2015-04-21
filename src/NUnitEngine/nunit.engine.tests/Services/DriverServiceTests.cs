// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using NUnit.Framework;
using NUnit.Engine.Drivers;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services.Tests
{
    [TestFixture, Ignore("DriverService currently only works in the primary AppDomain")]
    public class DriverServiceTests
    {
        [Test]
        public void AssemblyUsesNUnitFrameworkDriver()
        {
            Assert.That(GetDriver("mock-nunit-assembly.exe"), Is.InstanceOf<NUnit3FrameworkDriver>());
        }

        [Test]
        public void MissingFileUsesNotRunnableFrameworkDriver()
        {
            Assert.That(GetDriver("junk.dll"), Is.InstanceOf<NotRunnableFrameworkDriver>());
        }

        [Test]
        public void UnknownFileTypeUsesNotRunnableFrameworkDriver()
        {
            Assert.That(GetDriver("mock-nunit-assembly.pdb"), Is.InstanceOf<NotRunnableFrameworkDriver>());
        }

        private IFrameworkDriver GetDriver(string fileName)
        {
            var factory = new DriverService();
            return factory.GetDriver(
                AppDomain.CurrentDomain,
                Path.Combine(TestContext.CurrentContext.TestDirectory, fileName));
        }
    }
}
