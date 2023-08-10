// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.TestUtilities
{
    /// <summary>
    /// Utility class used to locate tests by name in a test suite
    /// </summary>
    public class TestFinder
    {
        public static Test? Find(string name, TestSuite suite, bool recursive)
        {
            foreach (Test child in suite.Tests)
            {
                if (child.Name == name)
                    return child;
                if (recursive)
                {
                    if (child is TestSuite childSuite)
                    {
                        Test? grandchild = Find(name, childSuite, true);
                        if (grandchild is not null)
                            return grandchild;
                    }
                }
            }

            return null;
        }

        public static ITestResult? Find(string name, ITestResult result, bool recursive)
        {
            if (result.HasChildren)
            {
                foreach (var childResult in result.Children)
                {
                    if (childResult.Name == name)
                        return childResult;

                    if (recursive && childResult.HasChildren)
                    {
                        ITestResult? r = Find(name, childResult, true);
                        if (r is not null)
                            return r;
                    }
                }
            }

            return null;
        }

        private TestFinder() { }
    }
}
