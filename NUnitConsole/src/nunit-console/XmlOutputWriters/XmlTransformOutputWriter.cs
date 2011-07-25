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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace NUnit.ConsoleRunner
{
    public class XmlTransformOutputWriter : IXmlOutputWriter
    {
        private string xsltFile;
        private XslCompiledTransform transform = new XslCompiledTransform();

        public XmlTransformOutputWriter(string xsltFile)
        {
            this.xsltFile = xsltFile;
            transform.Load(xsltFile);
        }

        public void WriteXmlOutput(XmlNode result, TextWriter writer)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(writer))
            {
                xmlWriter.Formatting = Formatting.Indented;
                transform.Transform(result, xmlWriter);
            }
        }

        public void WriteXmlOutput(XmlNode result, string outputPath)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(outputPath, Encoding.Default))
            {
                xmlWriter.Formatting = Formatting.Indented;
                transform.Transform(result, xmlWriter);
            }
        }
    }
}
