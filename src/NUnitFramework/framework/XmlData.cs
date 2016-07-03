// ***********************************************************************
// Copyright (c) 2009-2015 Charlie Poole
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
using System.Xml;

namespace NUnit.Framework
{
    /// <summary>
    /// Import XML data source
    /// </summary>
    public class XmlData : DataSource
    {


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        public XmlData(string source)
        {
            Name = source;
        }


        /// <summary>
        /// Retrieve data from source
        /// </summary>
        /// <returns>List of test data to map on target method</returns>
        public override IEnumerable GetData()
        {

            // open file and check that dimensions match that of parameters
            IList<object[]> testData = new List<object[]>();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Name);

            // select the root node and get child elements (rows)
            var dataRows = xmlDoc.SelectSingleNode("/*").ChildNodes;

            int N;
            if (RowsToRead == 0)
                N = dataRows.Count; // read all records
            else if (RowsToRead > 0 && RowsToRead >= dataRows.Count)
                N = dataRows.Count; // read all records
            else
                N = RowsToRead; // read subset

            for (int rowIdx = 0; rowIdx < N; rowIdx++)
            {
                int cols = dataRows[rowIdx].ChildNodes.Count;
                var paramList = new ArrayList(cols);

                for (int colIdx = 0; colIdx < cols; colIdx++)
                {
                    paramList.Add(dataRows[rowIdx].ChildNodes[colIdx].InnerText);
                }

                testData.Add(paramList.ToArray());
            }

            return testData;
        }
    }
}
