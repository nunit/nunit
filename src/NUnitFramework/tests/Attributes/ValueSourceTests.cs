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
using System.Diagnostics;
using System.Linq;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class ValueSourceTests : ValueSourceMayBeInherited
    {
        [Test]
        public void ValueSourceCanBeStaticProperty(
            [ValueSource("StaticProperty")] string source)
        {
            Assert.AreEqual("StaticProperty", source);
        }

        static IEnumerable StaticProperty
        {
            get
            {
                yield return "StaticProperty";
            }
        }

        [Test]
        public void ValueSourceCanBeInheritedStaticProperty(
            [ValueSource("InheritedStaticProperty")] bool source)
        {
            Assert.AreEqual(true, source);
        }

        [Test]
        public void ValueSourceMayNotBeInstanceProperty()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(GetType(), "MethodWithValueSourceInstanceProperty");
            Assert.That(result.Children.ToArray()[0].ResultState, Is.EqualTo(ResultState.NotRunnable));
        }

        public void MethodWithValueSourceInstanceProperty(
            [ValueSource("InstanceProperty")] string source)
        {
            Assert.AreEqual("InstanceProperty", source);
        }

        IEnumerable InstanceProperty
        {
            get { return new object[] { "InstanceProperty" }; }
        }

        [Test]
        public void ValueSourceCanBeStaticMethod(
            [ValueSource("StaticMethod")] string source)
        {
            Assert.AreEqual("StaticMethod", source);
        }

        static IEnumerable StaticMethod()
        {
            return new object[] { "StaticMethod" };
        }

        [Test]
        public void ValueSourceMayNotBeInstanceMethod()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(GetType(), "MethodWithValueSourceInstanceMethod");
            Assert.That(result.Children.ToArray()[0].ResultState, Is.EqualTo(ResultState.NotRunnable));
        }

        public void MethodWithValueSourceInstanceMethod(
            [ValueSource("InstanceMethod")] string source)
        {
            Assert.AreEqual("InstanceMethod", source);
        }

        IEnumerable InstanceMethod()
        {
            return new object[] { "InstanceMethod" };
        }

        [Test]
        public void ValueSourceCanBeStaticField(
            [ValueSource("StaticField")] string source)
        {
            Assert.AreEqual("StaticField", source);
        }

        internal static object[] StaticField = { "StaticField" };

        [Test]
        public void ValueSourceMayNotBeInstanceField()
        {
            var result = TestBuilder.RunParameterizedMethodSuite(GetType(), "MethodWithValueSourceInstanceField");
            Assert.That(result.Children.ToArray ()[0].ResultState, Is.EqualTo(ResultState.NotRunnable));
        }

        public void MethodWithValueSourceInstanceField(
            [ValueSource("InstanceField")] string source)
        {
            Assert.AreEqual("InstanceField", source);
        }

        internal object[] InstanceField = { "InstanceField" };

        [Test, Sequential]
        public void MultipleArguments(
            [ValueSource("Numerators")] int n, 
            [ValueSource("Denominators")] int d, 
            [ValueSource("Quotients")] int q)
        {
            Assert.AreEqual(q, n / d);
        }

        internal static int[] Numerators = new int[] { 12, 12, 12 };
        internal static int[] Denominators = new int[] { 3, 4, 6 };
        internal static int[] Quotients = new int[] { 4, 3, 2 };

        [Test, Sequential]
        public void ValueSourceMayBeInAnotherClass(
            [ValueSource(typeof(DivideDataProvider), "Numerators")] int n,
            [ValueSource(typeof(DivideDataProvider), "Denominators")] int d,
            [ValueSource(typeof(DivideDataProvider), "Quotients")] int q)
        {
            Assert.AreEqual(q, n / d);
        }

        private class DivideDataProvider
        {
            internal static int[] Numerators = new int[] { 12, 12, 12 };
            internal static int[] Denominators = new int[] { 3, 4, 6 };
            internal static int[] Quotients = new int[] { 4, 3, 2 };
        }

        [Test]
        public void ValueSourceMayBeGeneric(
            [ValueSourceAttribute(typeof(ValueProvider), "IntegerProvider")] int val)
        {
            Assert.That(2 * val, Is.EqualTo(val + val));
        }

        public class ValueProvider
        {
            public static IEnumerable<int> IntegerProvider()
            {
                List<int> dataList = new List<int>();

                dataList.Add(1);
                dataList.Add(2);
                dataList.Add(4);
                dataList.Add(8);

                return dataList;
            }

            public static IEnumerable<int> ForeignNullResultProvider()
            {
                return null;
            }
        }

        public static string NullSource = null;

        public static IEnumerable<int> NullDataSourceProvider()
        {
            return null;
        }

        public static IEnumerable<int> NullDataSourceProperty
        {
            get { return null; }
        }

        [Test, Explicit("Null or nonexisting data sources definitions should not prevent other tests from run #1121")]
        public void ValueSourceMayNotBeNull(
            [ValueSource("NullSource")] string nullSource,
            [ValueSource("NullDataSourceProvider")] string nullDataSourceProvided,
            [ValueSource(typeof(ValueProvider), "ForeignNullResultProvider")] string nullDataSourceProvider,
            [ValueSource("NullDataSourceProperty")] int nullDataSourceProperty,
            [ValueSource("SomeNonExistingMemberSource")] int nonExistingMember)
        {
            Assert.Fail();
        }

        [Test]
        public void ValueSourceAttributeShouldThrowInsteadOfReturningNull()
        {
            var method = new MethodWrapper(GetType(), "ValueSourceMayNotBeNull");
            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                var dataSource = parameter.GetCustomAttributes<IParameterDataSource>(false)[0];
                Assert.Throws<InvalidDataSourceException>(() => dataSource.GetData(parameter)); 
            }
        }
    }

    public class ValueSourceMayBeInherited
    {
        protected static IEnumerable<bool> InheritedStaticProperty
        {
            get { yield return true; }
        }
    }
}
