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
using NUnit.Engine;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
	[TestFixture]
	public class CommandLineTests
	{
        [Test]
        public void NoInputFiles()
        {
            ConsoleOptions options = new ConsoleOptions();
            Assert.True(options.Validate());
            Assert.AreEqual(0, options.InputFiles.Length);
        }

        //[Test]
        //public void AllowForwardSlashDefaultsCorrectly()
        //{
        //    ConsoleOptions options = new ConsoleOptions();
        //    Assert.AreEqual( Path.DirectorySeparatorChar != '/', options.AllowForwardSlash );
        //}

        [TestCase("noheader", "noheader|noh")]
        [TestCase("help", "help|h")]
		[TestCase("wait", "wait")]
        [TestCase("labels", "labels")]
        [TestCase("noresult", "noresult")]
		public void CanRecognizeBooleanOptions(string fieldName, string pattern)
		{
            string[] prototypes = pattern.Split('|');

            FieldInfo field = GetFieldInfo(fieldName);
            Assert.AreEqual(typeof(bool), field.FieldType, "Field '{0}' is wrong type", fieldName);

            foreach (string option in prototypes)
            {
                ConsoleOptions options = new ConsoleOptions("-" + option);
                Assert.AreEqual(true, (bool)field.GetValue(options), "Didn't recognize -" + option);

                options = new ConsoleOptions("-" + option + "+");
                Assert.AreEqual(true, (bool)field.GetValue(options), "Didn't recognize -" + option + "+");

                options = new ConsoleOptions("-" + option + "-");
                Assert.AreEqual(false, (bool)field.GetValue(options), "Didn't recognize -" + option + "-");

                options = new ConsoleOptions("--" + option);
                Assert.AreEqual(true, (bool)field.GetValue(options), "Didn't recognize --" + option);

                options = new ConsoleOptions("/" + option);
                Assert.AreEqual(true, (bool)field.GetValue(options), "Didn't recognize /" + option);
            }
        }

        [TestCase("activeConfig", "config")]
        //[TestCase("xmlPath", "xml")]
        [TestCase("outputPath", "output|out")]
        [TestCase("errorPath", "err")]
        [TestCase("include", "include")]
        [TestCase("exclude", "exclude")]
        [TestCase("workDir", "work")]
        public void CanRecognizeStringOptions(string fieldName, string pattern)
        {
            string[] prototypes = pattern.Split('|');

            FieldInfo field = GetFieldInfo(fieldName);
            Assert.AreEqual(typeof(string), field.FieldType);

            foreach (string option in prototypes)
            {
                ConsoleOptions options = new ConsoleOptions("--" + option + ":text");
                Assert.AreEqual("text", (string)field.GetValue(options), "Didn't recognize --" + option + ":text");
            }
        }

        [TestCase("domainUsage", "domain", typeof(DomainUsage))]
        [TestCase("processModel", "process", typeof(ProcessModel))]
        [TestCase("internalTraceLevel", "trace", typeof(InternalTraceLevel))]
        public void CanRecognizeEnumOptions(string fieldName, string pattern, Type enumType)
        {
            string[] prototypes = pattern.Split('|');

            FieldInfo field = GetFieldInfo(fieldName);
            Assert.IsNotNull(field, "Field {0} not found", fieldName);
            Assert.IsTrue(field.FieldType.IsEnum, "Field {0} is not an enum", fieldName);
            Assert.AreEqual(enumType, field.FieldType);

            foreach (string option in prototypes)
            {
                foreach (string name in Enum.GetNames(enumType))
                {
                    {
                        ConsoleOptions options = new ConsoleOptions("--" + option + ":" + name);
                        Assert.AreEqual(name, field.GetValue(options).ToString(), "Didn't recognize -" + option + ":" + name);
                    }
                }
            }
        }

        [TestCase("--config")]
        //[TestCase("--xml")]
        [TestCase("--output")]
        [TestCase("--err")]
        [TestCase("--include")]
        [TestCase("--exclude")]
        [TestCase("--work")]
        //[TestCase("--timeout")]
        //[TestCase("--trace")]
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
            Assert.AreEqual(1, options.InputFiles.Length);
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

        [Test]
        public void ResultOptionWithFilePath()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-result:results.xml");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Length, "assembly should be set");
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
            Assert.AreEqual(1, options.InputFiles.Length, "assembly should be set");
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
            Assert.AreEqual(1, options.InputFiles.Length, "assembly should be set");
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
            Assert.AreEqual(2, options.InputFiles.Length);
        }

        [Test]
        public void ResultOptionWithoutFileNameIsInvalid()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-result:");
            Assert.False(options.Validate(), "Should not be valid");
            Assert.AreEqual(1, options.ErrorMessages.Count, "An error was expected");
        }

        [Test]
        public void ExploreOptionWithoutPath()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-explore");
            Assert.True(options.Validate());
            Assert.True(options.explore);
        }

        [Test]
        public void ExploreOptionWithFilePath()
        {
            ConsoleOptions options = new ConsoleOptions("tests.dll", "-explore:results.xml");
            Assert.True(options.Validate());
            Assert.AreEqual(1, options.InputFiles.Length, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.True(options.explore);

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
            Assert.AreEqual(1, options.InputFiles.Length, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.True(options.explore);

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
            Assert.AreEqual(1, options.InputFiles.Length, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.True(options.explore);

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
            Assert.True(options.explore);
            Assert.AreEqual(1, options.InputFiles.Length, "assembly should be set");
            Assert.AreEqual("tests.dll", options.InputFiles[0]);
            Assert.AreEqual("C:/nunit/tests/bin/Debug/console-test.xml", options.ExploreOutputSpecifications[0].OutputPath);
        }

        #region Helper Methods

        private static FieldInfo GetFieldInfo(string fieldName)
        {
            FieldInfo field = typeof(ConsoleOptions).GetField(fieldName);
            Assert.IsNotNull(field, "The field '{0}' is not defined", fieldName);
            return field;
        }

        #endregion
    }
}
