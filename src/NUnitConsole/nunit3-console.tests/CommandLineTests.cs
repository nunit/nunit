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

using System.IO;
using System.Reflection;
using NUnit.Common;
using NUnit.Options;

namespace NUnit.ConsoleRunner.Tests
{
    using System.Collections.Generic;
    using Framework;

    [TestFixture]
    public class CommandLineTests
    {
        #region General Tests

        [Test]
        public void NoInputFiles()
        {
            ConsoleOptions options = new ConsoleOptions();
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.InputFiles.Count);
        }

        [TestCase("ShowHelp", "help|h")]
        [TestCase("ShowVersion", "version|V")]
        [TestCase("StopOnError", "stoponerror")]
        [TestCase("WaitBeforeExit", "wait")]
        [TestCase("NoHeader", "noheader|noh")]
        [TestCase("RunAsX86", "x86")]
        [TestCase("DisposeRunners", "dispose-runners")]
        [TestCase("ShadowCopyFiles", "shadowcopy")]
        [TestCase("TeamCity", "teamcity")]
        [TestCase("DebugTests", "debug")]
        [TestCase("PauseBeforeRun", "pause")]
#if DEBUG
        [TestCase("DebugAgent", "debug-agent")]
#endif
        public void CanRecognizeBooleanOptions(string propertyName, string pattern)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(bool), property.PropertyType, "Property '{0}' is wrong type", propertyName);

            foreach (string option in prototypes)
            {
                ConsoleOptions options;

                if (option.Length == 1)
                {
                    options = new ConsoleOptions("-" + option);
                    Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize -" + option);

                    options = new ConsoleOptions("-" + option + "+");
                    Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize -" + option + "+");

                    options = new ConsoleOptions("-" + option + "-");
                    Assert.AreEqual(false, (bool)property.GetValue(options, null), "Didn't recognize -" + option + "-");
                }
                else
                {
                    options = new ConsoleOptions("--" + option);
                    Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize --" + option);
                }

                options = new ConsoleOptions("/" + option);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize /" + option);
            }
        }

        [TestCase("WhereClause",        "where",      new string[] { "cat==Fast" },                      new string[0])]
        [TestCase("ActiveConfig",       "config",     new string[] { "Debug" },                          new string[0])]
        [TestCase("ProcessModel",       "process",    new string[] { "InProcess", "Separate", "Multiple" }, new string[] { "JUNK" })]
        [TestCase("DomainUsage",        "domain",     new string[] { "None", "Single", "Multiple" },     new string[] { "JUNK" })]
        [TestCase("Framework",          "framework",  new string[] { "net-4.0" },                        new string[0])]
        [TestCase("OutFile",            "output|out", new string[] { "output.txt" },                     new string[0])]
        [TestCase("ErrFile",            "err",        new string[] { "error.txt" },                      new string[0])]
        [TestCase("WorkDirectory",      "work",       new string[] { "results" },                        new string[0])]
        [TestCase("DisplayTestLabels",  "labels",     new string[] { "Off", "On", "All" },               new string[] { "JUNK" })]
        [TestCase("InternalTraceLevel", "trace",      new string[] { "Off", "Error", "Warning", "Info", "Debug", "Verbose" }, new string[] { "JUNK" })]
        public void CanRecognizeStringOptions(string propertyName, string pattern, string[] goodValues, string[] badValues)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(string), property.PropertyType);

            foreach (string option in prototypes)
            {
                foreach (string value in goodValues)
                {
                    string optionPlusValue = string.Format("--{0}:{1}", option, value);
                    ConsoleOptions options = new ConsoleOptions(optionPlusValue);
                    Assert.True(options.Validate(), "Should be valid: " + optionPlusValue);
                    Assert.AreEqual(value, (string)property.GetValue(options, null), "Didn't recognize " + optionPlusValue);
                }

                foreach (string value in badValues)
                {
                    string optionPlusValue = string.Format("--{0}:{1}", option, value);
                    ConsoleOptions options = new ConsoleOptions(optionPlusValue);
                    Assert.False(options.Validate(), "Should not be valid: " + optionPlusValue);
                }
            }
        }

        [Test]
        public void CanRegognizeInProcessOption()
        {
            ConsoleOptions options = new ConsoleOptions("--inprocess");
            Assert.True(options.Validate(), "Should be valid: --inprocess");
            Assert.AreEqual("InProcess", options.ProcessModel, "Didn't recognize --inprocess");
        }

        [TestCase("ProcessModel", "process", new string[] { "InProcess", "Separate", "Multiple" })]
        [TestCase("DomainUsage", "domain", new string[] { "None", "Single", "Multiple" })]
        [TestCase("DisplayTestLabels", "labels", new string[] { "Off", "On", "All" })]
        [TestCase("InternalTraceLevel", "trace", new string[] { "Off", "Error", "Warning", "Info", "Debug", "Verbose" })]
        public void CanRecognizeLowerCaseOptionValues(string propertyName, string optionName, string[] canonicalValues)
        {
            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(string), property.PropertyType);

            foreach (string canonicalValue in canonicalValues)
            {
                string lowercaseValue = canonicalValue.ToLowerInvariant();
                string optionPlusValue = string.Format("--{0}:{1}", optionName, lowercaseValue);
                ConsoleOptions options = new ConsoleOptions(optionPlusValue);
                Assert.True(options.Validate(), "Should be valid: " + optionPlusValue);
                Assert.AreEqual(canonicalValue, (string)property.GetValue(options, null), "Didn't recognize " + optionPlusValue);
            }
        }

        [TestCase("DefaultTimeout", "timeout")]
        [TestCase("RandomSeed", "seed")]
        [TestCase("NumberOfTestWorkers", "workers")]
        [TestCase("MaxAgents", "agents")]
        public void CanRecognizeIntOptions(string propertyName, string pattern)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(int), property.PropertyType);

            foreach (string option in prototypes)
            {
                ConsoleOptions options = new ConsoleOptions("--" + option + ":42");
                Assert.AreEqual(42, (int)property.GetValue(options, null), "Didn't recognize --" + option + ":42");
            }
        }

        //[TestCase("InternalTraceLevel", "trace", typeof(InternalTraceLevel))]
        //public void CanRecognizeEnumOptions(string propertyName, string pattern, Type enumType)
        //{
        //    string[] prototypes = pattern.Split('|');

        //    PropertyInfo property = GetPropertyInfo(propertyName);
        //    Assert.IsNotNull(property, "Property {0} not found", propertyName);
        //    Assert.IsTrue(property.PropertyType.IsEnum, "Property {0} is not an enum", propertyName);
        //    Assert.AreEqual(enumType, property.PropertyType);

        //    foreach (string option in prototypes)
        //    {
        //        foreach (string name in Enum.GetNames(enumType))
        //        {
        //            {
        //                ConsoleOptions options = new ConsoleOptions("--" + option + ":" + name);
        //                Assert.AreEqual(name, property.GetValue(options, null).ToString(), "Didn't recognize -" + option + ":" + name);
        //            }
        //        }
        //    }
        //}

        [TestCase("--where")]
        [TestCase("--config")]
        [TestCase("--process")]
        [TestCase("--domain")]
        [TestCase("--framework")]
        [TestCase("--timeout")]
        [TestCase("--output")]
        [TestCase("--err")]
        [TestCase("--work")]
        [TestCase("--trace")]
        public void MissingValuesAreReported(string option)
        {
            ConsoleOptions options = new ConsoleOptions(option + "=");
            Assert.False(options.Validate(), "Missing value should not be valid");
            Assert.AreEqual("Missing required value for option '" + option + "'.", options.ErrorMessages[0]);
        }

        [Test]
        public void AssemblyName()
        {
            ConsoleOptions options = new ConsoleOptions("nunit.tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count);
            Assert.AreEqual("nunit.tests.dll", options.InputFiles[0]);
        }

        //[Test]
        //public void FixtureNamePlusAssemblyIsValid()
        //{
        //    ConsoleOptions options = new ConsoleOptions( "-fixture:NUnit.Tests.AllTests", "nunit.tests.dll" );
        //    Assert.AreEqual("nunit.tests.dll", options.Parameters[0]);
        //    Assert.AreEqual("NUnit.Tests.AllTests", options.fixture);
        //    Assert.IsTrue(options.Validate());
        //}

        [Test]
        public void AssemblyAloneIsValid()
        {
            ConsoleOptions options = new ConsoleOptions("nunit.tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.ErrorMessages.Count, "command line should be valid");
        }

        [Test]
        public void X86AndInProcessAreNotCompatible()
        {
            ConsoleOptions options = new ConsoleOptions("nunit.tests.dll", "--x86", "--inprocess");
            Assert.False(options.Validate(), "Should be invalid");
            Assert.AreEqual("The --x86 and --inprocess options are incompatible.", options.ErrorMessages[0]);
        }

        [Test]
        public void InvalidOption()
        {
            ConsoleOptions options = new ConsoleOptions("-assembly:nunit.tests.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.AreEqual("Invalid argument: -assembly:nunit.tests.dll", options.ErrorMessages[0]);
        }


        //[Test]
        //public void NoFixtureNameProvided()
        //{
        //    ConsoleOptions options = new ConsoleOptions( "-fixture:", "nunit.tests.dll" );
        //    Assert.IsFalse(options.Validate());
        //}

        [Test]
        public void InvalidCommandLineParms()
        {
            ConsoleOptions options = new ConsoleOptions("-garbage:TestFixture", "-assembly:Tests.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(2, options.ErrorMessages.Count);
            Assert.AreEqual("Invalid argument: -garbage:TestFixture", options.ErrorMessages[0]);
            Assert.AreEqual("Invalid argument: -assembly:Tests.dll", options.ErrorMessages[1]);
        }

        #endregion

        #region Timeout Option

        [Test]
        public void TimeoutIsMinusOneIfNoOptionIsProvided()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual(-1, options.DefaultTimeout);
        }

        [Test]
        public void TimeoutThrowsExceptionIfOptionHasNoValue()
        {
            Assert.Throws<OptionException>(() => new ConsoleOptions("tests.dll", "-timeout"));
        }

        [Test]
        public void TimeoutParsesIntValueCorrectly()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-timeout:5000");
            Assert.True(options.Validate());
            Assert.AreEqual(5000, options.DefaultTimeout);
        }

        [Test]
        public void TimeoutCausesErrorIfValueIsNotInteger()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-timeout:abc");
            Assert.False(options.Validate());
            Assert.AreEqual(-1, options.DefaultTimeout);
        }

        #endregion

        #region EngineResult Option

        [Test]
        public void ResultOptionWithFilePath()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-result:results.xml");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ResultOptionWithFilePathAndFormat()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-result:results.xml;format=nunit2");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit2", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ResultOptionWithFilePathAndTransform()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-result:results.xml;transform=transform.xslt");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("user", spec.Format);
            Assert.AreEqual("transform.xslt", spec.Transform);
        }

        [Test]
        public void FileNameWithoutResultOptionLooksLikeParameter()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "results.xml");
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.ErrorMessages.Count);
            Assert.AreEqual(2, options.InputFiles.Count);
        }

        [Test]
        public void ResultOptionWithoutFileNameIsInvalid()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-result:");
            Assert.False(options.Validate(), "Should not be valid");
            Assert.AreEqual(1, options.ErrorMessages.Count, "An error was expected");
        }

        [Test]
        public void ResultOptionMayBeRepeated()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-result:results.xml", "-result:nunit2results.xml;format=nunit2", "-result:myresult.xml;transform=mytransform.xslt");
            Assert.True(options.Validate(), "Should be valid");

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
            var options = new ConsoleOptions("test.dll");
            Assert.AreEqual(1, options.ResultOutputSpecifications.Count);

            var spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("TestResult.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void NoResultSuppressesDefaultResultSpecification()
        {
            var options = new ConsoleOptions("test.dll", "-noresult");
            Assert.AreEqual(0, options.ResultOutputSpecifications.Count);
        }

        [Test]
        public void NoResultSuppressesAllResultSpecifications()
        {
            var options = new ConsoleOptions("test.dll", "-result:results.xml", "-noresult", "-result:nunit2results.xml;format=nunit2");
            Assert.AreEqual(0, options.ResultOutputSpecifications.Count);
        }

        #endregion

        #region Explore Option

        [Test]
        public void ExploreOptionWithoutPath()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-explore");
            Assert.True(options.Validate());
            Assert.True(options.Explore);
        }

        [Test]
        public void ExploreOptionWithFilePath()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-explore:results.xml");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ExploreOptionWithFilePathAndFormat()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-explore:results.xml;format=cases");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("cases", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ExploreOptionWithFilePathAndTransform()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-explore:results.xml;transform=myreport.xslt");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("user", spec.Format);
            Assert.AreEqual("myreport.xslt", spec.Transform);
        }

        [Test]
        public void ExploreOptionWithFilePathUsingEqualSign()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-explore=C:/nunit/tests/bin/Debug/console-test.xml");
            Assert.True(options.Validate());
            Assert.True(options.Explore);
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.AreEqual("C:/nunit/tests/bin/Debug/console-test.xml", options.ExploreOutputSpecifications[0].OutputPath);
        }

        [Test]
        [TestCase(true, null, true)]
        [TestCase(false, null, false)]
        [TestCase(true, false, true)]
        [TestCase(false, false, false)]
        [TestCase(true, true, true)]
        [TestCase(false, true, true)]
        public void ShouldSetTeamCityFlagAccordingToArgsAndDefauls(bool hasTeamcityInCmd, bool? defaultTeamcity, bool expectedTeamCity)
        {
            // Given
            List<string> args = new List<string> { "tests.dll" };
            if (hasTeamcityInCmd)
            {
                args.Add("--teamcity");
            }

            ConsoleOptions options;
            if (defaultTeamcity.HasValue)
            {
                options = new ConsoleOptions(new DefaultOptionsProviderStub(defaultTeamcity.Value), args.ToArray());
            }
            else
            {
                options = new ConsoleOptions(args.ToArray());
            }

            // When
            var actualTeamCity = options.TeamCity;

            // Then
            Assert.AreEqual(actualTeamCity, expectedTeamCity);
        }

        #endregion

        #region Testlist Option

        [Test]
        public void ShouldNotFailOnEmptyLine()
        {
            var testListPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestListWithEmptyLine.tst");
            // Not copying this test file into releases
            Assume.That(testListPath, Does.Exist);
            var options = new ConsoleOptions("--testlist=" + testListPath);
            Assert.That(options.errorMessages, Is.Empty);
            Assert.That(options.TestList, Is.EqualTo(new[] {"AmazingTest"}));
        }

        #endregion

        #region Helper Methods

        private static FieldInfo GetFieldInfo(string fieldName)
        {
            FieldInfo field = typeof(ConsoleOptions).GetField(fieldName);
            Assert.IsNotNull(field, "The field '{0}' is not defined", fieldName);
            return field;
        }

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            PropertyInfo property = typeof(ConsoleOptions).GetProperty(propertyName);
            Assert.IsNotNull(property, "The property '{0}' is not defined", propertyName);
            return property;
        }

        #endregion

        internal sealed class DefaultOptionsProviderStub : IDefaultOptionsProvider
        {
            public DefaultOptionsProviderStub(bool teamCity)
            {
                TeamCity = teamCity;
            }

            public bool TeamCity { get; private set; }
        }
    }
}
