// 
// AddinNameAttribute.cs
//  
// Author:
//       Lluis Sanchez Gual <lluis@novell.com>
// 
// Copyright (c) 2011 Novell, Inc (http://www.novell.com)
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
	/// Sets the display name of an add-in
	/// </summary>
	[AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
	public class AddinNameAttribute: Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinNameAttribute"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name of the add-in
		/// </param>
		public AddinNameAttribute (string name)
		{
			Name = name;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinNameAttribute"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name of the add-in
		/// </param>
		/// <param name='locale'>
		/// Locale of the name (for example, 'en-US', or 'en')
		/// </param>
		public AddinNameAttribute (string name, string locale)
		{
			Name = name;
			Locale = locale;
		}
		
		/// <value>
		/// Name of the add-in
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Locale of the name (for example, 'en-US', or 'en')
		/// </summary>
		public string Locale { get; set; }
	}
}

