// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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
using System.Collections;
using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
    public class InlineDataPointProvider : IDataPointProvider
    {
        private static readonly string ParameterDataAttribute = "NUnit.Framework.ParameterDataAttribute";

        private static readonly string GetDataMethod = "GetData";

        #region IDataPointProvider Members

        public bool HasDataFor(ParameterInfo parameter)
        {
            return parameter.IsDefined(typeof(NUnit.Framework.ParameterDataAttribute), false);
        }
        
        public IEnumerable GetDataFor(ParameterInfo parameter)
        {
            Attribute attr = Reflect.GetAttribute(parameter, ParameterDataAttribute, false);
            if (attr == null) return null;

            MethodInfo getData = attr.GetType().GetMethod(
                GetDataMethod,
                new Type[] { typeof(ParameterInfo) });
            if ( getData == null) return null;
            
            return getData.Invoke(attr, new object[] { parameter }) as IEnumerable;
        }
        #endregion
    }
}
