// 
// ExtensionAttributeAttribute.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

namespace Mono.Addins
{
	/// <summary>
	/// Assigns an attribute value to an extension
	/// </summary>
	/// <remarks>
	/// This attribute can be used together with the [Extension] attribute to specify
	/// a value for an attribute of the extension.
	/// </remarks>
	public class ExtensionAttributeAttribute: Attribute
	{
		Type targetType;
		string targetTypeName;
		string name;
		string val;
		string path;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.ExtensionAttributeAttribute"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name of the attribute
		/// </param>
		/// <param name='value'>
		/// Value of the attribute
		/// </param>
		public ExtensionAttributeAttribute (string name, string value)
		{
			Name = name;
			Value = value;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.ExtensionAttributeAttribute"/> class.
		/// </summary>
		/// <param name='type'>
		/// Type of the extension for which the attribute value is being set
		/// </param>
		/// <param name='name'>
		/// Name of the attribute
		/// </param>
		/// <param name='value'>
		/// Value of the attribute
		/// </param>
		public ExtensionAttributeAttribute (Type type, string name, string value)
		{
			Name = name;
			Value = value;
			Type = type;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.ExtensionAttributeAttribute"/> class.
		/// </summary>
		/// <param name='path'>
		/// Path of the extension for which the attribute value is being set
		/// </param>
		/// <param name='name'>
		/// Name of the attribute
		/// </param>
		/// <param name='value'>
		/// Value of the attribute
		/// </param>
		public ExtensionAttributeAttribute (string path, string name, string value)
		{
			Name = name;
			Value = value;
			Path = path;
		}
		
		/// <summary>
		/// Name of the attribute
		/// </summary>
		public string Name {
			get { return this.name; }
			set { this.name = value; }
		}

		/// <summary>
		/// Value of the attribute
		/// </summary>
		public string Value {
			get { return this.val; }
			set { this.val = value; }
		}

		/// <summary>
		/// Path of the extension for which the attribute value is being set
		/// </summary>
		public string Path {
			get { return this.path; }
			set { this.path = value; }
		}

		/// <summary>
		/// Type of the extension for which the attribute value is being set
		/// </summary>
		public Type Type {
			get { return targetType; }
			set { targetType = value; targetTypeName = targetType.FullName; }
		}
		
		internal string TypeName {
			get { return targetTypeName ?? string.Empty; }
			set { targetTypeName = value; }
		}
	}
}

