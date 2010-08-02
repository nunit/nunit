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

using System;
using System.Collections;

namespace NUnit.Framework.Extensibility
{
	/// <summary>
	/// ExtensionPoint is used as a base class for all 
	/// extension points.
	/// </summary>
	public abstract class ExtensionPoint : IExtensionPoint
	{
		private readonly string name;
		private readonly IExtensionHost host;
		private readonly ExtensionsCollection extensions;

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <value>The extensions.</value>
		protected IEnumerable Extensions
		{
			get { return extensions; }
		}

		#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPoint"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="host">The host.</param>
        public ExtensionPoint(string name, IExtensionHost host) : this( name, host, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionPoint"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="host">The host.</param>
        /// <param name="priorityLevels">The priority levels.</param>
	    public ExtensionPoint(string name, IExtensionHost host, int priorityLevels)
		{
			this.name = name;
			this.host = host;
            extensions = new ExtensionsCollection(priorityLevels);
		}
		#endregion

		#region IExtensionPoint Members

		/// <summary>
		/// Get the name of this extension point
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// Get the host that provides this extension point
		/// </summary>
		public IExtensionHost Host
		{
			get { return this.host; }
		}

        /// <summary>
        /// Install an extension at this extension point. If the
        /// extension object does not meet the requirements for
        /// this extension point, an exception is thrown.
        /// </summary>
        /// <param name="extension">The extension to install</param>
        public void Install(object extension)
        {
            if (!IsValidExtension(extension))
                throw new ArgumentException(
                    extension.GetType().FullName + " is not {0} extension point", "extension");

            extensions.Add(extension);
        }

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
        public void Install(object extension, int priority )
        {
            if (!IsValidExtension(extension))
                throw new ArgumentException(
                    extension.GetType().FullName + " is not {0} extension point", "extension");

            if (priority < 0 || priority >= extensions.Levels)
                throw new ArgumentException("Priority value not supported", "priority");

            extensions.Add(extension, priority);
        }

        /// <summary>
		/// Removes an extension from this extension point. If the
		/// extension object is not present, the method returns
		/// without error.
		/// </summary>
		/// <param name="extension"></param>
		public void Remove(object extension)
		{
			extensions.Remove( extension );
		}
		#endregion

		#region Abstract Members

        /// <summary>
        /// Determines whether the specified extension is valid for this extension point.
        /// </summary>
        /// <param name="extension">The extension.</param>
		protected abstract bool IsValidExtension(object extension);

		#endregion
	}

    /// <summary>
    /// ExtensionPointList represents a collection of extension points in a cross-platform manner.
    /// </summary>
#if CLR_2_0 || CLR_4_0
    public class ExtensionPointList : System.Collections.Generic.List<ExtensionPoint> { }
#else
    public class ExtensionPointList : ArrayList { }
#endif
}
