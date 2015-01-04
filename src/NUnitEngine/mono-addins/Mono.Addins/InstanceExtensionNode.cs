//
// InstanceExtensionNode.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
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
//


using System;

namespace Mono.Addins
{
	/// <summary>
	/// Base class for extension nodes which create extension objects
	/// </summary>
	public abstract class InstanceExtensionNode: ExtensionNode
	{
		object cachedInstance;
		
		/// <summary>
		/// Gets the extension object declared by this node
		/// </summary>
		/// <param name="expectedType">
		/// Expected object type. An exception will be thrown if the object is not an instance of the specified type.
		/// </param>
		/// <returns>
		/// The extension object
		/// </returns>
		/// <remarks>
		/// The extension object is cached and the same instance will be returned at every call.
		/// </remarks>
		public object GetInstance (Type expectedType)
		{
			object ob = GetInstance ();
			if (!expectedType.IsInstanceOfType (ob))
				throw new InvalidOperationException (string.Format ("Expected subclass of type '{0}'. Found '{1}'.", expectedType, ob.GetType ()));
			return ob;
		}
		
		/// <summary>
		/// Gets the extension object declared by this node
		/// </summary>
		/// <returns>
		/// The extension object
		/// </returns>
		/// <remarks>
		/// The extension object is cached and the same instance will be returned at every call.
		/// </remarks>
		public object GetInstance ()
		{
			if (cachedInstance == null)
				cachedInstance = CreateInstance ();
			return cachedInstance;
		}

		/// <summary>
		/// Creates a new extension object
		/// </summary>
		/// <param name="expectedType">
		/// Expected object type. An exception will be thrown if the object is not an instance of the specified type.
		/// </param>
		/// <returns>
		/// The extension object
		/// </returns>
		public object CreateInstance (Type expectedType)
		{
			object ob = CreateInstance ();
			if (!expectedType.IsInstanceOfType (ob))
				throw new InvalidOperationException (string.Format ("Expected subclass of type '{0}'. Found '{1}'.", expectedType, ob.GetType ()));
			return ob;
		}
		
		/// <summary>
		/// Creates a new extension object
		/// </summary>
		/// <returns>
		/// The extension object
		/// </returns>
		public abstract object CreateInstance ();
	}
}
