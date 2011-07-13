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

using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Engine.Internal;
using NUnit.Framework;

namespace NUnit.Engine.Runners.Tests
{
    public class AbstractTestRunnerTests
    {
        [Test] // TODO: This needs more thorough testing
        public void TestResultsCanBeCombined()
        {
            var result1 = new TestEngineResult("<test-assembly result=\"Passed\" passed=\"23\"/>");
            var result2 = new TestEngineResult("<test-assembly result=\"Passed\" passed=\"31\"/>");

            var results = new List<TestEngineResult>();
            results.Add(result1);
            results.Add(result2);

            var startTime = new DateTime(2011, 07, 04, 12, 34, 56);

            XmlNode combined = AbstractTestRunner.MakeTestRunResult(startTime, results).Xml;

            Assert.That(combined.Name, Is.EqualTo("test-run"));
            Assert.That(combined.Attributes["result"].Value, Is.EqualTo("Passed"));
            Assert.That(combined.Attributes["passed"].Value, Is.EqualTo("54"));
            Assert.That(combined.Attributes["run-date"].Value, Is.EqualTo("2011-07-04"));
            Assert.That(combined.Attributes["start-time"].Value, Is.EqualTo("12:34:56"));
        }
    }
}
