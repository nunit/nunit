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

using System.Text;
using System.Xml;
using NUnit.Engine;

namespace NUnit.ConsoleRunner
{
    public class NUnit3XmlOutputWriter
    {
        public void WriteXmlOutput(XmlNode resultNode, string outputPath)
        {
            using (XmlTextWriter writer = new XmlTextWriter(outputPath, Encoding.Default))
            {
                var doc = resultNode.OwnerDocument;
                doc.InsertAfter(doc.CreateXmlDeclaration("1.0", "utf-8", "no"), null);
                writer.Formatting = Formatting.Indented;
                doc.WriteTo(writer);
                writer.Close();
            }
        }
    }
}
