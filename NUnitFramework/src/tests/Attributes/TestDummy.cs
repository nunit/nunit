// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
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
using NUnit.Framework.Api;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Attributes
{
    public class TestDummy : ITest
    {
        private int id = 1234;
        private RunState runState;
        private IPropertyBag properties = new PropertyBag();

        #region ITest Members

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string Name
        {
            get { return "TestDummy"; }
        }

        public string FullName
        {
            get { return "NUnit.Framework.Attributes.TestDummy"; }
        }

        public Type FixtureType
        {
            get { return null; }
        }

        public RunState RunState
        {
            get
            {
                return runState;
            }
            set
            {
                runState = value;
            }
        }

        public string TestKind
        {
            get { return "dummy-test"; }
        }

        public int TestCaseCount
        {
            get { throw new NotImplementedException(); }
        }

        public IPropertyBag Properties
        {
            get 
            { 
                return properties;
            }
        }

        public bool HasChildren
        {
            get
            {
                return false;
            }
        }

        public ITest Parent
        {
            get
            {
                return null;
            }
        }

#if CLR_2_0 || CLR_4_0
        public System.Collections.Generic.IList<ITest> Tests
#else
        public System.Collections.IList Tests
#endif
        {
            get
            {
                return new ITest[0];
            }
        }

        #endregion

        #region IXmlNodeBuilder Members

        public System.Xml.XmlNode ToXml(bool recursive)
        {
            throw new NotImplementedException();
        }

        public System.Xml.XmlNode AddToXml(System.Xml.XmlNode parentNode, bool recursive)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
