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
using System.Reflection;
using System.Text;
using System.Xml;
using System.IO;
using NUnit.Framework.Internal;

namespace NUnit.ConsoleRunner
{
    /// <summary>
    /// NUnit3ResultOutputWriter is responsible for writing the results
    /// of a test to a file in NUnit 3.0 format.
    /// </summary>
    public class NUnit3ExploreOutputWriter : IResultWriter
    {
        public void WriteResultFile(XmlNode resultNode, string outputPath)
        {
            using (StreamWriter writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                WriteResultFile(resultNode, writer);
            }
        }

        private void WriteResultFile(XmlNode resultNode, TextWriter writer)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
            {
                xmlWriter.WriteStartDocument(false);
                resultNode.WriteTo(xmlWriter);
            }
        }
    }
}
