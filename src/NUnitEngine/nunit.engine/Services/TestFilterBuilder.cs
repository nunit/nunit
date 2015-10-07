// ***********************************************************************
// Copyright (c) 2013-2015 Charlie Poole
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
using System.Text;

namespace NUnit.Engine
{
    public class TestFilterBuilder : ITestFilterBuilder
    {
        private List<string> _testList = new List<string>();
        private List<string> _includedCategories = new List<string>();
        private List<string> _excludedCategories = new List<string>();
        private string _whereClause;

        /// <summary>
        /// Add a test to be selected
        /// </summary>
        /// <param name="fullName">The full name of the test, as created by NUnit</param>
        public void AddTest(string fullName)
        {
            _testList.Add(fullName);
        }

        /// <summary>
        /// Add a category to be included
        /// </summary>
        /// <param name="category">The category name</param>
        public void IncludeCategory(string category)
        {
            _includedCategories.Add(category);
        }

        /// <summary>
        /// Add a category to be excluded
        /// </summary>
        /// <param name="category">The category name</param>
        public void ExcludeCategory(string category)
        {
            _excludedCategories.Add(category);
        }

        /// <summary>
        /// Specify what is to be included by the filter using a where clause.
        /// </summary>
        /// <param name="whereClause">A where clause that will be parsed by NUnit to create the filter.</param>
        public void SelectWhere(string whereClause)
        {
            _whereClause = whereClause;
        }

        /// <summary>
        /// Get a TestFilter constructed according to the criteria specified by the other calls.
        /// </summary>
        /// <returns>A TestFilter.</returns>
        public TestFilter GetFilter()
        {
            var filter = new StringBuilder("<filter>");

            if (_testList.Count > 0)
            {
                if (_testList.Count > 1)
                    filter.Append("<or>");
                foreach (string test in _testList)
                    filter.AppendFormat("<test>{0}</test>", test);
                if (_testList.Count > 1)
                    filter.Append("</or>");
            }


            if (_includedCategories.Count > 0)
            {
                filter.Append("<cat>");
                bool needComma = false;
                foreach (string category in _includedCategories)
                {
                    if (needComma) filter.Append(',');
                    filter.Append(category);
                    needComma = true;
                }
                filter.Append("</cat>");
            }

            if (_excludedCategories.Count > 0)
            {
                filter.Append("<not><cat>");
                bool needComma = false;
                foreach (string category in _excludedCategories)
                {
                    if (needComma) filter.Append(',');
                    filter.Append(category);
                    needComma = true;
                }
                filter.Append("</cat></not>");
            }

            if (_whereClause != null)
                filter.Append(new TestSelectionParser().Parse(_whereClause));

            filter.Append("</filter>");

            return new TestFilter(filter.ToString());
        }
    }
}
