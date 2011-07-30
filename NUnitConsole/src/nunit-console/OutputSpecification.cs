// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

namespace NUnit.ConsoleRunner
{
    public class OutputSpecification
    {
        #region Private Fields

        private string output;
        private string format;
        private string transform;

        #endregion

        #region Constructor

        public OutputSpecification(string spec)
        {
            if (spec == null)
                throw new NullReferenceException("Output spec may not be null");

            string[] parts = spec.Split(';');
            this.output = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string[] opt = parts[i].Split('=');

                if (opt.Length != 2)
                    throw new ArgumentException();

                switch (opt[0].Trim())
                {
                    case "format":
                        string fmt = opt[1].Trim();

                        if (this.format != null && this.format != fmt)
                            throw new ArgumentException(
                                string.Format("Conflicting format options: {0}", spec));

                        this.format = fmt;
                        break;

                    case "transform":
                        string val = opt[1].Trim();

                        if (this.transform != null && this.transform != val)
                            throw new ArgumentException(
                                string.Format("Conflicting transform options: {0}", spec));

                        if (this.format != null && this.format != "user")
                            throw new ArgumentException(
                                string.Format("Conflicting format options: {0}", spec));

                        this.format = "user";
                        this.transform = opt[1].Trim();
                        break;
                }
            }

            if (format == null)
                format = "nunit3";
        }

        #endregion

        #region Properties

        public string OutputPath
        {
            get { return output; }
        }

        public string Format
        {
            get { return format; }
        }

        public string Transform
        {
            get { return transform; }
        }

        #endregion
    }
}
