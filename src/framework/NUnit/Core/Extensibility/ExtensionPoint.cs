// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Core.Extensibility
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

		protected IEnumerable Extensions
		{
			get { return extensions; }
		}

		#region Constructor
        public ExtensionPoint(string name, IExtensionHost host) : this( name, host, 0) { }

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
		protected abstract bool IsValidExtension(object extension);
		#endregion
	}
}
