// ***********************************************************************
// Copyright (c) 2014-2015 Charlie Poole
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
using System.Text;
using NUnit.Engine.Runners;
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    using Fakes;

    public class CoreTestRunnerFactoryTests
    {
        private CoreTestRunnerFactory _factory;

        [SetUp]
        public void CreateServiceContext()
        {
            var services = new ServiceContext();
            _factory = new CoreTestRunnerFactory();
            services.Add(_factory);
            services.ServiceManager.StartServices();
        }

        [Test]
        public void ServiceIsStarted()
        {
            Assert.That(_factory.Status, Is.EqualTo(ServiceStatus.Started), "Failed to start service");
        }

        // Single file
        [TestCase("x.dll",  null,      typeof(TestDomainRunner))]
        [TestCase("x.dll", "Single", typeof(TestDomainRunner))]
        [TestCase("x.dll", "None", typeof(LocalTestRunner))]
        // Two files
        [TestCase("x.dll y.dll",  null,     typeof(TestDomainRunner))]
        [TestCase("x.dll y.dll", "Single", typeof(TestDomainRunner))]
        [TestCase("x.dll y.dll", "None", typeof(LocalTestRunner))]
        // Three files
        [TestCase("x.dll y.dll z.dll", null,       typeof(TestDomainRunner))]
        [TestCase("x.dll y.dll z.dll", "Single", typeof(TestDomainRunner))]
        [TestCase("x.dll y.dll z.dll", "None", typeof(LocalTestRunner))]
        public void CorrectRunnerIsUsed(string files, string domainUsage, Type expectedType)
        {
            var package = new TestPackage(files.Split(new char[] { ' ' }));
            if (domainUsage != null)
                package.Settings["DomainUsage"] = domainUsage;
            Assert.That(_factory.MakeTestRunner(package), Is.TypeOf(expectedType));
        }
    }
}
