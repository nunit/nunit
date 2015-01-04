//
// IProgressStatus.cs
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
	/// Progress status listener.
	/// </summary>
	public interface IProgressStatus
	{
		/// <summary>
		/// Sets the description of the current operation.
		/// </summary>
		/// <param name="msg">
		/// A message
		/// </param>
		/// <remarks>
		/// This method is called by the add-in engine to show a description of the operation being monitorized.
		/// </remarks>
		void SetMessage (string msg);
		
		/// <summary>
		/// Sets the progress of the operation.
		/// </summary>
		/// <param name="progress">
		/// A number between 0 and 1. 0 means no progress, 1 means operation completed.
		/// </param>
		/// <remarks>
		/// This method is called by the add-in engine to show the progress of the operation being monitorized.
		/// </remarks>
		void SetProgress (double progress);
		
		/// <summary>
		/// Writes text to the log.
		/// </summary>
		/// <param name="msg">
		/// Message to write
		/// </param>
		void Log (string msg);
		
		/// <summary>
		/// Log level requested by the user: 0: no log, 1: normal log, >1 verbose log
		/// </summary>
		int LogLevel { get; }
		
		/// <summary>
		/// Reports a warning.
		/// </summary>
		/// <param name="message">
		/// Warning message
		/// </param>
		/// <remarks>
		/// This method is called by the add-in engine to report a warning in the operation being monitorized.
		/// </remarks>
		void ReportWarning (string message);
		
		/// <summary>
		/// Reports an error.
		/// </summary>
		/// <param name="message">
		/// Error message
		/// </param>
		/// <param name="exception">
		/// Exception that caused the error. It can be null.
		/// </param>
		/// <remarks>
		/// This method is called by the add-in engine to report an error occurred while executing the operation being monitorized.
		/// </remarks>
		void ReportError (string message, Exception exception);
		
		/// <summary>
		/// Returns True when the user requested to cancel this operation
		/// </summary>
		bool IsCanceled { get; }
		
		/// <summary>
		/// Cancels the operation being montorized.
		/// </summary>
		void Cancel ();
	}
}
