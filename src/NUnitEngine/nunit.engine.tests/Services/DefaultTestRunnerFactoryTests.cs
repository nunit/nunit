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
        }

        // Single file
        [TestCase(new string[] { "EngineTests.nunit" }, typeof(ProcessRunner))] // Needs to exist because contents are checked
        [TestCase(new string[] { "x.dll" }, typeof(ProcessRunner))]
        // Two files
        [TestCase(new string[] { "x.nunit", "y.nunit" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.nunit", "y.dll" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.dll" }, typeof(MultipleTestProcessRunner))]
        // Three files
        [TestCase(new string[] { "x.nunit", "y.dll", "z.nunit" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.nunit", "z.dll" }, typeof(AggregatingTestRunner))]
        public void CorrectRunnerIsUsed_Default(string[] args, Type expectedType)
        {
            var package = new TestPackage(args);
            var runner = _factory.MakeTestRunner(package);

            Assert.That(runner, Is.TypeOf(expectedType));

            var aggRunner = runner as AggregatingTestRunner;
            if (aggRunner != null)
                foreach (var childRunner in aggRunner.Runners)
                    Assert.That(childRunner, Is.TypeOf<TestDomainRunner>());
        }

        // Single file
        [TestCase(new string[] { "EngineTests.nunit" }, typeof(TestDomainRunner))] // Needs to exist because contents are checked
        [TestCase(new string[] { "x.dll" }, typeof(TestDomainRunner))]
        // Two files
        [TestCase(new string[] { "x.nunit", "y.nunit" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.nunit", "y.dll" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.dll" }, typeof(TestDomainRunner))]
        // Three files
        [TestCase(new string[] { "x.nunit", "y.dll", "z.nunit" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.nunit", "z.dll" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.dll", "z.dll" }, typeof(TestDomainRunner))]
        public void CorrectRunnerIsUsed_ProcessModelSingle(string[] args, Type expectedType)
        {
            var package = new TestPackage(args);
            package.Settings["ProcessModel"] = "Single";
            var runner = _factory.MakeTestRunner(package);

            Assert.That(runner, Is.TypeOf(expectedType));

            var aggRunner = runner as AggregatingTestRunner;
            if (aggRunner != null)
                foreach (var childRunner in aggRunner.Runners)
                    Assert.That(childRunner, Is.TypeOf<TestDomainRunner>());
        }

        // Single file
        [TestCase(new string[] { "EngineTests.nunit" }, typeof(ProcessRunner))] // Needs to exist because contents are checked
        [TestCase(new string[] { "x.dll" }, typeof(ProcessRunner))]
        // Two files
        [TestCase(new string[] { "x.nunit", "y.nunit" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.nunit", "y.dll" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.dll" }, typeof(ProcessRunner))]
        // Three files
        [TestCase(new string[] { "x.nunit", "y.dll", "z.nunit" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.nunit", "z.dll" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.dll", "z.dll" }, typeof(ProcessRunner))]
        public void CorrectRunnerIsUsed_ProcessModelSeparate(string[] args, Type expectedType)
        {
            var package = new TestPackage(args);
            package.Settings["ProcessModel"] = "Separate";
            var runner = _factory.MakeTestRunner(package);

            Assert.That(runner, Is.TypeOf(expectedType));

            var aggRunner = runner as AggregatingTestRunner;
            if (aggRunner != null)
                foreach (var childRunner in aggRunner.Runners)
                    Assert.That(childRunner, Is.TypeOf<TestDomainRunner>());
        }

        // Single file
        [TestCase(new string[] { "EngineTests.nunit" }, typeof(MultipleTestProcessRunner))] // Needs to exist because contents are checked
        [TestCase(new string[] { "x.dll" }, typeof(MultipleTestProcessRunner))]
        // Two files
        [TestCase(new string[] { "x.nunit", "y.nunit" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.nunit", "y.dll" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.dll" }, typeof(MultipleTestProcessRunner))]
        // Three files
        [TestCase(new string[] { "x.nunit", "y.dll", "z.nunit" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.nunit", "z.dll" }, typeof(AggregatingTestRunner))]
        [TestCase(new string[] { "x.dll", "y.dll", "z.dll" }, typeof(MultipleTestProcessRunner))]
        public void CorrectRunnerIsUsed_ProcessModelMultiple(string[] args, Type expectedType)
        {
            var package = new TestPackage(args);
            package.Settings["ProcessModel"] = "Multiple";
            var runner = _factory.MakeTestRunner(package);
            Assert.That(runner, Is.TypeOf(expectedType));

            var aggRunner = runner as AggregatingTestRunner;
            if (aggRunner != null)
                foreach (var childRunner in aggRunner.Runners)
                    Assert.That(childRunner, Is.TypeOf<TestDomainRunner>());
        }

        [TestCase(new string[] { "x.junk" }, typeof(ProcessRunner))]
        [TestCase(new string[] { "x.junk", "y.dll" }, typeof(MultipleTestProcessRunner))]
        [TestCase(new string[] { "x.junk", "y.junk" }, typeof(MultipleTestProcessRunner))]
        [TestCase(new string[] { "x.dll", "y.junk", "z.dll" }, typeof(MultipleTestProcessRunner))]
        [TestCase(new string[] { "x.dll", "y.junk", "z.junk" }, typeof(MultipleTestProcessRunner))]
        public void CorrectRunnerIsUsed_InvalidExtension(string[] args, Type expectedType)
        {
            var package = new TestPackage(args);
            var runner = _factory.MakeTestRunner(package);

            Assert.That(runner, Is.TypeOf(expectedType));
        }
    }
}
