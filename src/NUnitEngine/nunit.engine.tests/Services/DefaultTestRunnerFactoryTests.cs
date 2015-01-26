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
    public class DefaultTestRunnerFactoryTests
    {
        private ServiceContext _services;
        private DefaultTestRunnerFactory _factory;

        [SetUp]
        public void CreateServiceContext()
        {
            _services = new ServiceContext();
            _services.Add(new ProjectService());
            _factory = new DefaultTestRunnerFactory();
            _services.Add(_factory);
            _services.ServiceManager.InitializeServices();
        }

        // Single file
        [TestCase("EngineTests.nunit", null,        typeof(ProcessRunner))] // Needs to exist because contents are checked
        [TestCase("x.dll",             null,        typeof(ProcessRunner))]
        [TestCase("EngineTests.nunit", "Single",    typeof(TestDomainRunner))]
        [TestCase("x.dll",             "Single",    typeof(TestDomainRunner))]
        [TestCase("EngineTests.nunit", "Separate",  typeof(ProcessRunner))]
        [TestCase("x.dll",             "Separate",  typeof(ProcessRunner))]
        [TestCase("EngineTests.nunit", "Multiple",  typeof(MultipleTestProcessRunner))]
        [TestCase("x.dll",             "Multiple",  typeof(MultipleTestProcessRunner))]
        // Two files
        [TestCase("x.nunit y.nunit",   null,        typeof(AggregatingTestRunner))]
        [TestCase("x.nunit y.dll",     null,        typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.dll",       null,        typeof(MultipleTestProcessRunner))]
        [TestCase("x.nunit y.nunit",   "Single",    typeof(AggregatingTestRunner))]
        [TestCase("x.nunit y.dll",     "Single",    typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.dll",       "Single",    typeof(MultipleTestDomainRunner))]
        [TestCase("x.nunit y.nunit",   "Separate",  typeof(AggregatingTestRunner))]
        [TestCase("x.nunit y.dll",     "Separate",  typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.dll",       "Separate",  typeof(ProcessRunner))]
        [TestCase("x.nunit y.nunit",   "Multiple",  typeof(AggregatingTestRunner))]
        [TestCase("x.nunit y.dll",     "Multiple",  typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.dll",       "Multiple",  typeof(MultipleTestProcessRunner))]
        // Three files
        [TestCase("x.nunit y.dll z.nunit", null,       typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.nunit z.dll",   null,       typeof(AggregatingTestRunner))]
        [TestCase("x.nunit y.dll z.nunit", "Single",   typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.nunit z.dll",   "Single",   typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.dll z.dll",     "Single",   typeof(MultipleTestDomainRunner))]
        [TestCase("x.nunit y.dll z.nunit", "Separate", typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.nunit z.dll",   "Separate", typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.dll z.dll",     "Separate", typeof(ProcessRunner))]
        [TestCase("x.nunit y.dll z.nunit", "Multiple", typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.nunit z.dll",   "Multiple", typeof(AggregatingTestRunner))]
        [TestCase("x.dll y.dll z.dll",     "Multiple", typeof(MultipleTestProcessRunner))]
        public void CorrectRunnerIsUsed(string files, string processModel, Type expectedType)
        {
            var package = new TestPackage(files.Split(new char[] { ' ' }));
            if (processModel != null)
                package.Settings["ProcessModel"] = processModel;
            var runner = _factory.MakeTestRunner(package);

            Assert.That(runner, Is.TypeOf(expectedType));

            var aggRunner = runner as AggregatingTestRunner;
            if (aggRunner != null)
                foreach (var childRunner in aggRunner.Runners)
                    Assert.That(childRunner, Is.TypeOf<TestDomainRunner>());
        }

        [TestCase("x.junk", typeof(ProcessRunner))]
        [TestCase("x.junk y.dll", typeof(MultipleTestProcessRunner))]
        [TestCase("x.junk y.junk", typeof(MultipleTestProcessRunner))]
        [TestCase("x.dll y.junk z.dll", typeof(MultipleTestProcessRunner))]
        [TestCase("x.dll y.junk z.junk", typeof(MultipleTestProcessRunner))]
        public void CorrectRunnerIsUsed_InvalidExtension(string files, Type expectedType)
        {
            var package = new TestPackage(files.Split(new char[] {' '}));
            var runner = _factory.MakeTestRunner(package);

            Assert.That(runner, Is.TypeOf(expectedType));
        }
    }
}
