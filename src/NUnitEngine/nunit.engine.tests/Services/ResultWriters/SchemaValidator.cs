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
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace NUnit.Engine.Tests.Services.ResultWriters
{
    public class SchemaValidator
    {
        private readonly XmlReaderSettings settings;
        private List<string> errors;

        public SchemaValidator(string schemaFile)
        {
            settings = new XmlReaderSettings {ValidationType = ValidationType.Schema};
            settings.Schemas.Add(XmlSchema.Read(new StreamReader(schemaFile), ValidationEventHandle));
        }

        public string[] Errors
        {
            get { return errors.ToArray(); }
        }

        public bool Validate(string xmlFile)
        {
            return Validate(new StreamReader(xmlFile));
        }

        public bool Validate(TextReader rdr)
        {
            errors = new List<string>();
            var myXmlValidatingReader = XmlReader.Create(rdr, settings);

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