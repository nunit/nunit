// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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
using System.Xml;
using System.Xml.Xsl;
using NUnit.Engine.Extensibility;

namespace NUnit.Engine.Services
{
    public class XmlTransformResultWriter : IResultWriter
    {
        private string _xsltFile;
        private readonly XslCompiledTransform _transform = new XslCompiledTransform();

        public XmlTransformResultWriter(object[] args)
        {
            Guard.ArgumentNotNull(args, "args");
            _xsltFile = args[0] as string;

            Guard.ArgumentValid(
                !string.IsNullOrEmpty(_xsltFile),
                "Argument to XmlTransformWriter must be a non-empty string",
                "args");

            try
            {
                _transform.Load(_xsltFile);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to load transform " + _xsltFile, ex.InnerException);
            }
        }

        /// <summary>
        /// Checks if the output is writable. If the output is not
        /// writable, this method should throw an exception.
        /// </summary>
        /// <param name="outputPath"></param>
        public void CheckWritability(string outputPath)
        {
            using ( new StreamWriter( outputPath, false ) )
            {
                // We don't need to check if the XSLT file exists, 
                // that would have thrown in the constructor
            }
        }

        public void WriteResultFile(XmlNode result, TextWriter writer)
        {
            using (var xmlWriter = new XmlTextWriter(writer))
            {
                xmlWriter.Formatting = Formatting.Indented;
                _transform.Transform(result, xmlWriter);
            }
        }

        public void WriteResultFile(XmlNode result, string outputPath)
        {
            using (var xmlWriter = new XmlTextWriter(outputPath, Encoding.Default))
            {
                xmlWriter.Formatting = Formatting.Indented;
                _transform.Transform(result, xmlWriter);
            }
        }
    }
}
