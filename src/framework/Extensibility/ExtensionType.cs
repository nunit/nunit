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
	/// The ExtensionType enumeration is used to indicate the
	/// kinds of extensions provided by an Addin. The addin
	/// is only installed by hosts supporting one of its
	/// extension types.
	/// </summary>
	[Flags]
	public enum ExtensionType
	{
		/// <summary>
		/// A Core extension is installed by the CoreExtensions
		/// host in each test domain.
		/// </summary>
		Core=1,

		/// <summary>
		/// A Client extension is installed by all clients
		/// </summary>
		Client=2,

		/// <summary>
		/// A Gui extension is installed by the gui client
		/// </summary>
		Gui=4
	}
}
