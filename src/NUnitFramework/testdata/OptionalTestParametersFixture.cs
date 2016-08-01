// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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

#if !NETCF
using NUnit.Framework;

namespace NUnit.TestData
{
    public class OptionalTestParametersFixture
    {
        [TestCase()]
        public void MethodWithOptionalParams0(int x, int y = 1)
        {
        }

        [TestCase(10)]
        public void MethodWithOptionalParams1(int x, int y = 1)
        {
        }

        [TestCase(10, 20)]
        public void MethodWithOptionalParams2(int x, int y = 1)
        {
        }

        [TestCase(10, 20, 30)]
        public void MethodWithOptionalParams3(int x, int y = 1)
        {
        }

        [TestCase]
        public void MethodWithAllOptionalParams0(int x = 0, int y = 1)
        {
        }

        [TestCase(10)]
        public void MethodWithAllOptionalParams1(int x = 0, int y = 1)
        {
        }

        [TestCase(10, 20)]
        public void MethodWithAllOptionalParams2(int x = 0, int y = 1)
        {
        }

        [TestCase(10, 20, 30)]
        public void MethodWithAllOptionalParams3(int x = 0, int y = 1)
        {
        }
    }
}
#endif