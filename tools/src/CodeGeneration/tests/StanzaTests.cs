// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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

namespace NUnit.Framework.CodeGeneration
{
    [TestFixture]
    public class StanzaTests
    {
        static readonly string NL = Environment.NewLine;

        [Test]
        public void EmptyStringGivesEmptyStanza()
        {
            Stanza stanza = Stanza.Read(new StringReader(string.Empty));
            Assert.That(stanza.Count, Is.EqualTo(0));
        }

        [Test]
        public void EmptyPlusPercentGivesEmptyStanza()
        {
            Stanza stanza = Stanza.Read(new StringReader("%"));
            Assert.That(stanza.Count, Is.EqualTo(0));
        }

        [Test]
        public void CommentsAreIgnored()
        {
            Stanza stanza = Stanza.Read(new StringReader(
                "# comment 1" + NL +
                "# comment 2" + NL +
                "line 1" + NL +
                "# comment 3" + NL +
                "line 2" + NL +
                "# comment 4" + NL));

            Assert.That(stanza, Is.EqualTo(new string[] {"line 1", "line 2"} ));
        }

        [Test]
        public void PercentSeparatesStanzas()
        {
            StringReader rdr = new StringReader(
                "line 1-1" + NL +
                "line 1-2" + NL +
                "%" + NL +
                "line 2-1" + NL +
                "line 2-2" + NL);

            Stanza stanza = Stanza.Read(rdr);
            Assert.That(stanza, Is.EqualTo(new string[] { "line 1-1", "line 1-2" } ));

            stanza = Stanza.Read(rdr);
            Assert.That(stanza, Is.EqualTo(new string[] { "line 2-1", "line 2-2" }));
        }
    }
}
