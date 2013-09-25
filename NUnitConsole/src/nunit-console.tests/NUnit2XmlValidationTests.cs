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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.ConsoleRunner.Tests
{
	[TestFixture]
	public class NUnit2XmlValidationTests : XmlOutputTest
	{
        private SchemaValidator validator;

        private static readonly string schemaFile = "NUnit2TestResult.xsd";

        [TestFixtureSetUp]
        public void InitializeValidator()
        {
            this.validator = new SchemaValidator(GetLocalPath(schemaFile));
        }

		[Test,SetCulture("")]
		public void TestSchemaValidatorInvariantCulture()
		{
			runSchemaValidatorTest();
		}

		[Test,SetCulture("en-US")]
		public void TestSchemaValidatorUnitedStatesCulture()
		{
			runSchemaValidatorTest();
		}

		[Test,SetCulture("fr-FR")]
		public void TestSchemaValidatorFrenchCulture()
		{
			runSchemaValidatorTest();
        }

        #region Helper Methods

        private void runSchemaValidatorTest()
        {
            StringBuilder output = new StringBuilder();

            new NUnit2XmlOutputWriter().WriteResultFile(this.EngineResult.Xml, new StringWriter(output));

            if (!validator.Validate(new StringReader(output.ToString())))
            {
                StringBuilder errors = new StringBuilder("Validation Errors:" + Environment.NewLine);
                foreach (string error in validator.Errors)
                    errors.Append("    " + error + Environment.NewLine);
                Assert.Fail(errors.ToString());
            }
        }

        #endregion
    }
}
