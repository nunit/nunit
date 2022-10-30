// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnitLite.Tests
{
    public class MakeRunSettingsTests
    {
        private static TestCaseData[] SettingsData =
        {
            new TestCaseData("--timeout=50", "DefaultTimeout", 50),
            new TestCaseData("--work=results", "WorkDirectory", Path.GetFullPath("results")),
            new TestCaseData("--seed=1234", "RandomSeed", 1234),
            new TestCaseData("--workers=8", "NumberOfTestWorkers", 8),
            new TestCaseData("--prefilter=A.B.C", "LOAD", new[] { "A.B.C" }),
            new TestCaseData("--test=A.B.C", "LOAD", new[] { "A.B.C" }),
            new TestCaseData("--test=A.B.C(arg)", "LOAD", new[] { "A.B.C" }),
            new TestCaseData("--test=A.B.C (arg)", "LOAD", new[] { "A.B.C" }),
            new TestCaseData("--test=A.B<type>.C(arg)", "LOAD", new[] { "A.B" })
       };

        [TestCaseSource(nameof(SettingsData))]
        public void CheckSetting(string option, string key, object value)
        {
            var options = new NUnitLiteOptions("test.dll", option);
            var settings = TextRunner.MakeRunSettings(options);

            Assert.That(settings.ContainsKey(key));
            Assert.AreEqual(value, settings[key]);
        }
    }
}
