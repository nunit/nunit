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
    /// Represents a single point of extension for NUnit. Some extension
    /// points may accept only a single extension, while others may
    /// accept more than one at the same time.
    /// </summary>
    public interface IExtensionPoint
	{
		/// <summary>
		/// Get the name of this extension point
		/// </summary>
		string Name { get; }

        /// <summary>
        /// Get the host that provides this extension point
        /// </summary>
        IExtensionHost Host { get; }

        /// <summary>
        /// Install an extension at this extension point. If the
        /// extension object does not meet the requirements for
        /// this extension point, an exception is thrown.
        /// </summary>
        /// <param name="extension">The extension to install</param>
        void Install(object extension);

        /// <summary>
		/// Removes an extension from this extension point. If the
		/// extension object is not present, the method returns
		/// without error.
        /// </summary>
        /// <param name="extension"></param>
		void Remove( object extension );
	}

    /// <summary>
    /// Represents a single point of extension for NUnit. Some extension
    /// points may accept only a single extension, while others may
    /// accept more than one at the same time. This interface enhances
    /// IExtensionPoint by allowing specification of a priority
    /// order for applying addins.
    /// </summary>
    public interface IExtensionPoint2 : IExtensionPoint
    {
        /// <summary>
        /// Install an extension at this extension point specifying
        /// an integer priority value for the extension.If the
        /// extension object does not meet the requirements for
        /// this extension point, or if the extension point does
        /// not support the requested priority level, an exception 
        /// is thrown.
        /// </summary>
        /// <param name="extension">The extension to install</param>
        /// <param name="priority">The priority level for this extension</param>
        void Install(object extension, int priority);
    }
}
#endif
