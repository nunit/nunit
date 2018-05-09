// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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
        #region Argument PreProcessor Tests

        [TestCase("--arg", "--arg")]
        [TestCase("--ArG", "--ArG")]
        [TestCase("--arg1 --arg2", "--arg1", "--arg2")]
        [TestCase("--arg1 data --arg2", "--arg1", "data", "--arg2")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("\"--arg 1\" --arg2", "--arg 1", "--arg2")]
        [TestCase("--arg1 \"--arg 2\"", "--arg1", "--arg 2")]
        [TestCase("\"--arg 1\" \"--arg 2\"", "--arg 1", "--arg 2")]
        [TestCase("\"--arg 1\" \"--arg 2\" arg3 \"arg 4\"", "--arg 1", "--arg 2", "arg3", "arg 4")]
        [TestCase("--arg1 \"--arg 2\" arg3 \"arg 4\"", "--arg1", "--arg 2", "arg3", "arg 4")]
        [TestCase("\"--arg 1\" \"--arg 2\" arg3 \"arg 4\" \"--arg 1\" \"--arg 2\" arg3 \"arg 4\"",
            "--arg 1", "--arg 2", "arg3", "arg 4", "--arg 1", "--arg 2", "arg3", "arg 4")]
        [TestCase("\"--arg\"", "--arg")]
        [TestCase("\"--arg 1\"", "--arg 1")]
        [TestCase("\"--arg abc\"", "--arg abc")]
        [TestCase("\"--arg   abc\"", "--arg   abc")]
        [TestCase("\" --arg   abc \"", " --arg   abc ")]
        [TestCase("\"--arg=abc\"", "--arg=abc")]
        [TestCase("\"--arg=aBc\"", "--arg=aBc")]
        [TestCase("\"--arg = abc\"", "--arg = abc")]
        [TestCase("\"--arg=abc,xyz\"", "--arg=abc,xyz")]
        [TestCase("\"--arg=abc, xyz\"", "--arg=abc, xyz")]
        [TestCase("\"@arg = ~ ` ! @ # $ % ^ & * ( ) _ - : ; + ' ' { } [ ] | \\ ? / . , , xYz\"",
            "@arg = ~ ` ! @ # $ % ^ & * ( ) _ - : ; + ' ' { } [ ] | \\ ? / . , , xYz")]
        public void GetArgsFromCommandLine(string cmdline, params string[] expectedArgs)
        {
            var actualArgs = CommandLineOptions.GetArgs(cmdline);

            Assert.AreEqual(expectedArgs, actualArgs);
        }

        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--filearg1 --filearg2", "--arg1", "--filearg1", "--filearg2", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--filearg1\r\n--filearg2", "--arg1", "--filearg1", "--filearg2", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--filearg1 data", "--arg1", "--filearg1", "data", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--filearg1 \"data in quotes\"", "--arg1", "--filearg1", "data in quotes", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--filearg1 \"data in quotes with 'single' quotes\"", "--arg1", "--filearg1", "data in quotes with 'single' quotes", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--filearg1 \"data in quotes with /slashes/\"", "--arg1", "--filearg1", "data in quotes with /slashes/", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2 @file2.txt", "file1.txt:--filearg1 --filearg2,file2.txt:--filearg3", "--arg1", "--filearg1", "--filearg2", "--arg2", "--filearg3")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:", "--arg1", "--arg2")]
        // Blank lines
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--fileArg1\n\n\n--fileArg2", "--arg1", "--fileArg1", "--fileArg2", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--fileArg1\n    \n\t\t\n--fileArg2", "--arg1", "--fileArg1", "--fileArg2", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--fileArg1\r\n\r\n\r\n--fileArg2", "--arg1", "--fileArg1", "--fileArg2", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--fileArg1\r\n    \r\n\t\t\r\n--fileArg2", "--arg1", "--fileArg1", "--fileArg2", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--filearg1 --filearg2\r\n\n--filearg3 --filearg4", "--arg1", "--filearg1", "--filearg2", "--filearg3", "--filearg4", "--arg2")]

        // Comments
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--fileArg1\nThis is NOT treated as a COMMENT\n--fileArg2", "--arg1", "--fileArg1", "This", "is", "NOT", "treated", "as", "a", "COMMENT", "--fileArg2", "--arg2")]
        [TestCase("--arg1 @file1.txt --arg2", "file1.txt:--fileArg1\n#This is treated as a COMMENT\n--fileArg2", "--arg1", "--fileArg1", "--fileArg2", "--arg2")]
        // Nested files
        [TestCase("--arg1 @file1.txt --arg2 @file2.txt", "file1.txt:--filearg1 --filearg2,file2.txt:--filearg3 @file3.txt,file3.txt:--filearg4", "--arg1", "--filearg1", "--filearg2", "--arg2", "--filearg3", "--filearg4")]
        // Where clauses
        [TestCase("testfile.dll @file1.txt --arg2", "file1.txt:--where test==somelongname", "testfile.dll", "--where", "test==somelongname", "--arg2")]
        // NOTE: The next is not valid. Where clause is spread over several args and therefore won't parse. Quotes are required.
        [TestCase("testfile.dll @file1.txt --arg2", "file1.txt:--where test == somelongname", "testfile.dll", "--where", "test", "==", "somelongname", "--arg2")]
        [TestCase("testfile.dll @file1.txt --arg2",
            "file1.txt:--where \"test == somelongname\"",
            "testfile.dll", "--where", "test == somelongname", "--arg2")]
        [TestCase("testfile.dll @file1.txt --arg2",
            "file1.txt:--where\n    \"test == somelongname\"",
            "testfile.dll", "--where", "test == somelongname", "--arg2")]
        [TestCase("testfile.dll @file1.txt --arg2",
            "file1.txt:--where\n    \"test == somelongname or test == /another long name/ or cat == SomeCategory\"",
            "testfile.dll", "--where", "test == somelongname or test == /another long name/ or cat == SomeCategory", "--arg2")]
        [TestCase("testfile.dll @file1.txt --arg2",
            "file1.txt:--where\n    \"test == somelongname or\ntest == /another long name/ or\ncat == SomeCategory\"",
            "testfile.dll", "--where", "test == somelongname or test == /another long name/ or cat == SomeCategory", "--arg2")]
        [TestCase("testfile.dll @file1.txt --arg2",
            "file1.txt:--where\n    \"test == somelongname ||\ntest == /another long name/ ||\ncat == SomeCategory\"",
            "testfile.dll", "--where", "test == somelongname || test == /another long name/ || cat == SomeCategory", "--arg2")]
        public void GetArgsFromFiles(string commandLine, string files, params string[] expectedArgs)
        {
            var filespecs = files.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var testFiles = new TestFile[filespecs.Length];

            for (int ix = 0; ix < filespecs.Length; ++ix)
            {
                var filespec = filespecs[ix];
                var split = filespec.IndexOf( ':' );
                if (split < 0) throw new Exception("Invalid test data");

                var fileName = filespec.Substring(0, split);
                var fileContent = filespec.Substring(split + 1);

                testFiles[ix] = new TestFile(Path.Combine(TestContext.CurrentContext.TestDirectory, fileName), fileContent, true);
            }

            var options = new NUnitLiteOptions();

            string[] expandedArgs;

            try
            {
                expandedArgs = options.PreParse(CommandLineOptions.GetArgs(commandLine)).ToArray();
            }
            finally
            {
                foreach (var tf in testFiles)
                    tf.Dispose();
            }

            Assert.AreEqual(expectedArgs, expandedArgs);
            Assert.Zero(options.ErrorMessages.Count);
        }

        [TestCase("--arg1 @file1.txt --arg2", "The file \"file1.txt\" was not found.")]
        [TestCase("--arg1 @ --arg2", "You must include a file name after @.")]
        public void GetArgsFromFiles_FailureTests(string args, string errorMessage)
        {
            var options = new NUnitLiteOptions();

            options.PreParse(CommandLineOptions.GetArgs(args));

            Assert.That(options.ErrorMessages, Is.EqualTo(new object[] { errorMessage }));
        }

        //[Test]
        public void GetArgsFromFiles_NestingOverflow()
        {
            var options = new NUnitLiteOptions();
            var args = new[] { "--arg1", "@file1.txt", "--arg2" };
            var expectedErrors = new string[] { "@ nesting exceeds maximum depth of 3." };

            using (new TestFile(Path.Combine(TestContext.CurrentContext.TestDirectory, "file1.txt"), "@file1.txt", true))
            {
                var expandedArgs = options.PreParse(args);

                Assert.AreEqual(args, expandedArgs);
                Assert.AreEqual(expectedErrors, options.ErrorMessages);
            }
        }
        #endregion

        #region General Tests

        [Test]
        public void NoInputFiles()
        {
            var options = new NUnitLiteOptions();
            Assert.True(options.Validate());
            Assert.Null(options.InputFile);
        }

        [TestCase("ShowHelp", "help|h")]
        [TestCase("ShowVersion", "version|V")]
        [TestCase("StopOnError", "stoponerror")]
        [TestCase("WaitBeforeExit", "wait")]
        [TestCase("NoHeader", "noheader|noh")]
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
        [TestCase("DisplayTestLabels", "labels", new string[] { "Off", "On", "Before", "After", "All" }, new string[] { "JUNK" })]
        [TestCase("OutFile", "output|out", new string[] { "output.txt" }, new string[0])]
        [TestCase("ErrFile", "err", new string[] { "error.txt" }, new string[0])]
        [TestCase("WorkDirectory", "work", new string[] { "results" }, new string[0])]
        [TestCase("InternalTraceLevel", "trace", new string[] { "Off", "Error", "Warning", "Info", "Debug", "Verbose" }, new string[] { "JUNK" })]
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
        [TestCase("InternalTraceLevel", "trace", new string[] { "Off", "Error", "Warning", "Info", "Debug", "Verbose" })]
        public void CanRecognizeLowerCaseOptionValues(string propertyName, string optionName, string[] canonicalValues)
        {
            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(string), property.PropertyType);

            foreach (string canonicalValue in canonicalValues)
            {
                string lowercaseValue = canonicalValue.ToLowerInvariant();
                string optionPlusValue = string.Format("--{0}:{1}", optionName, lowercaseValue);
                var options = new NUnitLiteOptions(optionPlusValue);
                Assert.True(options.Validate(), "Should be valid: " + optionPlusValue);
                Assert.AreEqual(canonicalValue, (string)property.GetValue(options, null), "Didn't recognize " + optionPlusValue);
            }
        }
