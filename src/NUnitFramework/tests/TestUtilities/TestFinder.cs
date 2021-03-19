// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.TestUtilities
{
    /// <summary>
    /// Utility class used to locate tests by name in a test suite
    /// </summary>
    public class TestFinder
    {
        public static Test Find(string name, TestSuite suite, bool recursive)
        {
            foreach (Test child in suite.Tests)
            {
                if (child.Name == name)
                    return child;
                if (recursive)
                {
                    TestSuite childSuite = child as TestSuite;
                    if (childSuite != null)
                    {
                        Test grandchild = Find(name, childSuite, true);
                        if (grandchild != null)
                            return grandchild;
                    }
                }
            }

            return null;
        }

        public static ITestResult Find(string name, ITestResult result, bool recursive)
        {
            if (result.HasChildren)
            {
                foreach (var childResult in result.Children)
                {
                    if (childResult.Name == name)
                        return childResult;

                    if (recursive && childResult.HasChildren)
                    {
                        ITestResult r = Find(name, childResult, true);
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
