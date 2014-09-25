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
using System.IO;
using System.Reflection;
using System.Text;

namespace NUnit.ConsoleRunner.Tests
{
    using Common;
    using Engine;
    using Framework;
    using Options;

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

        //[Test]
        //public void AllowForwardSlashDefaultsCorrectly()
        //{
        //    ConsoleOptions options = new ConsoleOptions();
        //    Assert.AreEqual( Path.DirectorySeparatorChar != '/', options.AllowForwardSlash );
        //}

        [TestCase("ShowHelp", "help|h")]
        [TestCase("StopOnError", "stoponerror")]
        [TestCase("WaitBeforeExit", "wait")]
        [TestCase("PauseBeforeRun", "pause")]
        [TestCase("NoHeader", "noheader|noh")]
        public void CanRecognizeBooleanOptions(string propertyName, string pattern)
        {
            Console.WriteLine("Testing " + propertyName);
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(bool), property.PropertyType, "Property '{0}' is wrong type", propertyName);

            foreach (string option in prototypes)
            {
                ConsoleOptions options = new ConsoleOptions("-" + option);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize -" + option);

                options = new ConsoleOptions("-" + option + "+");
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize -" + option + "+");

                options = new ConsoleOptions("-" + option + "-");
                Assert.AreEqual(false, (bool)property.GetValue(options, null), "Didn't recognize -" + option + "-");

                options = new ConsoleOptions("--" + option);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize --" + option);

                options = new ConsoleOptions("/" + option);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize /" + option);
            }
        }

        [TestCase("Include",            "include",    new string[] { "Short,Fast" },                     new string[0])]
        [TestCase("Exclude",            "exclude",    new string[] { "Long" },                           new string[0])]
        [TestCase("ActiveConfig",       "config",     new string[] { "Debug" },                          new string[0])]
        [TestCase("ProcessModel",       "process",    new string[] { "Single", "Separate", "Multiple" }, new string[] { "JUNK" })]
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

        [TestCase("DefaultTimeout", "timeout")]
        [TestCase("RandomSeed", "seed")]
        [TestCase("NumWorkers", "workers")]
        public void CanRecognizeIntOptions(string propertyName, string pattern)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(int), property.PropertyType);

            foreach (string option in prototypes)
            {
                ConsoleOptions options = new ConsoleOptions("--" + option + ":42");
                Assert.AreEqual(42, (int)property.GetValue(options, null), "Didn't recognize --" + option + ":text");
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

        [TestCase("--include")]
        [TestCase("--exclude")]
        [TestCase("--config")]
        [TestCase("--process")]
        [TestCase("--domain")]
        [TestCase("--framework")]
        [TestCase("--timeout")]
        //[TestCase("--xml")]
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
        public void InvalidOption()
        {
            ConsoleOptions options = new ConsoleOptions("-asembly:nunit.tests.dll");
            Assert.False(options.Validate());
            Assert.AreEqual(1, options.ErrorMessages.Count);
            Assert.AreEqual("Invalid argument: -asembly:nunit.tests.dll", options.ErrorMessages[0]);
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
            Assert.Throws<Mono.Options.OptionException>(() => new ConsoleOptions("tests.dll", "-timeout"));
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
    }
}
