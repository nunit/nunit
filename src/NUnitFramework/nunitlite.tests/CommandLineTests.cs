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
using System.Reflection;
using System.Globalization;
using NUnit.Common;
using NUnit.Options;
using NUnit.Framework;

namespace NUnitLite.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using NUnit.TestUtilities;

    [TestFixture]
    public class CommandLineTests
    {
        #region @filename Tests

#if !PORTABLE
        [Test]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt", "--filearg1\r\n--filearg2", "--arg1 --filearg1 --filearg2 --arg2", "")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt", "--filearg1 --filearg2", "--arg1 --filearg1 --filearg2 --arg2", "")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt", "--filearg1 --filearg2\r\n--filearg3 --filearg4", "--arg1 --filearg1 --filearg2 --filearg3 --filearg4 --arg2", "")]
        [TestCase("--arg1 @[,]file1.txt --arg2", "file1.txt", "--filearg1:filearg2\r\nfilearg3\r\nfilearg4", "--arg1 --filearg1:filearg2,filearg3,filearg4 --arg2", "")]
        [TestCase("--arg1 @file1.txt --arg2", "", "", "--arg1 --arg2", "The file \"file1.txt\" was not found")]
        [TestCase("--arg1 @ --arg2", "", "", "--arg1 --arg2", "The file name should not be empty")]
        [TestCase("--arg1 @file1.txt --arg2 @file2.txt", "file1.txt|file2.txt", "--filearg1 --filearg2|--filearg3", "--arg1 --filearg1 --filearg2 --arg2 --filearg3", "")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt", "", "--arg1 --arg2", "")]
        [TestCase("--arg1 @file1.txt --arg2 @file2.txt", "file1.txt|file2.txt|file3.txt", "--filearg1 --filearg2|--filearg3 @file3.txt|--filearg4", "--arg1 --filearg1 --filearg2 --arg2 --filearg3 --filearg4", "")]
        public void AtsignFilenameTests(string commandLine, string testFileNames, string testFileContents, string expectedArgs, string expectedErrors)
        {
            var ee = expectedErrors.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var tfn = testFileNames.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var tfc = testFileContents.Split(new[] { '|' });
            var tfs = new TestFile[tfn.Length];

            for (int ix = 0; ix < tfn.Length; ++ix)
                tfs[ix] = new TestFile(Path.Combine(TestContext.CurrentContext.TestDirectory, tfn[ix]), tfc[ix], true);

            var options = new NUnitLiteOptions();

            string actualExpectedArgs;

            try
            {
                actualExpectedArgs = String.Join(" ", options.PreParse(CommandLineOptions.GetArgs(commandLine)).ToArray());
            }
            finally
            {
                foreach (var tf in tfs)
                    tf.Dispose();
            }

            Assert.AreEqual(expectedArgs, actualExpectedArgs);
            Assert.AreEqual(options.ErrorMessages, ee);
        }
#endif
        #endregion

        #region General Tests

        [Test]
        public void NoInputFiles()
        {
            var options = new NUnitLiteOptions();
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.InputFiles.Count);
        }

        [TestCase("ShowHelp", "help|h")]
        [TestCase("ShowVersion", "version|V")]
        [TestCase("StopOnError", "stoponerror")]
        [TestCase("WaitBeforeExit", "wait")]
#if !PORTABLE
        [TestCase("NoHeader", "noheader|noh")]
        [TestCase("Full", "full")]
#endif
        [TestCase("TeamCity", "teamcity")]
        public void CanRecognizeBooleanOptions(string propertyName, string pattern)
        {
            Console.WriteLine("Testing " + propertyName);
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(bool), property.PropertyType, "Property '{0}' is wrong type", propertyName);

            NUnitLiteOptions options;
            foreach (string option in prototypes)
            {
                if (option.Length == 1)
                {
                    options = new NUnitLiteOptions("-" + option);
                    Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize -" + option);
                }
                else
                {
                    options = new NUnitLiteOptions("--" + option);
                    Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize --" + option);
                }

                options = new NUnitLiteOptions("/" + option);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize /" + option);
            }
        }

        [TestCase("WhereClause", "where", new string[] { "cat==Fast" }, new string[0])]
        [TestCase("DisplayTestLabels", "labels", new string[] { "Off", "On", "All" }, new string[] { "JUNK" })]
