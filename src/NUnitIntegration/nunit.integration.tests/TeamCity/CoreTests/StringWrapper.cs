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

using System.IO;

namespace NUnit.Integration.Tests.TeamCity.CoreTests
{
    public sealed class StringWrapper : TextReader
    {
        private readonly string _string;
        private int _offset = 0;

        public StringWrapper(string str)
        {
            _string = str;
        }

        public override int Peek()
        {
            throw new System.NotImplementedException();
        }

        public override int Read()
        {
            if (_offset == _string.Length)
            {
                return -1;
            }
            
            if (_offset > _string.Length)
            {
                throw new IOException("Beyond end");
            }

            return _string[_offset++];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public override string ReadToEnd()
        {
            throw new System.NotImplementedException();
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public override string ReadLine()
        {
            throw new System.NotImplementedException();
        }
    }
}