// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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

using System.IO;

namespace NUnit.Core
{
    /// <summary>
    /// Abstract base for classes that capture text output
    /// and redirect it to a TextWriter.
    /// </summary>
    public abstract class TextCapture
    {
        #region Private Fields
        /// <summary>
        /// True if capture is enabled
        /// </summary>
        private bool enabled;

        /// <summary>
        /// The TextWriter to which text is redirected
        /// </summary>
        private TextWriter writer;
        #endregion

        #region Properties
        /// <summary>
        /// The TextWriter to which text is redirected
        /// </summary>
        public TextWriter Writer
        {
            get { return writer; }
            set
            {
                writer = value;

                if (writer != null && enabled)
                    StartCapture();
            }
        }

        /// <summary>
        /// Controls whether text is captured or not
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled != value)
                {
                    if (writer != null && enabled)
                        StopCapture();

                    enabled = value;

                    if (writer != null && enabled && DefaultThreshold != "Off")
                        StartCapture();
                }
            }
        }

        /// <summary>
        /// Returns the default threshold value, which represents
        /// the degree of verbosity of the output text stream.
        /// Returns "None" in the base class. Derived classes that
        /// support verbosity levels should override it.
        /// </summary>
        public virtual string DefaultThreshold
        {
            get { return "None"; }
        }
        #endregion

        #region Abstract Members
        /// <summary>
        /// Override this to perform whatever actions are needed
        /// to start capturing text and sending it to the Writer.
        /// </summary>
        protected abstract void StartCapture();

        /// <summary>
        /// Override this to perform whatever actions are needed
        /// to flush remaining output and stop capturing text.
        /// The Writer should not be changed, allowing capture
        /// to be restarted at a future point.
        /// </summary>
        protected abstract void StopCapture();
        #endregion
    }

}
