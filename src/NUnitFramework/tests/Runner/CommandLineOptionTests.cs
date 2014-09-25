// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.IO;
using System.Reflection;
using NUnit.Common;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Env = NUnit.Env;

namespace NUnitLite.Runner.Tests
{
    [TestFixture]
    class CommandLineOptionTests
    {
        #region General Tests

        [TestCase("InternalTraceLevel", "Off")]
        [TestCase("DefaultTimeout", -1)]
        public void TestDefaultSetting<T>(string propertyName, T defaultValue)
        {
            var options = new CommandLineOptions();
            Assert.That(options.ErrorMessages, Is.Empty);
            CheckProperty(options, propertyName, defaultValue);
        }

        [TestCase("ShowHelp", "help|h")]
        [TestCase("Wait", "wait")]
        [TestCase("NoHeader", "noheader|noh")]
        [TestCase("Full", "full")]
        [TestCase("DisplayTeamCityServiceMessages", "teamcity")]
        public void CanRecognizeBooleanOptions(string propertyName, string pattern)
        {
            string[] prototypes = pattern.Split('|');

            PropertyInfo property = GetPropertyInfo(propertyName);
            Assert.AreEqual(typeof(bool), property.PropertyType, "Property '{0}' is wrong type", propertyName);

            foreach (string option in prototypes)
            {
                var options = new CommandLineOptions("-" + option);
                Assert.That(options.ErrorMessages, Is.Empty);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize -" + option);

                options = new CommandLineOptions("--" + option);
                Assert.That(options.ErrorMessages, Is.Empty);
                Assert.AreEqual(true, (bool)property.GetValue(options, null), "Didn't recognize --" + option);

                options = new CommandLineOptions("/" + option);
                Assert.That(options.ErrorMessages, Is.Empty);
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
        //[TestCase("V2ResultFile", "xml2", new string[] { "v2.xml" }, new string[0])]
        //[TestCase("V3ResultFile", "xml3", new string[] { "v3.xml" }, new string[0])]
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
                    Assert.That(options.ErrorMessages, Is.Empty, "Should be valid: " + optionPlusValue);
                    Assert.AreEqual(value, (string)property.GetValue(options, null), "Didn't recognize " + optionPlusValue);
                    CheckProperty(options, propertyName, value);
                }

                foreach (string value in badValues)
                {
                    string optionPlusValue = string.Format("--{0}:{1}", option, value);
                    CommandLineOptions options = new CommandLineOptions(optionPlusValue);
                    Assert.That(options.ErrorMessages, Is.EqualTo(new string[] { 
                        string.Format("The value '{0}' is not valid for option '{1}'.", value, optionPlusValue) } ));
                }
            }
        }

        [TestCase("DefaultTimeout", "timeout", typeof(int))]
        [TestCase("InitialSeed", "seed", typeof(int))]
        //[TestCase("NumWorkers", "workers", typeof(int?))]
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

        #endregion