#if !PORTABLE
        [TestCase("OutFile", "output|out", new string[] { "output.txt" }, new string[0])]
        [TestCase("ErrFile", "err", new string[] { "error.txt" }, new string[0])]
        [TestCase("WorkDirectory", "work", new string[] { "results" }, new string[0])]
        [TestCase("InternalTraceLevel", "trace", new string[] { "Off", "Error", "Warning", "Info", "Debug", "Verbose" }, new string[] { "JUNK" })]
#endif
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
                    var options = new NUnitLiteOptions(optionPlusValue);
                    Assert.True(options.Validate(), "Should be valid: " + optionPlusValue);
                    Assert.AreEqual(value, (string)property.GetValue(options, null), "Didn't recognize " + optionPlusValue);
                }

                foreach (string value in badValues)
                {
                    string optionPlusValue = string.Format("--{0}:{1}", option, value);
                    var options = new NUnitLiteOptions(optionPlusValue);
                    Assert.False(options.Validate(), "Should not be valid: " + optionPlusValue);
                }
            }
        }

        [TestCase("DisplayTestLabels", "labels", new string[] { "Off", "On", "All" })]
#if !PORTABLE
        [TestCase("InternalTraceLevel", "trace", new string[] { "Off", "Error", "Warning", "Info", "Debug", "Verbose" })]
#endif
        public void CanRecognizeLowerCaseOptionValues(string propertyName, string optionName, string[] canonicalValues)
        {
            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(string), property.PropertyType);

            foreach (string canonicalValue in canonicalValues)
            {
                string lowercaseValue = canonicalValue.ToLower(CultureInfo.InvariantCulture);
                string optionPlusValue = string.Format("--{0}:{1}", optionName, lowercaseValue);
                var options = new NUnitLiteOptions(optionPlusValue);
                Assert.True(options.Validate(), "Should be valid: " + optionPlusValue);
                Assert.AreEqual(canonicalValue, (string)property.GetValue(options, null), "Didn't recognize " + optionPlusValue);
            }
        }

        [TestCase("DefaultTimeout", "timeout")]
        [TestCase("RandomSeed", "seed")]
#if PARALLEL
        [TestCase("NumberOfTestWorkers", "workers")]
#endif
        public void CanRecognizeIntOptions(string propertyName, string pattern)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(int), property.PropertyType);

            foreach (string option in prototypes)
            {
                var options = new NUnitLiteOptions("--" + option + ":42");
                Assert.AreEqual(42, (int)property.GetValue(options, null), "Didn't recognize --" + option + ":text");
            }
        }

        // [TestCase("InternalTraceLevel", "trace", typeof(InternalTraceLevel))]
        // public void CanRecognizeEnumOptions(string propertyName, string pattern, Type enumType)
        // {
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
        // }

        [TestCase("--where")]
        [TestCase("--timeout")]
#if !PORTABLE
        [TestCase("--output")]
        [TestCase("--err")]
        [TestCase("--work")]
        [TestCase("--trace")]
