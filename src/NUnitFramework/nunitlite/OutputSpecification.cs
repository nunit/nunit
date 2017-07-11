// ***********************************************************************
// Copyright (c) 2011 Charlie Poole, Rob Prouse
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

namespace NUnit.Common
{
    /// <summary>
    /// OutputSpecification encapsulates a file output path and format
    /// for use in saving the results of a run.
    /// </summary>
    public class OutputSpecification
    {
        #region Constructor

        /// <summary>
        /// Construct an OutputSpecification from an option value.
        /// </summary>
        /// <param name="spec">The option value string.</param>
        public OutputSpecification(string spec)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec), "Output spec may not be null.");

            string[] parts = spec.Split(';');

            if (parts.Length > 2)
                throw new ArgumentException($"Invalid output spec: {spec}.");

            this.OutputPath = parts[0];

            if (parts.Length == 1)
                return;

            string[] opt = parts[1].Split('=');

            if (opt.Length != 2 || opt[0].Trim() != "format")
                throw new ArgumentException($"Invalid output spec: {spec}.");

            this.Format = opt[1].Trim();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the path to which output will be written
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// Gets the name of the format to be used
        /// </summary>
        public string Format { get; } = "nunit3";

        #endregion
    }
}
