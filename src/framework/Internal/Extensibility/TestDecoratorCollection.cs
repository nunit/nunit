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
using System.Collections;
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// TestDecoratorCollection is an ExtensionPoint for TestDecorators and
	/// implements the ITestDecorator interface itself, passing calls 
	/// on to the individual decorators.
	/// </summary>
	public class TestDecoratorCollection : ExtensionPoint, IExtensionPoint2, ITestDecorator
	{
		#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestDecoratorCollection"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
		public TestDecoratorCollection(IExtensionHost host)
			: base( "TestDecorators", host, 10 ) { }

		#endregion

		#region ITestDecorator Members

        /// <summary>
        /// Examine the a Test and either return it as is, modify it
        /// or return a different TestCase.
        /// </summary>
        /// <param name="test">The Test to be decorated</param>
        /// <param name="member">The MethodInfo used to construct the test</param>
        /// <returns>The resulting Test</returns>
		public Test Decorate(Test test, MemberInfo member)
		{
			Test decoratedTest = test;

			foreach( ITestDecorator decorator in Extensions )
				decoratedTest = decorator.Decorate( decoratedTest, member );

			return decoratedTest;
		}

		#endregion

		#region ExtensionPoint Overrides

        /// <summary>
        /// Determines whether [is valid extension] [the specified extension].
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid extension] [the specified extension]; otherwise, <c>false</c>.
        /// </returns>
		protected override bool IsValidExtension(object extension)
		{
			return extension is ITestDecorator; 
		}

		#endregion
	}
}
