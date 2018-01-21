// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.TestUtilities
{
    internal static class TestSuiteExtensions
    {
        public static TestSuite Containing(this TestSuite theSuite, string name)
        {
            return theSuite.Containing(new TestSuite(name));
        }

        public static TestSuite Containing(this TestSuite theSuite, params Type[] types)
        {
            foreach (var type in types)
                theSuite.Add(TestBuilder.MakeFixture(type));

            return theSuite;
        }

        public static TestSuite Containing(this TestSuite theSuite, params Test[] tests)
        {
            foreach (var test in tests)
                theSuite.Add(test);

            return theSuite;
        }

        public static TestSuite Parallelizable(this TestSuite theSuite)
        {
            theSuite.Properties.Set(PropertyNames.ParallelScope, ParallelScope.Self);

            return theSuite;
        }

        public static TestSuite NonParallelizable(this TestSuite theSuite)
        {
            theSuite.Properties.Set(PropertyNames.ParallelScope, ParallelScope.None);

            return theSuite;
        }
    }
}
