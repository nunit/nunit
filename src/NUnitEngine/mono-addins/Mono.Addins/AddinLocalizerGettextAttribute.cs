// 
// AddinLocalizerAttribute.cs
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
	/// Declares a Gettext-based localizer for an add-in
	/// </summary>
	[AttributeUsage (AttributeTargets.Assembly)]
	public class AddinLocalizerGettextAttribute: Attribute
	{
		string catalog;
		string location;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinLocalizerGettextAttribute"/> class.
		/// </summary>
		public AddinLocalizerGettextAttribute ()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinLocalizerGettextAttribute"/> class.
		/// </summary>
		/// <param name='catalog'>
		/// Name of the catalog which contains the strings.
		/// </param>
		public AddinLocalizerGettextAttribute (string catalog)
		{
			this.catalog = catalog;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinLocalizerGettextAttribute"/> class.
		/// </summary>
		/// <param name='catalog'>
		/// Name of the catalog which contains the strings.
		/// </param>
		/// <param name='location'>
		/// Relative path to the location of the catalog. This path must be relative to the add-in location.
		/// </param>
		/// <remarks>
		/// The location path must contain a directory structure like this:
		/// 
		/// {language-id}/LC_MESSAGES/{Catalog}.mo
		/// 
		/// For example, the catalog for spanish strings would be located at:
		/// 
		/// locale/es/LC_MESSAGES/some-addin.mo
		/// </remarks>
		public AddinLocalizerGettextAttribute (string catalog, string location)
		{
			this.catalog = catalog;
			this.location = location;
		}
		
		/// <summary>
		/// Name of the catalog which contains the strings.
		/// </summary>
		public string Catalog {
			get { return this.catalog; }
			set { this.catalog = value; }
		}

		/// <summary>
		/// Relative path to the location of the catalog. This path must be relative to the add-in location.
		/// </summary>
		/// <remarks>
		/// When not specified, the default value of this property is 'locale'.
		/// The location path must contain a directory structure like this:
		/// 
		/// {language-id}/LC_MESSAGES/{Catalog}.mo
		/// 
		/// For example, the catalog for spanish strings would be located at:
		/// 
		/// locale/es/LC_MESSAGES/some-addin.mo
		/// </remarks>
		public string Location {
			get { return this.location; }
			set { this.location = value; }
		}
	}
}

