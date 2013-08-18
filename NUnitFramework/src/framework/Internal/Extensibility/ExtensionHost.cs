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
using System.Collections.Generic;

namespace NUnit.Framework.Extensibility
{
	/// <summary>
	/// ExtensionHost is the abstract base class used for
	/// all extension hosts. It provides an array of 
	/// extension points and a FrameworkRegistry and
	/// implements the IExtensionHost interface. Derived
	/// classes must initialize the extension points.
	/// </summary>
	public abstract class ExtensionHost : IExtensionHost
	{
		#region Protected Fields

        /// <summary>
        /// List of extension points on this host
        /// </summary>
        protected List<ExtensionPoint> extensions = new List<ExtensionPoint>();

        /// <summary>
        /// Flags indicating the types of extensions supported by this host
        /// </summary>
        protected ExtensionType supportedTypes;

		#endregion

		#region IExtensionHost Interface

        /// <summary>
        /// Get a list of the ExtensionPoints provided by this host.
        /// </summary>
        /// <value></value>
		public IExtensionPoint[] ExtensionPoints
		{
			get { return (IExtensionPoint[])extensions.ToArray(); }
		}

        /// <summary>
        /// Return an extension point by name, if present
        /// </summary>
        /// <param name="name">The name of the extension point</param>
        /// <returns>
        /// The extension point, if found, otherwise null
        /// </returns>
		public IExtensionPoint GetExtensionPoint( string name )
		{
			foreach ( IExtensionPoint extensionPoint in extensions )
				if ( extensionPoint.Name == name )
					return extensionPoint;

			return null;
		}

        /// <summary>
        /// Gets the ExtensionTypes supported by this host
        /// </summary>
        /// <value></value>
        /// <returns>An enum indicating the ExtensionTypes supported</returns>
		public ExtensionType ExtensionTypes
		{
			get { return supportedTypes; }
		}
		#endregion
	}
}
#endif
