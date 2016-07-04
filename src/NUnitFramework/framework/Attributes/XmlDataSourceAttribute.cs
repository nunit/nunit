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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;


namespace NUnit.Framework.Attributes
{
    /// <summary>
    /// Import data from XML document
    /// </summary>
    public class XmlDataAttribute : BaseTestCaseSourceAttribute
    {
        private XmlData _xml;

        /// <summary>
        /// Number of rows to read
        /// </summary>
        public int RowsToRead
        {
            get { return _xml.RowsToRead; }
            set { _xml.RowsToRead = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source"></param>
        public XmlDataAttribute(string source)
        {
            _xml = new XmlData(source);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        protected override IEnumerable GetTestCaseSource(IMethodInfo methodInfo)
        {
            if (!_xml.SourceFileExists)
            {
                string errMsg = String.Format("Unable to find source: {0}", _xml.Name);
                ReturnErrorAsParameter(errMsg);
            }

            // get dimensions from method parameters
            var paramInfo = methodInfo.GetParameters();
            var paramDim = paramInfo.Length;
            var dataFromQuery = _xml.GetData();

            // check if items were returned
            if (((IList)dataFromQuery).Count == 0)
                ReturnErrorAsParameter("No data returned. Please check file.");

            foreach (object[] data in dataFromQuery)
            {
                if (data.Length != paramDim)
                    ReturnErrorAsParameter(String.Format("Method parameters ({0}) and number of columns ({1}) differ. They need to be equal.", paramDim, data.Length));

                // Convert the identified columns to the type(s) of each method parameter
                var paramList = new ArrayList(data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    var paramType = paramInfo[i].ParameterType;
                    paramList.Add(Convert.ChangeType(data[i], paramType));
                }

                yield return paramList.ToArray();
            }
        }
    }
}
