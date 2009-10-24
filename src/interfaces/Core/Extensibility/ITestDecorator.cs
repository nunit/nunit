// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// DecoratorPriority wraps constants that may be used
    /// to represent the relative priority of TestDecorators.
    /// Decorators with a lower priority are applied first
    /// so that higher priority decorators wrap them.
    /// 
    /// NOTE: This feature is subject to change.
	/// </summary>
    public class DecoratorPriority
	{
	    /// <summary>
	    /// The default priority, equivalent to Normal
	    /// </summary>
        public static readonly int Default = 0;
        /// <summary>
        /// Priority for Decorators that must apply first 
        /// </summary>
		public static readonly int First = 1;
        /// <summary>
        /// Normal Decorator priority
        /// </summary>
		public static readonly int Normal = 5;
        /// <summary>
        /// Priority for Decorators that must apply last
        /// </summary>
	    public static readonly int Last = 9;
	}

	/// <summary>
	/// The ITestDecorator interface is exposed by a class that knows how to
	/// enhance the functionality of a test case or suite by decorating it.
	/// </summary>
	public interface ITestDecorator
	{
		/// <summary>
		/// Examine the a Test and either return it as is, modify it
		/// or return a different TestCase.
		/// </summary>
		/// <param name="test">The Test to be decorated</param>
		/// <param name="member">The MethodInfo used to construct the test</param>
		/// <returns>The resulting Test</returns>
		Test Decorate( Test test, MemberInfo member );
	}
}
