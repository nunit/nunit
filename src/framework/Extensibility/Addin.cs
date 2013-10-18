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
using System.Reflection;

namespace NUnit.Framework.Extensibility
{
	/// <summary>
	/// The Addin class holds information about an addin.
	/// </summary>
	[Serializable]
	public class Addin
	{
		#region Private Fields
		private string typeName;
		private string name;
		private string description;
		private ExtensionType extensionType;
		private AddinStatus status;
		private string message;
		#endregion

		#region Constructor
		/// <summary>
		/// Construct an Addin for a type.
		/// </summary>
		/// <param name="type">The type to be used</param>
		public Addin( Type type )
		{
			this.typeName = type.AssemblyQualifiedName;

			object[] attrs = type.GetCustomAttributes( typeof(NUnitAddinAttribute), false );
			if ( attrs.Length == 1 )
			{
				NUnitAddinAttribute attr = (NUnitAddinAttribute)attrs[0];
				this.name = attr.Name;
				this.description = attr.Description;
				this.extensionType = attr.Type;
			}

			if ( this.name == null )
				this.name = type.Name;

			if ( this.extensionType == 0 )
				this.extensionType = ExtensionType.Core;

			this.status = AddinStatus.Enabled;
        }
		#endregion

		#region Properties
		/// <summary>
		/// The name of the Addin
		/// </summary>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Brief description of what the Addin does
		/// </summary>
		public string Description
		{
			get { return description; }
		}

		/// <summary>
		/// The type or types of extension provided, using 
		/// one or more members of the ExtensionType enumeration.
		/// </summary>
		public ExtensionType ExtensionType
		{
			get { return extensionType; }
		}

		/// <summary>
		/// The AssemblyQualifiedName of the type that implements
		/// the addin.
		/// </summary>
		public string TypeName
		{
			get { return typeName; }
		}

		/// <summary>
		/// The status of the addin
		/// </summary>
		public AddinStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		/// <summary>
		/// Any message that clarifies the status of the Addin,
		/// such as an error message or an explanation of why
		/// the addin is disabled.
		/// </summary>
		public string Message
		{
			get { return message; }
			set { message = value; }
		}
		#endregion

        #region Object Overrides
        /// <summary>
        /// Return true if two Addins have teh same type name
        /// </summary>
        /// <param name="obj">The other addin to be compared</param>
        public override bool Equals(object obj)
        {
            Addin addin = obj as Addin;
            if (addin == null)
                return false;

            return this.typeName.Equals(addin.typeName);
        }

        /// <summary>
        /// Return a hash code for this addin
        /// </summary>
        public override int GetHashCode()
        {
            return this.typeName.GetHashCode();
        }
        #endregion
    }
}
#endif
