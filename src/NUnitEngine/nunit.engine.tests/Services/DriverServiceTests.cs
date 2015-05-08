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
    [TestFixture]
    public class DriverServiceTests
    {
        private DriverService _service;

        [SetUp]
        public void CreateDriverFactory()
        {
            _service = new DriverService();
            _service.StartService();
        }

        [Test]
        public void ServiceIsStarted()
        {
            Assert.That(_service.Status, Is.EqualTo(ServiceStatus.Started), "Failed to start service");
        }


        [TestCase("mock-nunit-assembly.exe", typeof(NUnit3FrameworkDriver))]
        [TestCase("mock-nunit-assembly.pdb", typeof(NotRunnableFrameworkDriver))]
        [TestCase("junk.dll", typeof(NotRunnableFrameworkDriver))]
        public void CorrectDriverIsUsed(string fileName, Type expectedType)
        {
            var driver = _service.GetDriver(
                AppDomain.CurrentDomain,
                Path.Combine(TestContext.CurrentContext.TestDirectory, fileName));

            Assert.That(driver, Is.InstanceOf(expectedType));
        }
    }
}
