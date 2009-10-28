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
using System.Collections.Generic;

namespace NUnit.Framework.CodeGeneration
{
    public class Stanza : List<string>
    {
        public static Stanza Read(TextReader rdr)
        {
            Stanza stanza = new Stanza();
            string line = rdr.ReadLine();

            while (line != null && line != "%")
            {
                if (!line.StartsWith("#"))
                    stanza.AddLine(line);

                line = rdr.ReadLine();
            }

            return stanza;
        }

        private void AddLine(string line)
        {
            int count = this.Count;

            if (char.IsWhiteSpace(line[0]) && count > 0)
                this[count - 1] += line.Trim();
            else
                this.Add(line);
        }

        private Stanza() { }
    }
}
