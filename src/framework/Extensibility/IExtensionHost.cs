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

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// The IExtensionHost interface is implemented by each
	/// of NUnit's Extension hosts. Currently, there is
	/// only one host, which resides in the test domain.
	/// </summary>
	public interface IExtensionHost
	{
        /// <summary>
        /// Get a list of the ExtensionPoints provided by this host.
        /// </summary>
        IExtensionPoint[] ExtensionPoints
        {
            get;
        }

        /// <summary>
		/// Return an extension point by name, if present
		/// </summary>
		/// <param name="name">The name of the extension point</param>
		/// <returns>The extension point, if found, otherwise null</returns>
		IExtensionPoint GetExtensionPoint( string name );

        /// <summary>
        /// Gets the ExtensionTypes supported by this host
        /// </summary>
        /// <returns>An enum indicating the ExtensionTypes supported</returns>
        ExtensionType ExtensionTypes { get; }
	}
}
