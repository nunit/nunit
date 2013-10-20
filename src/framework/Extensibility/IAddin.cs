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

#if !NUNITLITE
using System;

namespace NUnit.Framework.Extensibility
{
	/// <summary>
	/// Add-ins are used to extend NUnti. All add-ins must
	/// implement the IAddin interface.
	/// </summary>
	public interface IAddin
	{
		/// <summary>
		/// When called, the add-in installs itself into
		/// the host, if possible. Because NUnit uses separate
		/// hosts for the client and test domain environments,
		/// an add-in may be invited to istall itself more than
		/// once. The add-in is responsible for checking which
		/// extension points are supported by the host that is
		/// passed to it and taking the appropriate action.
		/// </summary>
		/// <param name="host">The host in which to install the add-in</param>
		/// <returns>True if the add-in was installed, otehrwise false</returns>
		bool Install( IExtensionHost host );
	}
}
#endif

