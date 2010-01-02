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

using System;
using NUnit.Core;
using NUnit.Framework.Api;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Utility class used to locate tests by name in a test tree
    /// </summary>
    public class TestFinder
    {
        public static Test Find(string name, Test test, bool recursive)
        {
            if (test.Tests != null)
            {
                foreach (Test child in test.Tests)
                {
                    if (child.TestName.Name == name)
                        return child;
                    if (recursive)
                    {
                        Test grandchild = Find(name, child, true);
                        if (grandchild != null)
                            return grandchild;
                    }
                }
            }

            return null;
        }

        public static TestResult Find(string name, TestResult result, bool recursive)
        {
            if (result.HasResults)
            {
                foreach (TestResult childResult in result.Results)
                {
                    if (childResult.Test.TestName.Name == name)
                        return childResult;

                    if (recursive)
                    {
                        TestResult r = Find(name, childResult, true);
                        if (r != null)
                            return r;
                    }
                }
            }

            return null;
        }

        private TestFinder() { }
    }
}