        #region Explore Option Tests

#if !SILVERLIGHT && !NETCF
        [Test]
        public void ExploreOptionWithNoFileName()
        {
            var options = new CommandLineOptions("-explore");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.Explore, Is.True);
            Assert.That(options.ExploreOutputSpecifications.Count, Is.EqualTo(0));
        }

        [Test]
        public void ExploreOptionWithFileName()
        {
            var options = new CommandLineOptions("-explore=MyFile.xml");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.Explore, Is.True);

            var spec = options.ExploreOutputSpecifications[0];
            Assert.That(spec.OutputPath, Is.EqualTo("MyFile.xml"));
            Assert.That(spec.Format, Is.EqualTo("nunit3"));
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ExploreOptionWithFilePathAndFormat()
        {
            var options = new CommandLineOptions("-explore:results.xml;format=cases");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.That(spec.OutputPath, Is.EqualTo("results.xml"));
            Assert.That(spec.Format, Is.EqualTo("cases"));
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ExploreOptionWithFilePathAndTransform()
        {
            var options = new CommandLineOptions("-explore:results.xml;transform=myreport.xslt");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.True(options.Explore);

            OutputSpecification spec = options.ExploreOutputSpecifications[0];
            Assert.That(spec.OutputPath, Is.EqualTo("results.xml"));
            Assert.That(spec.Format, Is.EqualTo("user"));
            Assert.That(spec.Transform, Is.EqualTo("myreport.xslt"));
        }

        [Test]
        public void ExploreOptionMayBeRepeated()
        {
            var options = new CommandLineOptions("-explore:results.xml", "-explore:nunit2results.xml;format=nunit2", "-explore:myresult.xml;transform=mytransform.xslt");
            Assert.That(options.ErrorMessages, Is.Empty);

            var specs = options.ExploreOutputSpecifications;
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
#endif

        #endregion

        #region Result Option Tests

#if !SILVERLIGHT && !NETCF
        [Test]
        public void ResultOptionWithNoFileNameDefaultsToTestResultXml()
        {
            var options = new CommandLineOptions("-result");
            Assert.That(options.ErrorMessages, Is.Empty);

            var spec = options.ResultOutputSpecifications[0];
            Assert.That(spec.OutputPath, Is.EqualTo("TestResult.xml"));
            Assert.That(spec.Format, Is.EqualTo("nunit3"));
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ResultOptionWithFileName()
        {
            var options = new CommandLineOptions("-result=MyResult.xml");
            Assert.That(options.ErrorMessages, Is.Empty);

            var spec = options.ResultOutputSpecifications[0];
            Assert.That(spec.OutputPath, Is.EqualTo("MyResult.xml"));
            Assert.That(spec.Format, Is.EqualTo("nunit3"));
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ResultOptionWithFilePathAndFormat()
        {
            var options = new CommandLineOptions("-result:results.xml;format=nunit2");
            Assert.That(options.ErrorMessages, Is.Empty);

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("nunit2", spec.Format);
            Assert.Null(spec.Transform);
        }

        [Test]
        public void ResultOptionWithFilePathAndTransform()
        {
            var options = new CommandLineOptions("-result:results.xml;transform=transform.xslt");
            Assert.That(options.ErrorMessages, Is.Empty);

            OutputSpecification spec = options.ResultOutputSpecifications[0];
            Assert.AreEqual("results.xml", spec.OutputPath);
            Assert.AreEqual("user", spec.Format);
            Assert.AreEqual("transform.xslt", spec.Transform);
        }

        [Test]
        public void ResultOptionMayBeRepeated()
        {
            var options = new CommandLineOptions("-result:results.xml", "-result:nunit2results.xml;format=nunit2", "-result:myresult.xml;transform=mytransform.xslt");
            Assert.That(options.ErrorMessages, Is.Empty);

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
#endif

        #endregion

        #region Invalid Options

        [Test]
        public void InvalidOptionProducesError()
        {
            var options = new CommandLineOptions("-junk");
            Assert.That(options.ErrorMessages, Is.EqualTo(
                new string[] { "Invalid option: -junk" } ));
        }

        [Test]
        public void MultipleInvalidOptionsAreListedInErrorMessage()
        {
            var options = new CommandLineOptions("-junk", "-trash", "something", "-garbage");
            Assert.That(options.ErrorMessages, Is.EqualTo(
                new string[] {
                    "Invalid option: -junk",
                    "Invalid option: -trash",
                    "Invalid option: -garbage"} ));
        }

        #endregion

        #region Input Files

        [Test]
        public void SingleInputFileIsSaved()
        {
            var options = new CommandLineOptions("myassembly.dll");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.InputFiles, Is.EqualTo(
                new string[] { "myassembly.dll" } ));
        }

        [Test]
        public void MultipleInputFilesAreSaved()
        {
            var options = new CommandLineOptions("assembly1.dll", "-wait", "assembly2.dll", "assembly3.dll");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.InputFiles, Is.EqualTo(
                new string[] { "assembly1.dll", "assembly2.dll", "assembly3.dll" } ));
        }

        #endregion

        #region Test Option

        [Test]
        public void TestOptionIsRecognized()
        {
            var options = new CommandLineOptions("-test:Some.Class.Name");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.Tests, Is.EqualTo(
                new string[] { "Some.Class.Name" } ));
        }

        [Test]
        public void MultipleTestNamesAreRecognized()
        {
            var options = new CommandLineOptions("-test:NUnit.Tests.Test1,NUnit.Tests.Test2,NUnit.Tests.Test3");
            Assert.That(options.Tests, Is.EqualTo(
                new string[] { "NUnit.Tests.Test1", "NUnit.Tests.Test2", "NUnit.Tests.Test3" }));
        }

        [Test]
        public void MultipleTestOptionsAreRecognized()
        {
            var options = new CommandLineOptions("-test:Class1", "-test=Class2", "-test:Class3");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.Tests, Is.EqualTo(
                new string[] { "Class1", "Class2", "Class3" } ));
        }

        #endregion

        #region Helper Methods

        private static PropertyInfo GetPropertyInfo(string propertyName)
        {
            PropertyInfo property = typeof(CommandLineOptions).GetProperty(propertyName);
            Assert.IsNotNull(property, "The property '{0}' is not defined", propertyName);
            return property;
        }

        private static void CheckProperty<T>(CommandLineOptions options, string propertyName, T expectedValue)
        {
            var property = GetPropertyInfo(propertyName);
            Assert.That(property.PropertyType, Is.EqualTo(typeof(T)));
            Assert.That(property.GetValue(options, null), Is.EqualTo(expectedValue));
        }

        #endregion
    }
}
