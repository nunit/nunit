// ***********************************************************************
// Copyright (c) 2008 Charlie Poole
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

using System;
using System.Diagnostics;
using System.IO;

namespace NUnit.Framework.Internal
{
	/// <summary>
	/// A trace listener that writes to a separate file per domain
	/// and process using it.
	/// </summary>
	public class InternalTraceWriter : TextWriter
	{
        StreamWriter writer;

        static string logDirectory;
        public static string LogDirectory
        {
            get
            {
                if (logDirectory == null)
                {
                    logDirectory = Path.Combine(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "NUnit"), 
                        "logs");

                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);
                }

                return logDirectory;
            }
        }

		public InternalTraceWriter(string logName)
		{
			int pId = Process.GetCurrentProcess().Id;
			string domainName = AppDomain.CurrentDomain.FriendlyName;

			string fileName = logName
				.Replace("%p", Process.GetCurrentProcess().Id.ToString() )
				.Replace("%a", AppDomain.CurrentDomain.FriendlyName );

            string logPath = Path.Combine(LogDirectory, fileName);
            this.writer = new StreamWriter(logPath, true);
            this.writer.AutoFlush = true;
		}

        public override System.Text.Encoding Encoding
        {
            get { return writer.Encoding; }
        }

        public override void Write(char value)
        {
            writer.Write(value);
        }

        public override void Write(string value)
        {
            base.Write(value);
        }

        public override void WriteLine(string value)
        {
            writer.WriteLine(value);
        }

        public override void Close()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Close();
                writer = null;
            }
        }

        public override void Flush()
        {
            if ( writer != null )
                writer.Flush();
        }
	}
}
