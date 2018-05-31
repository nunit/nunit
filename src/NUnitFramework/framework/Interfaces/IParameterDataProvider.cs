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

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Provides data for a single test parameter.
    /// </summary>
    public interface IParameterDataProvider
    {
        /// <summary>
        /// Determines whether any data is available for a parameter.
        /// </summary>
        /// <param name="fixtureType">The point of context in the fixture’s inheritance hierarchy.</param>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        bool HasDataFor(Type fixtureType, ParameterInfo parameter);

        /// <summary>
        /// Retrieves a list of arguments which can be passed to the specified parameter.
        /// </summary>
        /// <param name="fixtureType">The point of context in the fixture’s inheritance hierarchy.</param>
        /// <param name="parameter">The parameter of a parameterized test.</param>
        IEnumerable GetDataFor(Type fixtureType, ParameterInfo parameter);
    }
}
