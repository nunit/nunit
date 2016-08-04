// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using NUnit.Framework;

namespace NUnit.Engine.Runners.Tests
{
    public class MultipleTestProcessRunnerTests
    {
        [TestCase(1, 0, ExpectedResult = 1)]
        [TestCase(1, 1, ExpectedResult = 1)]
        [TestCase(3, 0, ExpectedResult = 3)]
        [TestCase(3, 1, ExpectedResult = 1)]
        [TestCase(3, 2, ExpectedResult = 2)]
        [TestCase(3, 3, ExpectedResult = 3)]
        [TestCase(20, 8, ExpectedResult = 8)]
        [TestCase(8, 20, ExpectedResult = 8)]
        public int CheckLevelOfParallelism_ListOfAssemblies(int assemblyCount, int maxAgents)
        {
            return CreateRunner(assemblyCount, maxAgents).LevelOfParallelism;
        }

        [Test]
        public void CheckLevelOfParallelism_SingleAssembly()
        {
            var package = new TestPackage("junk.dll");
            Assert.That(new MultipleTestProcessRunner(new ServiceContext(), package).LevelOfParallelism, Is.EqualTo(0));
        }

        // Create a MultipleTestProcessRunner with a fake package consisting of
        // some number of assemblies and with an optional MaxAgents setting.
        // Zero means that MaxAgents is not specified.
        MultipleTestProcessRunner CreateRunner(int assemblyCount, int maxAgents)
        {
            // Currently, we can get away with null entries here
            var package = new TestPackage(new string[assemblyCount]);
            if (maxAgents > 0)
                package.Settings[EnginePackageSettings.MaxAgents] = maxAgents;
            return new MultipleTestProcessRunner(new ServiceContext(), package);
        }
    }
}
