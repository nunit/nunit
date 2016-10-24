// ***********************************************************************
// Copyright (c) 2008-2016 Charlie Poole
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

using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.Framework
{
    /// <summary>
    /// DataSource housing the test data
    /// </summary>
    public abstract class DataSource
    {
        private int _rowsToRead = 0;

        /// <summary>
        /// The number of rows to read from the source.
        /// </summary>
        /// <remarks>The default is 0 which corresponds to all rows. If a number greater than zero is specified, it reads that number of rows or until the end of the source, which ever is first.</remarks>
        public int RowsToRead
        {
            get
            {
                return _rowsToRead;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Cannot set Rows to Read less than zero", "RowsToRead");
                else
                    _rowsToRead = value;
            }
        }

        /// <summary>
        /// The location of the data (file, database connection string)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The set of values from which test methods will be constructed
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable GetData();

        /// <summary>
        /// Whether the source file exists
        /// </summary>
        public bool SourceFileExists
        {
            get
            {
                return File.Exists(Name);
            }
        }
    }
}
