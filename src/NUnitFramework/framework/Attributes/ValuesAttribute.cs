// ***********************************************************************
// Copyright (c) 2008–2018 Charlie Poole, Rob Prouse
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
using System.Collections;
using System.Reflection;

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
    /// <summary>
    /// ValuesAttribute is used to provide literal arguments for
    /// an individual parameter of a test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ValuesAttribute : DataAttribute, IParameterDataSource
    {
        /// <summary>
        /// The collection of data to be returned. Must
        /// be set by any derived attribute classes.
        /// We use an object[] so that the individual
        /// elements may have their type changed in GetData
        /// if necessary
        /// </summary>
        protected object[] data;

        /// <summary>
        /// Constructs for use with an Enum parameter. Will pass every enum
        /// value in to the test.
        /// </summary>
        public ValuesAttribute()
        {
            data = new object[]{};
        }

        /// <summary>
        /// Construct with one argument
        /// </summary>
        /// <param name="arg1"></param>
        public ValuesAttribute(object arg1)
        {
            data = new object[] { arg1 };
        }

        /// <summary>
        /// Construct with two arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public ValuesAttribute(object arg1, object arg2)
        {
            data = new object[] { arg1, arg2 };
        }

        /// <summary>
        /// Construct with three arguments
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public ValuesAttribute(object arg1, object arg2, object arg3)
        {
            data = new object[] { arg1, arg2, arg3 };
        }

        /// <summary>
        /// Construct with an array of arguments
        /// </summary>
        /// <param name="args"></param>
        public ValuesAttribute(params object[] args)
        {
            data = args ?? new object[] { null };
        }

        /// <summary>
        /// Retrieves a list of arguments which can be passed to the specified parameter.
        /// </summary>
        /// <param name="fixtureType">The point of context in the fixture’s inheritance hierarchy.</param>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        public IEnumerable GetData(Type fixtureType, ParameterInfo parameter)
        {
            return ParamAttributeTypeConversions.ConvertData(data, parameter.ParameterType);
        }
    }
}
