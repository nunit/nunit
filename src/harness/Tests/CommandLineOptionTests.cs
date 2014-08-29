// ***********************************************************************
// Copyright (c) 2013 Charlie Poole
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
using System.Reflection;

namespace NUnit.Framework.TestHarness.Options.Tests
{
    [TestFixture][Category("Options")]
    public class CommandLineOptionTests
    {
        #region General Tests

        [Test]
        public void NoInputFileIsInvalid()
        {
            CommandLineOptions options = new CommandLineOptions();
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.AreEqual("Error: No assembly was specified", options.ErrorMessages[0]);
        }

        [Test]
        public void MultipleInputFilesIsInvalid()
        {
            CommandLineOptions options = new CommandLineOptions("test.dll", "dummy.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.AreEqual("Error: Only one assembly may be specified", options.ErrorMessages[0]);
        }

        [TestCase("ShowHelp", "help|h")]
        [TestCase("WaitBeforeExit", "wait")]
        [TestCase("NoHeader", "noheader|noh")]
        [TestCase("DisplayTeamCityServiceMessages", "teamcity")]
        public void CanRecognizeBooleanOptions(string propertyName, string pattern)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(bool), property.PropertyType, "Property '{0}' is wrong type", propertyName);

            foreach (string option in prototypes)
            {
                CommandLineOptions options = new CommandLineOptions("-" + option);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize -" + option);

                options = new CommandLineOptions("-" + option + "+");
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize -" + option + "+");

                options = new CommandLineOptions("-" + option + "-");
                Assert.AreEqual(false, (bool)property.GetValue(options, null), "Didn't recognize -" + option + "-");

                options = new CommandLineOptions("--" + option);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize --" + option);

                options = new CommandLineOptions("/" + option);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize /" + option);
            }
        }

