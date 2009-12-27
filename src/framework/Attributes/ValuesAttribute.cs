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

#if !NUNITLITE
using System;
using System.Collections;
using System.Reflection;

namespace NUnit.Framework
{
    /// <summary>
    /// Abstract base class for attributes that apply to parameters 
    /// and supply data for the parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class ParameterDataAttribute : Attribute
    {
        /// <summary>
        /// Gets the data to be provided to the specified parameter
        /// </summary>
        public abstract IEnumerable GetData(ParameterInfo parameter);
    }

    /// <summary>
    /// ValuesAttribute is used to provide literal arguments for
    /// an individual parameter of a test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ValuesAttribute : ParameterDataAttribute
    {
        /// <summary>
        /// The collection of data to be returned. Must
        /// be set by any derived attribute classes.
        /// </summary>
        protected ICollection data;

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
            data = args;
        }

        /// <summary>
        /// Get the collection of values to be used as arguments
        /// </summary>
        public override IEnumerable GetData(ParameterInfo parameter)
        {
			return data;
        }
    }
}
#endif
