// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Arguments to parameters aligning interface.
    /// </summary>
    /// <remarks>
    /// Sometimes we have test case data (<see cref="TestCaseAttribute"/>,
    /// <see cref="TestCaseSourceAttribute"/>, etc.) describing arguments
    /// that differs in terms of their number or types from the ones 
    /// expected by testing method. This interface is responsible for
    /// access to the implementation of such arguments to parameters
    /// aligning routine.
    /// </remarks>
    public interface IAlignArguments
    {
        /// <summary>
        /// Align arguments with provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">Parameters to align with.</param>
        void Align(IParameterInfo[] parameters);
    }

    /// <summary>
    /// Useful method extensions for <see cref="IAlignArguments"/>.
    /// </summary>
    public static class IAlignArgumentsExtensions
    {
        /// <summary>
        /// Align arguments with parameters of the provided
        /// <paramref name="method"/>.
        /// </summary>
        /// <param name="source">Extended instance of <see cref="IAlignArguments"/>.</param>
        /// <param name="method">Method to align with.</param>
        public static void Align(this IAlignArguments source, IMethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var parameters = method.GetParameters();

            source.Align(parameters);
        }
    }
}
