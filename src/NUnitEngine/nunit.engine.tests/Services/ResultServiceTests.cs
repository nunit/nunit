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
using NUnit.Framework;

namespace NUnit.Engine.Services.Tests
{
    using ResultWriters;

    public class ResultServiceTests
    {
        private IResultService _resultService;

        [SetUp]
        public void CreateService()
        {
            _resultService = new ResultService();
        }

        [Test]
        public void AvailableFormats()
        {
            Assert.That(_resultService.Formats, Is.EquivalentTo(new string[] { "nunit3", "nunit2", "cases", "user" }));
        }

        [TestCase("nunit3", null, ExpectedResult = typeof(NUnit3XmlResultWriter))]
        [TestCase("nunit2", null, ExpectedResult = typeof(NUnit2XmlResultWriter))]
        [TestCase("cases", null, ExpectedResult = typeof(TestCaseResultWriter))]
        [TestCase("user", new object[] { "TextSummary.xslt" }, ExpectedResult = typeof(XmlTransformResultWriter))]
        public Type CanGetWriter(string format, object[] args)
        {
            var writer = _resultService.GetResultWriter(format, args);

            Assert.NotNull(writer);
            return writer.GetType();
        }

        [Test]
        public void NUnit3Format_NonExistentTranform_ThrowsArgumentException()
        {
            Assert.That(
                () => _resultService.GetResultWriter("user", new object[] { "junk.xslt" }), 
                Throws.ArgumentException);
        }

        [Test]
        public void NUnit3Format_NullArgument_ThrowsArgumentNullException()
        {
            Assert.That(
                () => _resultService.GetResultWriter("user", null),
                Throws.TypeOf<ArgumentNullException>());
        }
    }
}
