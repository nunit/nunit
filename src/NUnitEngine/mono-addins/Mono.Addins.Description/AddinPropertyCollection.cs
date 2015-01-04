// 
// AddinPropertyCollection.cs
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
using System.Linq;
using System.Collections.Generic;

namespace Mono.Addins.Description
{
	/// <summary>
	/// A collection of add-in properties
	/// </summary>
	public interface AddinPropertyCollection: IEnumerable<AddinProperty>
	{
		/// <summary>
		/// Gets the value of a property
		/// </summary>
		/// <returns>
		/// The property value.
		/// </returns>
		/// <param name='name'>
		/// Name of the property.
		/// </param>
		/// <remarks>
		/// If the property is localized, it will return the value for the current language if exists, or the
		/// default value if it doesn't.
		/// </remarks>
		string GetPropertyValue (string name);
		
		/// <summary>
		/// Gets the value of a property
		/// </summary>
		/// <returns>
		/// The property value.
		/// </returns>
		/// <param name='name'>
		/// Name of the property.
		/// </param>
		/// <param name='locale'>
		/// Locale for which the value must be returned.
		/// </param>
		string GetPropertyValue (string name, string locale);
		
		/// <summary>
		/// Sets the value of a property
		/// </summary>
		/// <param name='name'>
		/// Name of the property
		/// </param>
		/// <param name='value'>
		/// New value.
		/// </param>
		void SetPropertyValue (string name, string value);
		
		/// <summary>
		/// Sets the value of a property for a specific locale
		/// </summary>
		/// <param name='name'>
		/// Name of the property.
		/// </param>
		/// <param name='value'>
		/// New value.
		/// </param>
		/// <param name='locale'>
		/// Locale of the property to be set.
		/// </param>
		void SetPropertyValue (string name, string value, string locale);
		
		/// <summary>
		/// Removes a property.
		/// </summary>
		/// <param name='name'>
		/// Name of the property.
		/// </param>
		/// <remarks>
		/// This method only removes properties which have no locale set.
		/// </remarks>
		void RemoveProperty (string name);
		
		/// <summary>
		/// Removes a property with a specified locale
		/// </summary>
		/// <param name='name'>
		/// Name of the property
		/// </param>
		/// <param name='locale'>
		/// Locale of the property
		/// </param>
		void RemoveProperty (string name, string locale);
	}
	
	class AddinPropertyCollectionImpl: List<AddinProperty>, AddinPropertyCollection
	{
		AddinDescription desc;

		public AddinPropertyCollectionImpl ()
		{
		}
		
		public AddinPropertyCollectionImpl (AddinDescription desc)
		{
			this.desc = desc;
		}
		
		public AddinPropertyCollectionImpl (AddinPropertyCollection col)
		{
			AddRange (col);
		}
		
		public string GetPropertyValue (string name)
		{
			return GetPropertyValue (name, System.Threading.Thread.CurrentThread.CurrentCulture.ToString ());
		}
		
		public string GetPropertyValue (string name, string locale)
		{
			locale = NormalizeLocale (locale);
			string lang = GetLocaleLang (locale);
			AddinProperty sameLangDifCountry = null;
			AddinProperty sameLang = null;
			AddinProperty defaultLoc = null;
			
			foreach (var p in this) {
				if (p.Name == name) {
					if (p.Locale == locale)
						return ParseString (p.Value);
					string plang = GetLocaleLang (p.Locale);
					if (plang == p.Locale && plang == lang) // No country specified
						sameLang = p;
					else if (plang == lang)
						sameLangDifCountry = p;
					else if (p.Locale == null)
						defaultLoc = p;
				}
			}
			if (sameLang != null)
				return ParseString (sameLang.Value);
			else if (sameLangDifCountry != null)
				return ParseString (sameLangDifCountry.Value);
			else if (defaultLoc != null)
				return ParseString (defaultLoc.Value);
			else
				return string.Empty;
		}

		string ParseString (string s)
		{
			if (desc != null)
				return desc.ParseString (s);
			else
				return s;
		}
		
		string NormalizeLocale (string loc)
		{
			if (string.IsNullOrEmpty (loc))
				return null;
			return loc.Replace ('_','-');
		}
		
		string GetLocaleLang (string loc)
		{
			if (loc == null)
				return null;
			int i = loc.IndexOf ('-');
			if (i != -1)
				return loc.Substring (0, i);
			else
				return loc;
		}
		
		public void SetPropertyValue (string name, string value)
		{
			SetPropertyValue (name, value, null);
		}
		
		public void SetPropertyValue (string name, string value, string locale)
		{
			if (string.IsNullOrEmpty (name))
				throw new ArgumentException ("name can't be null or empty");
			
			if (value == null)
				throw new ArgumentNullException ("value");
			
			locale = NormalizeLocale (locale);
			
			foreach (var p in this) {
				if (p.Name == name && p.Locale == locale) {
					p.Value = value;
					return;
				}
			}
			AddinProperty prop = new AddinProperty ();
			prop.Name = name;
			prop.Value = value;
			prop.Locale = locale;
			Add (prop);
		}
		
		public void RemoveProperty (string name)
		{
			RemoveProperty (name, null);
		}
		
		public void RemoveProperty (string name, string locale)
		{
			locale = NormalizeLocale (locale);
			
			foreach (var p in this) {
				if (p.Name == name && p.Locale == locale) {
					Remove (p);
					return;
				}
			}
		}
		
		internal bool HasProperty (string name)
		{
			return this.Any (p => p.Name == name);
		}
		
		internal string ExtractCoreProperty (string name, bool removeProperty)
		{
			foreach (var p in this) {
				if (p.Name == name && p.Locale == null) {
					if (removeProperty)
						Remove (p);
					return p.Value;
				}
			}
			return null;
		}
	}
}

