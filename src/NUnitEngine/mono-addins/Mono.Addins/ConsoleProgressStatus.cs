//
// ConsoleProgressStatus.cs
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
	/// An IProgressStatus class which writes output to the console.
	/// </summary>
	public class ConsoleProgressStatus: MarshalByRefObject, IProgressStatus
	{
		bool canceled;
		int logLevel;
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="verboseLog">
		/// Set to true to enabled verbose log
		/// </param>
		public ConsoleProgressStatus (bool verboseLog)
		{
			if (verboseLog)
				logLevel = 2;
			else
				logLevel = 1;
		}
		
		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="logLevel">
		/// Verbosity level. 0: not verbose, 1: normal, >1 extra verbose
		/// </param>
		public ConsoleProgressStatus (int logLevel)
		{
			this.logLevel = logLevel;
		}
		
		/// <summary>
		/// Sets the description of the current operation.
		/// </summary>
		/// <param name="msg">
		/// A message
		/// </param>
		/// <remarks>
		/// This method is called by the add-in engine to show a description of the operation being monitorized.
		/// </remarks>
		public void SetMessage (string msg)
		{
		}
		
		/// <summary>
		/// Sets the progress of the operation.
		/// </summary>
		/// <param name="progress">
		/// A number between 0 and 1. 0 means no progress, 1 means operation completed.
		/// </param>
		/// <remarks>
		/// This method is called by the add-in engine to show the progress of the operation being monitorized.
		/// </remarks>
		public void SetProgress (double progress)
		{
		}
		
		/// <summary>
		/// Writes text to the log.
		/// </summary>
		/// <param name="msg">
		/// Message to write
		/// </param>
		public void Log (string msg)
		{
			Console.WriteLine (msg);
		}
		
		/// <summary>
		/// Reports a warning.
		/// </summary>
		/// <param name="message">
		/// Warning message
		/// </param>
		/// <remarks>
		/// This method is called by the add-in engine to report a warning in the operation being monitorized.
		/// </remarks>
		public void ReportWarning (string message)
		{
			if (logLevel > 0)
				Console.WriteLine ("WARNING: " + message);
		}
		
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
		public void ReportError (string message, Exception exception)
		{
			if (logLevel == 0)
				return;
			Console.Write ("ERROR: ");
			if (logLevel > 1) {
				if (message != null)
					Console.WriteLine (message);
				if (exception != null)
					Console.WriteLine (exception);
			} else {
				if (message != null && exception != null)
					Console.WriteLine (message + " (" + exception.Message + ")");
				else {
					if (message != null)
						Console.WriteLine (message);
					if (exception != null)
						Console.WriteLine (exception.Message);
				}
			}
		}
		
		/// <summary>
		/// Returns True when the user requested to cancel this operation
		/// </summary>
		public bool IsCanceled {
			get { return canceled; }
		}
		
		/// <summary>
		/// Log level requested by the user: 0: no log, 1: normal log, >1 verbose log
		/// </summary>
		public int LogLevel {
			get { return logLevel; }
		}
		
		/// <summary>
		/// Cancels the operation being montorized.
		/// </summary>
		public void Cancel ()
		{
			canceled = true;
		}
	}
}

