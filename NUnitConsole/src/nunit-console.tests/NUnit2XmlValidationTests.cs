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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using NUnit.Engine;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
	[TestFixture]
	public class NUnit2XmlValidationTests
	{
        private void runSchemaValidatorTest(string reportFileName)
        {
            new NUnit2XmlOutputWriter().WriteXmlOutput(this.result.Xml, reportFileName);

            if (!validator.Validate(reportFileName))
            {
                StringBuilder sb = new StringBuilder("Validation Errors:" + Environment.NewLine);
                foreach (string error in validator.Errors)
                    sb.Append("    " + error + Environment.NewLine);
                Assert.Fail(sb.ToString());
            }
        }

        private ITestEngineResult result;
        private SchemaValidator validator;

		private string tempFile;
        private static readonly string schemaFile = "NUnit2TestResult.xsd";

        [TestFixtureSetUp]
        public void CreateTestResultAndValidator()
        {
            ITestEngine engine = NUnit.Engine.TestEngineActivator.CreateInstance();
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string dir = Path.GetDirectoryName(uri.LocalPath);
            this.result = engine.Run(new TestPackage(Path.Combine(dir, "mock-assembly.dll")), TestListener.Null, TestFilter.Empty);

            this.validator = new SchemaValidator(Path.Combine(dir, schemaFile));
        }

		[SetUp]
		public void CreateTempFileName()
		{
            tempFile = Path.GetTempFileName();
		}

		[TearDown]
		public void RemoveTempFile()
		{
			FileInfo info = new FileInfo(tempFile);
			if(info.Exists) info.Delete();
		}

		[Test,SetCulture("")]
		public void TestSchemaValidatorInvariantCulture()
		{
			runSchemaValidatorTest(tempFile);
		}

		[Test,SetCulture("en-US")]
		public void TestSchemaValidatorUnitedStatesCulture()
		{
			runSchemaValidatorTest(tempFile);
		}

		[Test,SetCulture("fr-FR")]
		public void TestSchemaValidatorFrenchCulture()
		{
			runSchemaValidatorTest(tempFile);
		}
	}
}
