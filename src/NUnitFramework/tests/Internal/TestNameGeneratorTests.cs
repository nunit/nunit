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

using System;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Internal.Tests
{
    public class TestNameGeneratorTests
    {
        private TestMethod _simpleTest;
        private TestMethod _genericTest;

        [SetUp]
        public void InitializeMethodInfos()
        {
            Type thisType = GetType();
            var simpleMethod = thisType.GetMethod("TestMethod", BindingFlags.NonPublic | BindingFlags.Instance);
            var genericMethod = thisType.GetMethod("GenericTest", BindingFlags.NonPublic | BindingFlags.Instance);
            _simpleTest = new TestMethod(new MethodWrapper(thisType, simpleMethod));
            _genericTest = new TestMethod(new MethodWrapper(thisType, genericMethod));
            _simpleTest.Id = "THE_ID";
        }

        [TestCase("FIXED", ExpectedResult = "FIXED")]
        [TestCase("{m}", ExpectedResult = "TestMethod")]
        [TestCase("{n}", ExpectedResult = "NUnit.Framework.Internal.Tests")]
        [TestCase("{c}", ExpectedResult = "TestNameGeneratorTests")]
        [TestCase("{C}", ExpectedResult = "NUnit.Framework.Internal.Tests.TestNameGeneratorTests")]
        [TestCase("{M}", ExpectedResult = "NUnit.Framework.Internal.Tests.TestNameGeneratorTests.TestMethod")]
        [TestCase("{m}_SpecialCase", ExpectedResult = "TestMethod_SpecialCase")]
        [TestCase("{n}.{c}.{m}", ExpectedResult = "NUnit.Framework.Internal.Tests.TestNameGeneratorTests.TestMethod")]
        [TestCase("{x}", ExpectedResult = "{x}")]
        [TestCase("{n}.{c.{m}", ExpectedResult = "NUnit.Framework.Internal.Tests.{c.{m}")]
        [TestCase("{m}{a}", ExpectedResult = "TestMethod")]
        [TestCase("{i}", ExpectedResult="THE_ID")]
        public string SimpleTestNames(string pattern)
        {
            return new TestNameGenerator(pattern).GetDisplayName(_simpleTest);
        }

        [TestCase("{m}{a}", new object[] { 1, 2 }, ExpectedResult = "TestMethod(1,2)")]
        [TestCase("{m}{a:50}", new object[] { 1, 2 }, ExpectedResult = "TestMethod(1,2)")]
        [TestCase("{m}{a}", new object[] { "Now is the time for all good men to come to the aid of their country." },
            ExpectedResult = "TestMethod(\"Now is the time for all good men to come to the aid of their country.\")")]
        [TestCase("{m}{a:20}", new object[] { "Now is the time for all good men to come to the aid of their country." },
            ExpectedResult = "TestMethod(\"Now is the time f...\")")]
        [TestCase("{m}{a:40}", new object[] { "Now is the time for all good men to come to the aid of their country." },
            ExpectedResult = "TestMethod(\"Now is the time for all good men to c...\")")]
        [TestCase("{m}{a:100}", new object[] { "Now is the time for all good men to come to the aid of their country." },
            ExpectedResult = "TestMethod(\"Now is the time for all good men to come to the aid of their country.\")")]
        [TestCase("{m}{a:20}", new object[] { 42, "Now is the time for all good men to come to the aid of their country.", 5.2 },
            ExpectedResult = "TestMethod(42,\"Now is the time f...\",5.2d)")]
        [TestCase("{m}{a:20}%{i}", new object[] { 42, "Now is the time for all good men to come to the aid of their country.", 5.2 },
            ExpectedResult = "TestMethod(42,\"Now is the time f...\",5.2d)%THE_ID")]
        [TestCase("{m}({0})", new object[] { 1, 2, 3 }, ExpectedResult = "TestMethod(1)")]
        [TestCase("{m}({1})", new object[] { 1, 2, 3 }, ExpectedResult = "TestMethod(2)")]
        [TestCase("{m}({2})", new object[] { 1, 2, 3 }, ExpectedResult = "TestMethod(3)")]
        [TestCase("{m}({3})", new object[] { 1, 2, 3 }, ExpectedResult = "TestMethod()")]
        [TestCase("{m}({1:20})", new object[] { 42, "Now is the time for all good men to come to the aid of their country.", 5.2 },
            ExpectedResult = "TestMethod(\"Now is the time f...\")")]
        public string ParameterizedTests(string pattern, object[] args)
        {
            return new TestNameGenerator(pattern).GetDisplayName(_simpleTest, args);
        }

        [TestCase("FIXED", ExpectedResult="FIXED")]
        [TestCase("{m}",   ExpectedResult="GenericTest<T,U,V>")]
        [TestCase("{n}", ExpectedResult = "NUnit.Framework.Internal.Tests")]
        [TestCase("{c}", ExpectedResult = "TestNameGeneratorTests")]
        [TestCase("{C}", ExpectedResult = "NUnit.Framework.Internal.Tests.TestNameGeneratorTests")]
        [TestCase("{M}", ExpectedResult = "NUnit.Framework.Internal.Tests.TestNameGeneratorTests.GenericTest<T,U,V>")]
        [TestCase("{m}_SpecialCase", ExpectedResult = "GenericTest<T,U,V>_SpecialCase")]
        [TestCase("{n}.{c}.{m}", ExpectedResult = "NUnit.Framework.Internal.Tests.TestNameGeneratorTests.GenericTest<T,U,V>")]
        public string GenericTestNames(string pattern)
        {
            return new TestNameGenerator(pattern).GetDisplayName(_genericTest);
        }

        [TestCase("{x}", ExpectedResult = "{x}")]
        [TestCase("{xy}", ExpectedResult = "{xy}")]
        [TestCase("{x:}", ExpectedResult = "{x:}")]
        [TestCase("{x:50}", ExpectedResult = "{x:50}")]
        [TestCase("{n}.{c.{m}", ExpectedResult = "NUnit.Framework.Internal.Tests.{c.{m}")]
        [TestCase("{m}{a:X}", ExpectedResult = "TestMethod{a:X}")]
        [TestCase("{m}{0:X}", ExpectedResult = "TestMethod{0:X}")]
        [TestCase("{m}{a:}", ExpectedResult = "TestMethod{a:}")]
        [TestCase("{m}{0:}", ExpectedResult = "TestMethod{0:}")]
        public string ErrorInPattern(string pattern)
        {
            return new TestNameGenerator(pattern).GetDisplayName(_simpleTest);
        }

        [TestCase(double.MaxValue, ExpectedResult = "double.MaxValue")]
        [TestCase(double.MinValue, ExpectedResult = "double.MinValue")]
        [TestCase(double.NaN, ExpectedResult = "double.NaN")]
        [TestCase(double.PositiveInfinity, ExpectedResult = "double.PositiveInfinity")]
        [TestCase(double.NegativeInfinity, ExpectedResult = "double.NegativeInfinity")]
        [TestCase(float.MaxValue, ExpectedResult = "float.MaxValue")]
        [TestCase(float.MinValue, ExpectedResult = "float.MinValue")]
        [TestCase(float.NaN, ExpectedResult = "float.NaN")]
        [TestCase(float.PositiveInfinity, ExpectedResult = "float.PositiveInfinity")]
        [TestCase(float.NegativeInfinity, ExpectedResult = "float.NegativeInfinity")]
        [TestCase(int.MaxValue, ExpectedResult = "int.MaxValue")]
        [TestCase(int.MinValue, ExpectedResult = "int.MinValue")]
        [TestCase(uint.MaxValue, ExpectedResult = "uint.MaxValue")]
        [TestCase(uint.MinValue, ExpectedResult = "uint.MinValue")]
        [TestCase(long.MaxValue, ExpectedResult = "long.MaxValue")]
        [TestCase(long.MinValue, ExpectedResult = "long.MinValue")]
        [TestCase(ulong.MaxValue, ExpectedResult = "ulong.MaxValue")]
        [TestCase(ulong.MinValue, ExpectedResult = "ulong.MinValue")]
        [TestCase(short.MaxValue, ExpectedResult = "short.MaxValue")]
        [TestCase(short.MinValue, ExpectedResult = "short.MinValue")]
        [TestCase(ushort.MaxValue, ExpectedResult = "ushort.MaxValue")]
        [TestCase(ushort.MinValue, ExpectedResult = "ushort.MinValue")]
        [TestCase(byte.MaxValue, ExpectedResult = "byte.MaxValue")]
        [TestCase(byte.MinValue, ExpectedResult = "byte.MinValue")]
        [TestCase(sbyte.MaxValue, ExpectedResult = "sbyte.MaxValue")]
        [TestCase(sbyte.MinValue, ExpectedResult = "sbyte.MinValue")]
        public string SpecialNamedValues(object arg)
        {
            return new TestNameGenerator("{0}").GetDisplayName(_simpleTest, new[] { arg } );
        }

        #region Methods Used as Data

        private void TestMethod() { }

        private void GenericTest<T, U, V>() { }

        #endregion
    }
}
