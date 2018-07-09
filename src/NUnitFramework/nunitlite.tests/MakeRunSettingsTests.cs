// ***********************************************************************
// Copyright (c) 2014 Charlie Poole, Rob Prouse
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

using System.IO;
using NUnit.Common;
using NUnit.Framework;

namespace NUnitLite.Tests
{
    public class MakeRunSettingsTests
    {
        private static TestCaseData[] SettingsData =
        {
#if !NETCOREAPP1_1
            new TestCaseData("--timeout=50", "DefaultTimeout", 50),
#endif
            new TestCaseData("--work=results", "WorkDirectory", Path.GetFullPath("results")),
            new TestCaseData("--seed=1234", "RandomSeed", 1234),
            new TestCaseData("--workers=8", "NumberOfTestWorkers", 8),
            new TestCaseData("--prefilter=A.B.C", "LOAD", new string[] { "A.B.C" }),
            new TestCaseData("--test=A.B.C", "LOAD", new string[] { "A.B.C" }),
            new TestCaseData("--test=A.B.C(arg)", "LOAD", new string[] { "A.B.C" }),
            new TestCaseData("--test=A.B<type>.C(arg)", "LOAD", new string[] { "A.B" })
       };

        [TestCaseSource(nameof(SettingsData))]
        public void CheckSetting(string option, string key, object value)
        {
            var options = new NUnitLiteOptions("test.dll", option);
            var settings = TextRunner.MakeRunSettings(options);

            Assert.That(settings.ContainsKey(key));
            Assert.AreEqual(value, settings[key]);
        }

        //[Test]
        //public void WhenTestFixtureIsSpecified_RunSettingsIncludeIt)
    }
}
