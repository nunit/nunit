// ***********************************************************************
// Copyright (c) 2007-2016 Charlie Poole, Rob Prouse
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
using System.IO;
using System.Text;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Execution
{
    /// <summary>
    /// EventListenerTextWriter sends text output to the currently active
    /// ITestEventListener in the form of a TestOutput object. If no event
    /// listener is active in the context, or if there is no context,
    /// the output is forwarded to the supplied default writer.
    /// </summary>
	public class EventListenerTextWriter : TextWriter
	{
        private readonly TextWriter _defaultWriter;
		private readonly string _streamName;

        /// <summary>
        /// Construct an EventListenerTextWriter
        /// </summary>
        /// <param name="streamName">The name of the stream to use for events</param>
        /// <param name="defaultWriter">The default writer to use if no listener is available</param>
		public EventListenerTextWriter( string streamName, TextWriter defaultWriter )
		{
			_streamName = streamName;
            _defaultWriter = defaultWriter;
		}

        /// <summary>
        /// Get the Encoding for this TextWriter
        /// </summary>
        public override Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private bool TrySendToListener(string text)
        {
            var context = TestExecutionContext.CurrentContext;
            if (context == null || context.Listener == null)
                return false;

            context.Listener.TestOutput(new TestOutput(text, _streamName, 
                context.CurrentTest?.Id, context.CurrentTest?.FullName));

            return true;
        }

        private bool TrySendLineToListener(string text)
        {
            return TrySendToListener(text + Environment.NewLine);
        }

        #region Write/WriteLine Methods
        // NB: We explicitly implement each of the Write and WriteLine methods so that
        // we are not dependent on the implementation of the TextWriter base class.
        //
        // For example, ideally, calling WriteLine(char[]) will send the text once,
        // however, the base implementation will send one character at a time.

        /// <summary>
        /// Write formatted string
        /// </summary>
        public override void Write(string format, params object[] arg)
        {
            if (!TrySendToListener(String.Format(FormatProvider, format, arg)))
                _defaultWriter.Write(format, arg);
        }

        /// <summary>
        /// Write formatted string
        /// </summary>
        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            if (!TrySendToListener(String.Format(FormatProvider, format, arg0, arg1, arg2)))
                _defaultWriter.Write(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Write formatted string
        /// </summary>
        public override void Write(string format, object arg0)
        {
            if (!TrySendToListener(String.Format(FormatProvider, format, arg0)))
                _defaultWriter.Write(format, arg0);
        }

        /// <summary>
        /// Write an object
        /// </summary>
        public override void Write(object value)
        {
            if (value != null)
            {
                IFormattable f = value as IFormattable;
                if (f != null)
                {
                    if (!TrySendToListener(f.ToString(null, FormatProvider)))
                        _defaultWriter.Write(value);
                }
                else
                {
                    if (!TrySendToListener(value.ToString()))
                        _defaultWriter.Write(value);
                }
            }
            else
            {
                _defaultWriter.Write(value);
            }
        }

        /// <summary>
        /// Write a string
        /// </summary>
        public override void Write(string value)
        {
            if (!TrySendToListener(value))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write a decimal
        /// </summary>
        public override void Write(decimal value)
        {
            if (!TrySendToListener(value.ToString(FormatProvider)))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write a double
        /// </summary>
        public override void Write(double value)
        {
            if (!TrySendToListener(value.ToString(FormatProvider)))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write formatted string
        /// </summary>
        public override void Write(string format, object arg0, object arg1)
        {
            if (!TrySendToListener(String.Format(FormatProvider, format, arg0, arg1)))
                _defaultWriter.Write(format, arg0, arg1);
        }

        /// <summary>
        /// Write a ulong
        /// </summary>
        [CLSCompliant(false)]
        public override void Write(ulong value)
        {
            if (!TrySendToListener(value.ToString(FormatProvider)))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write a long
        /// </summary>
        public override void Write(long value)
        {
            if (!TrySendToListener(value.ToString(FormatProvider)))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write a uint
        /// </summary>
        [CLSCompliant(false)]
        public override void Write(uint value)
        {
            if (!TrySendToListener(value.ToString(FormatProvider)))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write an int
        /// </summary>
        public override void Write(int value)
        {
            if (!TrySendToListener(value.ToString(FormatProvider)))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write a char
        /// </summary>
        public override void Write(char value)
        {
            if (!TrySendToListener(value.ToString()))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write a boolean
        /// </summary>
        public override void Write(bool value)
        {
            if (!TrySendToListener(value ? Boolean.TrueString : Boolean.FalseString))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write chars
        /// </summary>
        public override void Write(char[] buffer, int index, int count)
        {
            if (!TrySendToListener(new string(buffer, index, count)))
                _defaultWriter.Write(buffer, index, count);
        }

        /// <summary>
        /// Write chars
        /// </summary>
        public override void Write(char[] buffer)
        {
            if (!TrySendToListener(new string(buffer)))
                _defaultWriter.Write(buffer);
        }

        /// <summary>
        /// Write a float
        /// </summary>
        public override void Write(float value)
        {
            if (!TrySendToListener(value.ToString(FormatProvider)))
                _defaultWriter.Write(value);
        }

        /// <summary>
        /// Write a string with newline
        /// </summary>
        public override void WriteLine(string value)
        {
            if (!TrySendLineToListener(value))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write an object with newline
        /// </summary>
        public override void WriteLine(object value)
        {
            if (value == null)
            {
                if (!TrySendLineToListener(string.Empty))
                    _defaultWriter.WriteLine(value);
            }
            else
            {
                IFormattable f = value as IFormattable;
                if (f != null)
                {
                    if (!TrySendLineToListener(f.ToString(null, FormatProvider)))
                        _defaultWriter.WriteLine(value);
                }
                else
                {
                    if (!TrySendLineToListener(value.ToString()))
                        _defaultWriter.WriteLine(value);
                }
            }
        }

        /// <summary>
        /// Write formatted string with newline
        /// </summary>
        public override void WriteLine(string format, params object[] arg)
        {
            if (!TrySendLineToListener(String.Format(FormatProvider, format, arg)))
                _defaultWriter.WriteLine(format, arg);
        }

        /// <summary>
        /// Write formatted string with newline
        /// </summary>
        public override void WriteLine(string format, object arg0, object arg1)
        {
            if (!TrySendLineToListener(String.Format(FormatProvider, format, arg0, arg1)))
                _defaultWriter.WriteLine(format, arg0, arg1);
        }

        /// <summary>
        /// Write formatted string with newline
        /// </summary>
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            if (!TrySendLineToListener(String.Format(FormatProvider, format, arg0, arg1, arg2)))
                _defaultWriter.WriteLine(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// Write a decimal with newline
        /// </summary>
        public override void WriteLine(decimal value)
        {
            if (!TrySendLineToListener(value.ToString(FormatProvider)))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write a formatted string with newline
        /// </summary>
        public override void WriteLine(string format, object arg0)
        {
            if (!TrySendLineToListener(String.Format(FormatProvider, format, arg0)))
                _defaultWriter.WriteLine(format, arg0);
        }

        /// <summary>
        /// Write a double with newline
        /// </summary>
        public override void WriteLine(double value)
        {
            if (!TrySendLineToListener(value.ToString(FormatProvider)))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write a uint with newline
        /// </summary>
        [CLSCompliant(false)]
        public override void WriteLine(uint value)
        {
            if (!TrySendLineToListener(value.ToString(FormatProvider)))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write a ulong with newline
        /// </summary>
        [CLSCompliant(false)]
        public override void WriteLine(ulong value)
        {
            if (!TrySendLineToListener(value.ToString(FormatProvider)))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write a long with newline
        /// </summary>
        public override void WriteLine(long value)
        {
            if (!TrySendLineToListener(value.ToString(FormatProvider)))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write an int with newline
        /// </summary>
        public override void WriteLine(int value)
        {
            if (!TrySendLineToListener(value.ToString(FormatProvider)))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write a bool with newline
        /// </summary>
        public override void WriteLine(bool value)
        {
            if (!TrySendLineToListener(value ? Boolean.TrueString : Boolean.FalseString))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write chars with newline
        /// </summary>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            if (!TrySendLineToListener(new string(buffer, index, count)))
                _defaultWriter.WriteLine(buffer, index, count);
        }

        /// <summary>
        /// Write chars with newline
        /// </summary>
        public override void WriteLine(char[] buffer)
        {
            if (!TrySendLineToListener(new string(buffer)))
                _defaultWriter.WriteLine(buffer);
        }

        /// <summary>
        /// Write a char with newline
        /// </summary>
        public override void WriteLine(char value)
        {
            if (!TrySendLineToListener(value.ToString()))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write a float with newline
        /// </summary>
        public override void WriteLine(float value)
        {
            if (!TrySendLineToListener(value.ToString(FormatProvider)))
                _defaultWriter.WriteLine(value);
        }

        /// <summary>
        /// Write newline
        /// </summary>
        public override void WriteLine()
        {
            if (!TrySendLineToListener(string.Empty))
                _defaultWriter.WriteLine();
        }

        #endregion
    }
}
