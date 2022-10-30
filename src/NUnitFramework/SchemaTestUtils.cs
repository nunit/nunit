// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

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
            RunValidation(xml, schemaFileName, (sender, e) => Assert.Fail(e.Message));
        }

        public static void AssertInvalidXml(string xml, string schemaFileName)
        {
            var isInvalid = false;

            RunValidation(xml, schemaFileName, (sender, e) => isInvalid = true);

            if (!isInvalid) Assert.Fail("Validation did not fail.");
        }

        private static void RunValidation(string xml, string schemaFileName, ValidationEventHandler validationHandler)
        {
            var schemaSet = new XmlSchemaSet { XmlResolver = new SchemaResolver(GetSchemasPath()) };
            schemaSet.Add(AssertValidXsd(schemaFileName));

            var settings = new XmlReaderSettings
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema
            };

            settings.ValidationEventHandler += validationHandler;

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
