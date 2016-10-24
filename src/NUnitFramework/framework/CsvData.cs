// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using System.IO;
using System.ComponentModel;

namespace NUnit.Framework
{
    /// <summary>
    /// Import a Comma Separated Value data source
    /// </summary>
    public class CsvData : DataSource
    {

        /// <summary>
        /// The default character to use as a delimiter
        /// </summary>
        public string Delimiter {get; set;}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Filename of CSV file</param>
        public CsvData(string source): this(source, ",") { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">Filename of CSV file</param>
        /// <param name="delimiter">data delimiter. Defaults to comma (",")</param>
        public CsvData(string source, string delimiter) 
        {
            Name = source;
            Delimiter = delimiter;
        }

        /// <summary>
        /// Retrieve data from source
        /// </summary>
        /// <returns>List of test data to map on target method</returns>
        public override IEnumerable GetData()
        {
            // open file and check that dimensions match that of parameters
            IList<object[]> testData = new List<object[]>();

            // iterate through each row and generate the list of data parameters
            using (var sr = File.OpenText(Name))
            {
                string lineData = string.Empty;
                int lineIdx = 1;
                while ((lineData = sr.ReadLine()) != null)
                {
                    // determine if we need to stop before the end of the file
                    if (RowsToRead > 0 && lineIdx > RowsToRead)
                        break;

                    // skip empty lines as denoted by a newline character
                    if (lineData == string.Empty || lineData == Environment.NewLine)
                        continue;
                    
                    // once we have a valid (non-empty) string I continue to process
                    var csv = lineData.Split(new string[] { Delimiter}, StringSplitOptions.None);

                    var formatted_csv = stripExtraCharacters(csv);

                    testData.Add(formatted_csv);
                    lineIdx++;
                }
            }
            return testData;
        }

        /// <summary>
        /// Removes extra characters in CSV file.
        /// Based on the method signature, convert each column element respectively. For
        /// elements that have extra quotes(for encapsulation), we strip those out and
        /// include data(mainly strings)
        /// 
        /// This accounts for the following cases:
        /// 2,3,4
        /// "2","3","4"
        /// "a","b",c
        /// </summary>
        /// <param name="s">identified csv row</param>
        /// <returns>array with extra characters removed</returns>
        private string[] stripExtraCharacters(string[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = s[i].Trim(new char[] { '\'', '"', ' ' });
            }

            return s;
        }
    }
}
