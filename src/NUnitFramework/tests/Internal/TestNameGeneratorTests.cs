// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    public class TestNameGeneratorTests
    {
        private TestMethod _simpleTest;
        private TestMethod _simpleTestWithArgs;
        private TestMethod _genericTest;

        [SetUp]
        public void InitializeMethodInfos()
        {
            Type thisType = GetType();
            _simpleTest = new TestMethod(new MethodWrapper(thisType, nameof(TestMethod)));
            _simpleTestWithArgs = new TestMethod(new MethodWrapper(thisType, nameof(TestMethodWithArgs)));
            _genericTest = new TestMethod(new MethodWrapper(thisType, nameof(GenericTest)));
            _simpleTest.Id = "THE_ID";
        }

        [TestCase("FIXED", ExpectedResult = "FIXED")]
        [TestCase("{m}", ExpectedResult = "TestMethod")]
        [TestCase("{n}", ExpectedResult = "NUnit.Framework.Tests.Internal")]
        [TestCase("{c}", ExpectedResult = "TestNameGeneratorTests")]
        [TestCase("{C}", ExpectedResult = "NUnit.Framework.Tests.Internal.TestNameGeneratorTests")]
        [TestCase("{M}", ExpectedResult = "NUnit.Framework.Tests.Internal.TestNameGeneratorTests.TestMethod")]
        [TestCase("{m}_SpecialCase", ExpectedResult = "TestMethod_SpecialCase")]
        [TestCase("{n}.{c}.{m}", ExpectedResult = "NUnit.Framework.Tests.Internal.TestNameGeneratorTests.TestMethod")]
        [TestCase("{x}", ExpectedResult = "{x}")]
        [TestCase("{n}.{c.{m}", ExpectedResult = "NUnit.Framework.Tests.Internal.{c.{m}")]
        [TestCase("{m}{a}", ExpectedResult = "TestMethod")]
        [TestCase("{i}", ExpectedResult = "THE_ID")]
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
        [TestCase("{m}({0})", new object[] { "Now is the time for all good men to come to the aid of their country." },
            ExpectedResult = "TestMethod(\"Now is the time for all good men to come to the aid of their country.\")")]
        public string ParameterizedTests(string pattern, object[] args)
        {
            return new TestNameGenerator(pattern).GetDisplayName(_simpleTest, args);
        }

        [TestCase("{m}{p}", new object[] { 1 }, ExpectedResult = "TestMethodWithArgs(a: 1)")]
        [TestCase("{m}{p}", new object[] { 1, 2 }, ExpectedResult = "TestMethodWithArgs(a: 1, b: 2)")]
        [TestCase("{m}{p}", new object[] { 1, 2, 3 }, ExpectedResult = "TestMethodWithArgs(a: 1, b: 2, c: 3)")]
        [TestCase("{m}{p}", new object[] { 1, 2, 3, 4 }, ExpectedResult = "TestMethodWithArgs(a: 1, b: 2, c: 3, 4)")]
        public string ParameterizedTestsWithArgs(string pattern, object[] args)
        {
            return new TestNameGenerator(pattern).GetDisplayName(_simpleTestWithArgs, args);
        }

        [TestCase("FIXED", ExpectedResult = "FIXED")]
        [TestCase("{m}", ExpectedResult = "GenericTest<T1,T2,T3>")]
        [TestCase("{n}", ExpectedResult = "NUnit.Framework.Tests.Internal")]
        [TestCase("{c}", ExpectedResult = "TestNameGeneratorTests")]
        [TestCase("{C}", ExpectedResult = "NUnit.Framework.Tests.Internal.TestNameGeneratorTests")]
        [TestCase("{M}", ExpectedResult = "NUnit.Framework.Tests.Internal.TestNameGeneratorTests.GenericTest<T1,T2,T3>")]
        [TestCase("{m}_SpecialCase", ExpectedResult = "GenericTest<T1,T2,T3>_SpecialCase")]
        [TestCase("{n}.{c}.{m}", ExpectedResult = "NUnit.Framework.Tests.Internal.TestNameGeneratorTests.GenericTest<T1,T2,T3>")]
        public string GenericTestNames(string pattern)
        {
            return new TestNameGenerator(pattern).GetDisplayName(_genericTest);
        }

        [TestCase("{x}", ExpectedResult = "{x}")]
        [TestCase("{xy}", ExpectedResult = "{xy}")]
        [TestCase("{x:}", ExpectedResult = "{x:}")]
        [TestCase("{x:50}", ExpectedResult = "{x:50}")]
        [TestCase("{n}.{c.{m}", ExpectedResult = "NUnit.Framework.Tests.Internal.{c.{m}")]
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
            return new TestNameGenerator("{0}").GetDisplayName(_simpleTest, new[] { arg });
        }

        #region Methods Used as Data

        private void TestMethod()
        {
        }

        private void TestMethodWithArgs(int a, int b, int c = 0)
        {
        }

        private void GenericTest<T1, T2, T3>()
        {
        }

        #endregion
    }
}
