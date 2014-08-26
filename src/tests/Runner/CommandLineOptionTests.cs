// *****************************************************
// Copyright 2007, Charlie Poole
//
// Licensed under the Open Software License version 3.0
// *****************************************************

using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Env = NUnit.Env;
using System.Reflection;

namespace NUnitLite.Runner.Tests
{
    [TestFixture]
    class CommandLineOptionTests
    {
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

#if !SILVERLIGHT && !NETCF
        [Test]
        public void TestExploreOptionWithNoFileName()
        {
            var options = new CommandLineOptions("-explore");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.Explore, Is.True);
            Assert.That(options.ExploreFile, Is.EqualTo(Path.GetFullPath("tests.xml")));
        }

        [Test]
        public void TestExploreOptionWithGoodFileName()
        {
            var options = new CommandLineOptions("-explore=MyFile.xml");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.Explore, Is.True);
            Assert.That(options.ExploreFile, Is.EqualTo(Path.GetFullPath("MyFile.xml")));
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "No Exception thrown on bad path under Mono. Test should be revised.")]
        public void TestExploreOptionWithBadFileName()
        {
            var options = new CommandLineOptions("-explore=MyFile*.xml");
            Assert.That(options.ErrorMessages, Is.EqualTo(
                new string[] { "Invalid option: -explore=MyFile*.xml" } ));
        }

        [Test]
        public void TestResultOptionWithNoFileName()
        {
            var options = new CommandLineOptions("-result");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.ResultFile, Is.EqualTo(Path.GetFullPath("TestResult.xml")));
        }

        [Test]
        public void TestResultOptionWithGoodFileName()
        {
            var options = new CommandLineOptions("-result=MyResult.xml");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.ResultFile, Is.EqualTo(Path.GetFullPath("MyResult.xml")));
        }

        [Test]
        [Platform(Exclude = "Mono", Reason = "No Exception thrown on bad path under Mono. Test should be revised.")]
        public void TestResultOptionWithBadFileName()
        {
            var options = new CommandLineOptions("-result=MyResult*.xml");
            Assert.That(options.ErrorMessages, 
                Is.EqualTo(new string[] { "Invalid option: -result=MyResult*.xml" } ));
        }
#endif

        [Test]
        public void TestNUnit2FormatOption()
        {
            var options = new CommandLineOptions("-format=nunit2");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.ResultFormat, Is.EqualTo("nunit2"));
        }

        [Test]
        public void TestNUnit3FormatOption()
        {
            var options = new CommandLineOptions("-format=nunit3");
            Assert.That(options.ErrorMessages, Is.Empty);
            Assert.That(options.ResultFormat, Is.EqualTo("nunit3"));
        }

        [Test]
        public void TestBadFormatOption()
        {
            var options = new CommandLineOptions("-format=xyz");
            Assert.That(options.ErrorMessages, Is.EqualTo(
                new string[] { "Invalid option: -format=xyz" } ));
        }

        [Test]
        public void TestMissingFormatOption()
        {
            var options = new CommandLineOptions("-format");
            Assert.That(options.ErrorMessages, Is.EqualTo(
                new string[] { "Invalid option: -format" } ));
        }

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