        [TestCase("Include", "include", new string[] { "Short,Fast" }, new string[0])]
        [TestCase("Exclude", "exclude", new string[] { "Long" }, new string[0])]
        [TestCase("OutFile", "output|out", new string[] { "output.txt" }, new string[0])]
        [TestCase("ErrFile", "err", new string[] { "error.txt" }, new string[0])]
        [TestCase("WorkDirectory", "work", new string[] { "results" }, new string[0])]
        [TestCase("DisplayTestLabels", "labels|l", new string[] { "Off", "On", "All" }, new string[] { "JUNK" })]
        [TestCase("InternalTraceLevel", "trace", new string[] { "Off", "Error", "Warning", "Info", "Debug", "Verbose" }, new string[] { "JUNK" })]
        [TestCase("DomainUsage", "domain", new string[] {"None", "Single", "Multiple"}, new string[] { "JUNK" })]
        public void CanRecognizeStringOptions(string propertyName, string pattern, string[] goodValues, string[] badValues)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(string), property.PropertyType);

            foreach (string option in prototypes)
            {
                foreach (string value in goodValues)
                {
                    string optionPlusValue = string.Format("-{0}:{1}", option, value);
                    CommandLineOptions options = new CommandLineOptions(optionPlusValue, "dummy.dll");
                    Assert.True(options.Validate(), "Should be valid: " + optionPlusValue);
                    Assert.AreEqual(value, (string)property.GetValue(options, null), "Didn't recognize " + optionPlusValue);
                }

                foreach (string value in badValues)
                {
                    string optionPlusValue = string.Format("--{0}:{1}", option, value);
                    CommandLineOptions options = new CommandLineOptions(optionPlusValue);
                    Assert.False(options.Validate(), "Should not be valid: " + optionPlusValue);
                }
            }
        }

        [TestCase("DefaultTimeout", "timeout", typeof(int))]
        [TestCase("NumWorkers", "workers", typeof(int?))]
        public void CanRecognizeIntOptions(string propertyName, string pattern, Type realType)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(realType, property.PropertyType);

            foreach (string option in prototypes)
            {
                CommandLineOptions options = new CommandLineOptions("--" + option + ":42");
                Assert.AreEqual(42, (int)property.GetValue(options, null), "Didn't recognize --" + option + ":42");
            }
        }

        [TestCase("--include")]
        [TestCase("--exclude")]
        [TestCase("--result")]
        [TestCase("--timeout")]
        [TestCase("--output")]
        [TestCase("--err")]
        [TestCase("--work")]
        [TestCase("--trace")]
        [TestCase("--test")]
        [TestCase("--domain")]
        public void MissingValuesAreReported(string option)
        {
            CommandLineOptions options = new CommandLineOptions(option + "=");
            Assert.False(options.Validate(), "Missing value should not be valid");
            Assert.AreEqual("Missing required value for option '" + option + "'.", options.ErrorMessages[0]);
        }

        [Test]
        public void AssemblyAloneIsValid()
        {
            CommandLineOptions options = new CommandLineOptions("nunit.tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual("nunit.tests.dll", options.AssemblyName);
            Assert.AreEqual(0, options.Tests.Count);
        }

        [Test]
        public void TestNamePlusAssemblyIsValid()
        {
            CommandLineOptions options = new CommandLineOptions("-test:NUnit.Tests.AllTests", "nunit.tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual("nunit.tests.dll", options.AssemblyName);
            Assert.AreEqual(1, options.Tests.Count);
            Assert.AreEqual("NUnit.Tests.AllTests", options.Tests[0]);
        }

        [Test]
        public void MultipleTestNamesAreValid()
        {
            CommandLineOptions options = new CommandLineOptions("-test:NUnit.Tests.Test1,NUnit.Tests.Test2,NUnit.Tests.Test3", "nunit.tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual("nunit.tests.dll", options.AssemblyName);
            Assert.AreEqual(3, options.Tests.Count);
            Assert.AreEqual("NUnit.Tests.Test1", options.Tests[0]);
            Assert.AreEqual("NUnit.Tests.Test2", options.Tests[1]);
            Assert.AreEqual("NUnit.Tests.Test3", options.Tests[2]);
        }

        [Test]
        public void MultipleTestOptionsAreValid()
        {
            CommandLineOptions options = new CommandLineOptions("-test:NUnit.Tests.Test1", "-test:NUnit.Tests.Test2", "nunit.tests.dll", "-test:NUnit.Tests.Test3");
            Assert.True(options.Validate());
            Assert.AreEqual("nunit.tests.dll", options.AssemblyName);
            Assert.AreEqual(3, options.Tests.Count);
            Assert.AreEqual("NUnit.Tests.Test1", options.Tests[0]);
            Assert.AreEqual("NUnit.Tests.Test2", options.Tests[1]);
            Assert.AreEqual("NUnit.Tests.Test3", options.Tests[2]);
        }

        [Test]
        public void InvalidOption()
        {
            CommandLineOptions options = new CommandLineOptions("--assembly:nunit.tests.dll", "dummy.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.AreEqual("Invalid argument: --assembly:nunit.tests.dll", options.ErrorMessages[0]);
        }

        [Test]
        public void InvalidCommandLineParms()
        {
            CommandLineOptions options = new CommandLineOptions("--garbage:TestFixture", "--assembly:Tests.dll", "dummy.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(2, options.ErrorMessages.Count);
            Assert.AreEqual("Invalid argument: --garbage:TestFixture", options.ErrorMessages[0]);
            Assert.AreEqual("Invalid argument: --assembly:Tests.dll", options.ErrorMessages[1]);
        }

        #endregion

        #region CreateDriverSettings

        [TestCase("-timeout=50", "DefaultTimeout", 50)]
        [TestCase("--workers=3", "NumberOfTestWorkers", 3)]
        [TestCase("--workers=0", "NumberOfTestWorkers", 0)]
        [TestCase("--seed=123456789", "RandomSeed", 123456789)]
        [TestCase("--labels=On", "DisplayTestLabels", "On")]
        [TestCase("--trace=Debug", "InternalTraceLevel", "Debug")]
        [TestCase("--work=results", "WorkDirectory", "results")]
        [TestCase("--teamcity", "DisplayTeamCityServiceMessages", true)]
        public void CreateDriverSettings<T>(string option, string settingName, T value)
        {
            var options = new CommandLineOptions(option);
            var settings = options.CreateDriverSettings();
            Assert.That(settings.ContainsKey(settingName), settingName + " not found in settings");
            Assert.That(settings[settingName], Is.EqualTo(value));
            Assert.That(value is string || value is bool || value is int, "Settings must be of Type bool, string or int");
        }

        #endregion

        #region Timeout Option

        [Test]
        public void TimeoutIsMinusOneIfNoOptionIsProvided()
        {
            CommandLineOptions options = new CommandLineOptions("tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual(-1, options.DefaultTimeout);
        }

        [Test]
        public void TimeoutThrowsExceptionIfOptionHasNoValue()
        {
            Assert.Throws<Mono.Options.OptionException>(
                () => new CommandLineOptions("tests.dll", "-timeout"));
        }

        [Test]
        public void TimeoutParsesIntValueCorrectly()
        {
            CommandLineOptions options = new CommandLineOptions("tests.dll", "-timeout:5000");
            Assert.True(options.Validate());
            Assert.AreEqual(5000, options.DefaultTimeout);
        }

        [Test]
        public void TimeoutCausesErrorIfValueIsNotInteger()
        {
            CommandLineOptions options = new CommandLineOptions("tests.dll", "-timeout:abc");
            Assert.False(options.Validate());
            Assert.AreEqual(-1, options.DefaultTimeout);
        }

        #endregion

        #region Result Option

        [Test]
        public void ResultOptionWithFilePath()
        {
            var options = new CommandLineOptions("-result:results.xml");

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ResultOptionWithFilePathAndFormat()
        {
            var options = new CommandLineOptions("-result:results.xml;format=nunit2");

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit2", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ResultOptionWithFilePathAndTransform()
        {
            var options = new CommandLineOptions("-result:results.xml;transform=transform.xslt");

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("user", spec.Format);
            Assert.AreEqual("transform.xslt", spec.Transform);
        }

        [Test]
        public void ResultOptionMayBeRepeated()
        {
            var options = new CommandLineOptions("-result:results.xml", "-result:nunit2results.xml;format=nunit2", "-result:myresult.xml;transform=mytransform.xslt");

            var specs = options.ResultOutputSpecifications;
            Assert.AreEqual(3, specs.Count);

            var spec1 = specs[0];
            Assert.AreEqual("results.xml", spec1.OutputPath);
            Assert.AreEqual("nunit3", spec1.Format);
            Assert.Null(spec1.Transform);

            var spec2 = specs[1];
            Assert.AreEqual("nunit2results.xml", spec2.OutputPath);
            Assert.AreEqual("nunit2", spec2.Format);
            Assert.Null(spec2.Transform);

            var spec3 = specs[2];
            Assert.AreEqual("myresult.xml", spec3.OutputPath);
            Assert.AreEqual("user", spec3.Format);
            Assert.AreEqual("mytransform.xslt", spec3.Transform);
        }

        [Test]
        public void DefaultResultSpecification()
        {
            var options = new CommandLineOptions("test.dll");
            Assert.AreEqual(1, options.ResultOutputSpecifications.Count);

            var spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("TestResult.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void NoResultSuppressesDefaultResultSpecification()
        {
            var options = new CommandLineOptions("test.dll", "-noresult");
            Assert.AreEqual(0, options.ResultOutputSpecifications.Count);
        }

        [Test]
        public void NoResultSuppressesAllResultSpecifications()
        {
            var options = new CommandLineOptions("test.dll", "-result:results.xml", "-noresult", "-result:nunit2results.xml;format=nunit2");
            Assert.AreEqual(0, options.ResultOutputSpecifications.Count);
        }

        #endregion

        #region Explore Option

        [Test]
        public void ExploreOptionWithoutPath()
        {
            var options = new CommandLineOptions("tests.dll", "-explore");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.ExploreOutputSpecifications, Is.Empty);
            Assert.True(options.Explore);
        }

        [Test]
        public void ExploreOptionWithFilePath()
        {
            var options = new CommandLineOptions("tests.dll", "-explore:results.xml");
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ExploreOptionWithFilePathAndFormat()
        {
            var options = new CommandLineOptions("tests.dll", "-explore:results.xml;format=cases");
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("cases", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ExploreOptionWithFilePathAndTransform()
        {
            var options = new CommandLineOptions("tests.dll", "-explore:results.xml;transform=myreport.xslt");
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("user", spec.Format);
            Assert.AreEqual("myreport.xslt", spec.Transform);
        }

        #endregion

        #region Helper Methods

        private static FieldInfo GetFieldInfo(string fieldName)
        {
            FieldInfo field = typeof(CommandLineOptions).GetField(fieldName);
            Assert.IsNotNull(field, "The field '{0}' is not defined", fieldName);
            return field;
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            PropertyInfo property = typeof(CommandLineOptions).GetProperty(propertyName);
            Assert.IsNotNull(property, "The property '{0}' is not defined", propertyName);
            return property;
        }

        #endregion
    }
}
