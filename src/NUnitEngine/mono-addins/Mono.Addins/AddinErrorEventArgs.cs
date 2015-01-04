//
// AddinErrorEventArgs.cs
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
	/// Delegate to be used in add-in error subscriptions
	/// </summary>
	public delegate void AddinErrorEventHandler (object sender, AddinErrorEventArgs args);
	
	/// <summary>
	/// Provides information about an add-in loading error.
	/// </summary>
	public class AddinErrorEventArgs: AddinEventArgs
	{
		Exception exception;
		string message;

		/// <summary>
		/// Initializes a new instance of the <see cref="Mono.Addins.AddinErrorEventArgs"/> class.
		/// </summary>
		/// <param name='message'>
		/// Error message
		/// </param>
		/// <param name='addinId'>
		/// Add-in identifier.
		/// </param>
		/// <param name='exception'>
		/// Exception that caused the error.
		/// </param>
		public AddinErrorEventArgs (string message, string addinId, Exception exception): base (addinId)
		{
			this.message = message;
			this.exception = exception;
		}
		
		/// <summary>
		/// Exception that caused the error.
		/// </summary>
		public Exception Exception {
			get { return exception; }
		}
		
		/// <summary>
		/// Error message
		/// </summary>
		public string Message {
			get { return message; }
		}
	}
}