#endif
        public void MissingValuesAreReported(string option)
        {
            var options = new NUnitLiteOptions(option + "=");
            Assert.False(options.Validate(), "Missing value should not be valid");
            Assert.AreEqual("Missing required value for option '" + option + "'.", options.ErrorMessages[0]);
        }

        [Test]
        public void AssemblyName()
        {
            var options = new NUnitLiteOptions("nunit.tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count);
            Assert.AreEqual("nunit.tests.dll", options.InputFiles[0]);
        }

        // [Test]
        // public void FixtureNamePlusAssemblyIsValid()
        // {
        //    var options = new NUnitLiteOptions( "-fixture:NUnit.Tests.AllTests", "nunit.tests.dll" );
        //    Assert.AreEqual("nunit.tests.dll", options.Parameters[0]);
        //    Assert.AreEqual("NUnit.Tests.AllTests", options.fixture);
        //    Assert.IsTrue(options.Validate());
        // }

        [Test]
        public void AssemblyAloneIsValid()
        {
            var options = new NUnitLiteOptions("nunit.tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.ErrorMessages.Count, "command line should be valid");
        }

        [Test]
        public void InvalidOption()
        {
            var options = new NUnitLiteOptions("-asembly:nunit.tests.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.AreEqual("Invalid argument: -asembly:nunit.tests.dll", options.ErrorMessages[0]);
        }

        // [Test]
        // public void NoFixtureNameProvided()
        // {
        //    ConsoleOptions options = new ConsoleOptions( "-fixture:", "nunit.tests.dll" );
        //    Assert.IsFalse(options.Validate());
        // }

        [Test]
        public void InvalidCommandLineParms()
        {
            var options = new NUnitLiteOptions("-garbage:TestFixture", "-assembly:Tests.dll");
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
            var options = new NUnitLiteOptions("tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual(-1, options.DefaultTimeout);
        }

        [Test]
        public void TimeoutThrowsExceptionIfOptionHasNoValue()
        {
            Assert.Throws<OptionException>(() => new NUnitLiteOptions("tests.dll", "-timeout"));
        }

        [Test]
        public void TimeoutParsesIntValueCorrectly()
        {
            var options = new NUnitLiteOptions("tests.dll", "-timeout:5000");
            Assert.True(options.Validate());
            Assert.AreEqual(5000, options.DefaultTimeout);
        }

        [Test]
        public void TimeoutCausesErrorIfValueIsNotInteger()
        {
            var options = new NUnitLiteOptions("tests.dll", "-timeout:abc");
            Assert.False(options.Validate());
            Assert.AreEqual(-1, options.DefaultTimeout);
        }

        #endregion

        #region EngineResult Option

        [Test]
        public void FileNameWithoutResultOptionLooksLikeParameter()
        {
            var options = new NUnitLiteOptions("tests.dll", "results.xml");
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.ErrorMessages.Count);
            Assert.AreEqual(2, options.InputFiles.Count);
        }

#if !PORTABLE
        [Test]
        public void ResultOptionWithFilePath()
        {
            var options = new NUnitLiteOptions("tests.dll", "-result:results.xml");
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
            var options = new NUnitLiteOptions("tests.dll", "-result:results.xml;format=nunit2");
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
            var options = new NUnitLiteOptions("tests.dll", "-result:results.xml;transform=transform.xslt");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("user", spec.Format);
            Assert.AreEqual("transform.xslt", spec.Transform);
        }

        [Test]
        public void ResultOptionWithoutFileNameIsInvalid()
        {
            var options = new NUnitLiteOptions("tests.dll", "-result:");
            Assert.False(options.Validate(), "Should not be valid");
            Assert.AreEqual(1, options.ErrorMessages.Count, "An error was expected");
        }

        [Test]
        public void ResultOptionMayBeRepeated()
        {
            var options = new NUnitLiteOptions("tests.dll", "-result:results.xml", "-result:nunit2results.xml;format=nunit2", "-result:myresult.xml;transform=mytransform.xslt");
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
            var options = new NUnitLiteOptions("test.dll");
            Assert.AreEqual(1, options.ResultOutputSpecifications.Count);

            var spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("TestResult.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void NoResultSuppressesDefaultResultSpecification()
        {
            var options = new NUnitLiteOptions("test.dll", "-noresult");
            Assert.AreEqual(0, options.ResultOutputSpecifications.Count);
        }

        [Test]
        public void NoResultSuppressesAllResultSpecifications()
        {
            var options = new NUnitLiteOptions("test.dll", "-result:results.xml", "-noresult", "-result:nunit2results.xml;format=nunit2");
            Assert.AreEqual(0, options.ResultOutputSpecifications.Count);
        }
#endif

        #endregion

        #region Explore Option

#if !PORTABLE
        [Test]
        public void ExploreOptionWithoutPath()
        {
            var options = new NUnitLiteOptions("tests.dll", "-explore");
            Assert.True(options.Validate());
            Assert.True(options.Explore);
        }

        [Test]
        public void ExploreOptionWithFilePath()
        {
            var options = new NUnitLiteOptions("tests.dll", "-explore:results.xml");
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
            var options = new NUnitLiteOptions("tests.dll", "-explore:results.xml;format=cases");
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
            var options = new NUnitLiteOptions("tests.dll", "-explore:results.xml;transform=myreport.xslt");
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
            var options = new NUnitLiteOptions("tests.dll", "-explore=C:/nunit/tests/bin/Debug/console-test.xml");
            Assert.True(options.Validate());
            Assert.True(options.Explore);
            Assert.AreEqual(1, options.InputFiles.Count, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.AreEqual("C:/nunit/tests/bin/Debug/console-test.xml", options.ExploreOutputSpecifications[0].OutputPath);
        }

#endif

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

            CommandLineOptions options;
            if (defaultTeamcity.HasValue)
            {
                options = new NUnitLiteOptions(new DefaultOptionsProviderStub(defaultTeamcity.Value), args.ToArray());
            }
            else
            {
                options = new NUnitLiteOptions(args.ToArray());
            }

            // When
            var actualTeamCity = options.TeamCity;

            // Then
            Assert.AreEqual(actualTeamCity, expectedTeamCity);
        }

        #endregion

        #region Test Parameters

        [Test]
        public void SingleTestParameter()
        {
            var options = new NUnitLiteOptions("--params=X=5");
            Assert.That(options.errorMessages, Is.Empty);
            Assert.That(options.TestParameters, Is.EqualTo("X=5"));
        }

        [Test]
        public void TwoTestParametersInOneOption()
        {
            var options = new NUnitLiteOptions("--params:X=5;Y=7");
            Assert.That(options.errorMessages, Is.Empty);
            Assert.That(options.TestParameters, Is.EqualTo("X=5;Y=7"));
        }

        [Test]
        public void TwoTestParametersInSeparateOptions()
        {
            var options = new NUnitLiteOptions("-p:X=5", "-p:Y=7");
            Assert.That(options.errorMessages, Is.Empty);
            Assert.That(options.TestParameters, Is.EqualTo("X=5;Y=7"));
        }

        [Test]
        public void ThreeTestParametersInTwoOptions()
        {
            var options = new NUnitLiteOptions("--params:X=5;Y=7", "-p:Z=3");
            Assert.That(options.errorMessages, Is.Empty);
            Assert.That(options.TestParameters, Is.EqualTo("X=5;Y=7;Z=3"));
        }

        [Test]
        public void ParameterWithoutEqualSignIsInvalid()
        {
            var options = new NUnitLiteOptions("--params=X5");
            Assert.That(options.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void DisplayTestParameters()
        {
            if (TestContext.Parameters.Count == 0)
            {
                Console.WriteLine("No Test Parameters were passed");
                return;
            }

            Console.WriteLine("Test Parameters---");

            foreach (var name in TestContext.Parameters.Names)
                Console.WriteLine("   Name: {0} Value: {1}", name, TestContext.Parameters[name]);
        }

        #endregion

        #region Helper Methods

        //private static FieldInfo GetFieldInfo(string fieldName)
        //{
        //    FieldInfo field = typeof(NUnitLiteOptions).GetField(fieldName);
        //    Assert.IsNotNull(field, "The field '{0}' is not defined", fieldName);
        //    return field;
        //}

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            PropertyInfo property = typeof(NUnitLiteOptions).GetProperty(propertyName);
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
