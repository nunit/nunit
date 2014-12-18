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
using System.Text;
using NUnit.Engine.Runners;
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    public class InProcessTestRunnerFactoryTests
    {
        private ServiceContext _services;
        private InProcessTestRunnerFactory _factory;

        [SetUp]
        public void CreateServiceContext()
        {
            _services = new ServiceContext();
            _services.Add(new ProjectService());
            _factory = new InProcessTestRunnerFactory();
            _services.Add(_factory);
        }

        // Single file
        [TestCase(new string[] { "x.dll" }, typeof(TestDomainRunner))]
        // Two files
        [TestCase(new string[] { "x.dll", "y.dll" }, typeof(TestDomainRunner))]
        // Three files
        [TestCase(new string[] { "x.dll", "y.dll", "z.dll" }, typeof(TestDomainRunner))]
        public void CorrectRunnerIsUsed_Default(string[] args, Type expectedType)
        {
            var package = new TestPackage(args);
            Assert.That(_factory.MakeTestRunner(package), Is.TypeOf(expectedType));
        }

        // Single file
        [TestCase(new string[] { "x.dll" }, typeof(TestDomainRunner))]
        // Two files
        [TestCase(new string[] { "x.dll", "y.dll" }, typeof(TestDomainRunner))]
        // Three files
        [TestCase(new string[] { "x.dll", "y.dll", "z.dll" }, typeof(TestDomainRunner))]
        public void CorrectRunnerIsUsed_DomainUsageSingle(string[] args, Type expectedType)
        {
            var package = new TestPackage(args);
            package.Settings["DomainUsage"] = "Single";
            Assert.That(_factory.MakeTestRunner(package), Is.TypeOf(expectedType));
        }

        // Single file
        [TestCase(new string[] { "x.dll" }, typeof(MultipleTestDomainRunner))]
        // Two files
        [TestCase(new string[] { "x.dll", "y.dll" }, typeof(MultipleTestDomainRunner))]
        // Three files
        [TestCase(new string[] { "x.dll", "y.dll", "z.dll" }, typeof(MultipleTestDomainRunner))]
        public void CorrectRunnerIsUsed_DomainUsageMultiple(string[] args, Type expectedType)
        {
            var package = new TestPackage(args);
            package.Settings["DomainUsage"] = "Multiple";
            Assert.That(_factory.MakeTestRunner(package), Is.TypeOf(expectedType));
        }
    }
}