#if !NETCOREAPP1_1
        [TestCase("DefaultTimeout", "timeout")]
#endif
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
        [TestCase("--output")]
        [TestCase("--err")]
        [TestCase("--work")]
        [TestCase("--trace")]
#if !NETCOREAPP1_1
        [TestCase("--timeout")]
#endif
        public void MissingValuesAreReported(string option)
        {
            var options = new NUnitLiteOptions(option + "=");
            Assert.False(options.Validate(), "Missing value should not be valid");
            Assert.AreEqual("Missing required value for option '" + option + "'.", options.ErrorMessages[0]);
        }

        [Test]
        public void AssemblyIsInvalidByDefault()
        {
            var options = new NUnitLiteOptions("nunit.tests.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.That(options.ErrorMessages[0], Contains.Substring("Invalid entry: nunit.tests.dll"));
        }

        [Test]
        public void MultipleAssembliesAreInvalidByDefault()
        {
            var options = new NUnitLiteOptions("nunit.tests.dll", "another.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(2, options.ErrorMessages.Count);
            Assert.That(options.ErrorMessages[0], Contains.Substring("Invalid entry: nunit.tests.dll"));
            Assert.That(options.ErrorMessages[1], Contains.Substring("Invalid entry: another.dll"));
        }

        [Test]
        public void AssemblyIsValidIfAllowed()
        {
            var options = new NUnitLiteOptions(true, "nunit.tests.dll");
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.ErrorMessages.Count);
        }

        [Test]
        public void MultipleAssembliesAreInvalidEvenIfOneIsAllowed()
        {
            var options = new NUnitLiteOptions(true, "nunit.tests.dll", "another.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.That(options.ErrorMessages[0], Contains.Substring("Invalid entry: another.dll"));
        }

        [Test]
        public void InvalidOption()
        {
            var options = new NUnitLiteOptions("-asembly:nunit.tests.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.AreEqual("Invalid argument: -asembly:nunit.tests.dll", options.ErrorMessages[0]);
        }

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
            var options = new NUnitLiteOptions();
            Assert.True(options.Validate());
            Assert.AreEqual(-1, options.DefaultTimeout);
        }
#if !NETCOREAPP1_1
        [Test]
        public void TimeoutThrowsExceptionIfOptionHasNoValue()
        {
            Assert.Throws<OptionException>(() => new NUnitLiteOptions("-timeout"));
        }

        [Test]
        public void TimeoutParsesIntValueCorrectly()
        {
            var options = new NUnitLiteOptions("-timeout:5000");
            Assert.True(options.Validate());
            Assert.AreEqual(5000, options.DefaultTimeout);
        }

        [Test]
        public void TimeoutCausesErrorIfValueIsNotInteger()
        {
            var options = new NUnitLiteOptions("-timeout:abc");
            Assert.False(options.Validate());
            Assert.AreEqual(-1, options.DefaultTimeout);
        }
#endif

        #endregion

        #region EngineResult Option

        [Test]
        public void FileNameWithoutResultOptionLooksLikeParameter()
        {
            var options = new NUnitLiteOptions(true, "results.xml");
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.ErrorMessages.Count);
            //Assert.That(options.ErrorMessages[0], Contains.Substring("Invalid entry: results.xml"));
            Assert.AreEqual("results.xml", options.InputFile);
        }

        [Test]
        public void ResultOptionWithFilePath()
        {
            var options = new NUnitLiteOptions("-result:results.xml");
            Assert.True(options.Validate());

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
        }

        [Test]
        public void ResultOptionWithFilePathAndFormat()
        {
            var options = new NUnitLiteOptions("-result:results.xml;format=nunit2");
            Assert.True(options.Validate());

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit2", spec.Format);
        }

        [Test]
        public void ResultOptionWithoutFileNameIsInvalid()
        {
            var options = new NUnitLiteOptions("-result:");
            Assert.False(options.Validate(), "Should not be valid");
            Assert.AreEqual(1, options.ErrorMessages.Count, "An error was expected");
        }

        [Test]
        public void ResultOptionMayBeRepeated()
        {
            var options = new NUnitLiteOptions("-result:results.xml", "-result:nunit2results.xml;format=nunit2");
            Assert.True(options.Validate(), "Should be valid");

            var specs = options.ResultOutputSpecifications;
            Assert.That(specs, Has.Count.EqualTo(2));

            var spec1 = specs[0];
            Assert.AreEqual("results.xml", spec1.OutputPath);
            Assert.AreEqual("nunit3", spec1.Format);

            var spec2 = specs[1];
            Assert.AreEqual("nunit2results.xml", spec2.OutputPath);
            Assert.AreEqual("nunit2", spec2.Format);
        }

        [Test]
        public void DefaultResultSpecification()
        {
            var options = new NUnitLiteOptions();
            Assert.AreEqual(1, options.ResultOutputSpecifications.Count);

            var spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("TestResult.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
        }

        [Test]
        public void NoResultSuppressesDefaultResultSpecification()
        {
            var options = new NUnitLiteOptions("-noresult");
            Assert.AreEqual(0, options.ResultOutputSpecifications.Count);
        }

        [Test]
        public void NoResultSuppressesAllResultSpecifications()
        {
            var options = new NUnitLiteOptions("-result:results.xml", "-noresult", "-result:nunit2results.xml;format=nunit2");
            Assert.AreEqual(0, options.ResultOutputSpecifications.Count);
        }

        [Test]
        public void InvalidResultSpecRecordsError()
        {
            var options = new NUnitLiteOptions("test.dll", "-result:userspecifed.xml;format=nunit2;format=nunit3");
            Assert.That(options.ResultOutputSpecifications, Has.Exactly(1).Items
                    .And.Exactly(1).Property(nameof(OutputSpecification.OutputPath)).EqualTo("TestResult.xml"));
            Assert.That(options.ErrorMessages, Has.Exactly(1).Contains("invalid output spec").IgnoreCase);
        }

        #endregion

        #region Explore Option

        [Test]
        public void ExploreOptionWithoutPath()
        {
            var options = new NUnitLiteOptions("-explore");
            Assert.True(options.Validate());
            Assert.True(options.Explore);
        }

        [Test]
        public void ExploreOptionWithFilePath()
        {
            var options = new NUnitLiteOptions("-explore:results.xml");
            Assert.True(options.Validate());
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit3", spec.Format);
        }

        [Test]
        public void ExploreOptionWithFilePathAndFormat()
        {
            var options = new NUnitLiteOptions("-explore:results.xml;format=cases");
            Assert.True(options.Validate());
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("cases", spec.Format);
        }

        [Test]
        public void ExploreOptionWithFilePathUsingEqualSign()
        {
            var options = new NUnitLiteOptions("-explore=C:/nunit/tests/bin/Debug/console-test.xml");
            Assert.True(options.Validate());
            Assert.True(options.Explore);
            Assert.AreEqual("C:/nunit/tests/bin/Debug/console-test.xml", options.ExploreOutputSpecifications[0].OutputPath);
        }

        [TestCase(true, null, true)]
        [TestCase(false, null, false)]
        [TestCase(true, false, true)]
        [TestCase(false, false, false)]
        [TestCase(true, true, true)]
        [TestCase(false, true, true)]
        public void ShouldSetTeamCityFlagAccordingToArgsAndDefaults(bool hasTeamcityInCmd, bool? defaultTeamcity, bool expectedTeamCity)
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
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.TestParameters, Is.EqualTo(new Dictionary<string, string> { {"X", "5" } }));
        }

        [Test]
        public void TwoTestParametersInOneOption()
        {
            var options = new NUnitLiteOptions("--params:X=5;Y=7");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.TestParameters, Is.EqualTo(new Dictionary<string, string> { { "X", "5" }, { "Y", "7" } }));
        }

        [Test]
        public void TwoTestParametersInSeparateOptions()
        {
            var options = new NUnitLiteOptions("-p:X=5", "-p:Y=7");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.TestParameters, Is.EqualTo(new Dictionary<string, string> { { "X", "5" }, { "Y", "7" } }));
        }

        [Test]
        public void ThreeTestParametersInTwoOptions()
        {
            var options = new NUnitLiteOptions("--params:X=5;Y=7", "-p:Z=3");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.TestParameters, Is.EqualTo(new Dictionary<string, string> { { "X", "5" }, { "Y", "7" }, { "Z", "3" } }));
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

            public bool TeamCity { get; }
        }
    }
}
