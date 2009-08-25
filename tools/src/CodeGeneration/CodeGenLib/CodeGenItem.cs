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
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.CodeGeneration
{
    public class CodeGenItem
    {
        private string fullSpec;

        private string specType;
        private string leftPart;
        private string className;
        private string methodName;
        private string attributes;
        private string rightPart;

        public CodeGenItem(string spec)
        {
            this.fullSpec = spec;

            int colon = spec.IndexOf(':');
            int arrow = spec.IndexOf("=>", colon + 1);
            if (colon <= 0 || arrow <= 0)
                throw new ArgumentException(string.Format("Invalid generation spec: {0}", spec), "spec");

            this.specType = spec.Substring(0, colon + 1);
            this.leftPart = spec.Substring(colon+1, arrow-colon-1).Trim();
            this.rightPart = spec.Substring(arrow + 2).Trim();

            int nest = 0; int attributeLength = 0;
            foreach( char c in leftPart )
            {
                if ( c == '[' ) nest++;
                if ( nest == 0 ) break;
                attributeLength++;
                if ( c == ']' ) nest--;
            }

            //this.attributes = leftPart.Substring(0, attributeLength);

            int dot = leftPart.IndexOf('.', attributeLength);
            if (dot <= 0)
                throw new ArgumentException(string.Format("Invalid generation spec: {0}", spec), "spec");

            this.className = leftPart.Substring(0, dot).Trim();
            this.methodName = leftPart.Substring(dot + 1).Trim();

            if (attributeLength > 0)
            {
                this.attributes = className.Substring(0, attributeLength);
                this.className = className.Substring(attributeLength);
            }
        }

        public string ItemType
        {
            get { return this.specType; }
        }

        public string LeftPart
        {
            get { return this.leftPart; }
        }

        public string RightPart
        {
            get { return this.rightPart; }
        }

        public string MethodName
        {
            get { return methodName; }
        }

        public string ClassName
        {
            get { return className; }
        }

        public string Attributes
        {
            get { return attributes; }
        }

        public bool IsGeneric
        {
            get
            { 
                return this.MethodName.IndexOf('<') > 0
                    || this.MethodName.IndexOf('?') > 0;
            }
        }

        public bool IsProperty
        {
            get { return !IsGeneric && !methodName.EndsWith(")"); }
        }

        public override string ToString()
        {
            return fullSpec;
        }
    }
}
