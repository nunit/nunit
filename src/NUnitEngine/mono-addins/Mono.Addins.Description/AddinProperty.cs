// 
// AddinProperty.cs
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
using System.Xml.Serialization;
using Mono.Addins.Serialization;

namespace Mono.Addins.Description
{
	/// <summary>
	/// An add-in property.
	/// </summary>
	public class AddinProperty: IBinaryXmlElement
	{
		/// <summary>
		/// Name of the property
		/// </summary>
		[XmlAttribute ("name")]
		public string Name { get; set; }

		/// <summary>
		/// Locale of the property. It is null if the property is not localized.
		/// </summary>
		[XmlAttribute ("locale")]
		public string Locale { get; set; }

		/// <summary>
		/// Value of the property.
		/// </summary>
		[XmlText]
		public string Value { get; set; }
		
		void IBinaryXmlElement.Read (BinaryXmlReader reader)
		{
			Name = reader.ReadStringValue ("name");
			Locale = reader.ReadStringValue ("locale");
			Value = reader.ReadStringValue ("value");
		}
		
		void IBinaryXmlElement.Write (BinaryXmlWriter writer)
		{
			writer.WriteValue ("name", Name);
			writer.WriteValue ("locale", Locale);
			writer.WriteValue ("value", Value);
		}
	}
}

