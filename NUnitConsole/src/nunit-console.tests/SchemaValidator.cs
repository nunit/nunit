using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    public class SchemaValidator
    {
        private XmlReaderSettings settings;
        private List<string> errors;

        public SchemaValidator(string schemaFile)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(XmlSchema.Read(
                new StreamReader(schemaFile),
                new ValidationEventHandler(ValidationEventHandle)));
        }

        public string[] Errors
        {
            get { return errors.ToArray(); }
        }

        public bool Validate(string xmlFile)
        {
            this.errors = new List<string>();

            XmlTextReader myXmlTextReader = new XmlTextReader(xmlFile);
            XmlReader myXmlValidatingReader = XmlReader.Create(myXmlTextReader, this.settings);

            try
            {
                // Read XML data
                while (myXmlValidatingReader.Read()) { }
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
            finally
            {
                myXmlValidatingReader.Close();
            }

            return errors.Count == 0;
        }

        public void ValidationEventHandle(object sender, ValidationEventArgs args)
        {
            errors.Add(args.Message);
        }
    }
}
