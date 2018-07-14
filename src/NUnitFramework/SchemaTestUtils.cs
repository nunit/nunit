// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
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

#if !NETCOREAPP1_1
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace NUnit.TestUtilities
{
    internal static class SchemaTestUtils
    {
        public static string GetSchemasPath() => Path.Combine(TestContext.CurrentContext.TestDirectory, "Schemas");

        public static XmlSchema AssertValidXsd(string schemaFile)
        {
            using (var testXsd = File.OpenRead(Path.Combine(GetSchemasPath(), schemaFile)))
            {
                return XmlSchema.Read(testXsd,
                    validationEventHandler: (sender, e) => Assert.Fail(e.Message));
            }
        }

        public static void AssertValidXml(string xml, string schemaFileName)
        {
            var schemaSet = new XmlSchemaSet { XmlResolver = new SchemaResolver(GetSchemasPath()) };
            schemaSet.Add(AssertValidXsd(schemaFileName));

            var settings = new XmlReaderSettings
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema
            };

            settings.ValidationEventHandler += (sender, e) => Assert.Fail(e.Message);

            using (var reader = XmlReader.Create(new StringReader(xml), settings))
            {
                while (reader.Read()) { }
            }
        }

        private sealed class SchemaResolver : XmlUrlResolver
        {
            private readonly string _schemasPath;

            public SchemaResolver(string schemasPath)
            {
                _schemasPath = schemasPath;
            }

            public override Uri ResolveUri(Uri baseUri, string relativeUri)
            {
                return new Uri(Path.Combine(_schemasPath, relativeUri));
            }
        }
    }
}
#endif
