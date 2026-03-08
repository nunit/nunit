// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Attributes
{
    [TestFixture]
    public class TestCaseDataTests : TestSourceMayBeInherited
    {
        #region Create<T>

        private static IEnumerable<TestCaseData<Type, object?>> Create_T_TestCases()
        {
            yield return TestCaseData.Create(typeof(int), (object?)1);
            yield return TestCaseData.Create(typeof(double), (object?)1.0);
            yield return TestCaseData.Create(typeof(string), (object?)"test");
            yield return TestCaseData.Create(typeof(TestType), (object?)new TestType());
        }

        [Test]
        [TestCaseSource(nameof(Create_T_TestCases))]
        public void Create_T_yields_TestCaseData_T(Type t, object? argument)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(Create_T_yields_TestCaseData_T_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t);

            method.Invoke(this, [argument]);
        }

        private void Create_T_yields_TestCaseData_T_impl<T>(T argument)
        {
            // Act
            TestCaseData<T> result = TestCaseData.Create(argument);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseData<T>>());
        }

        #endregion

        #region Create<T1, T2>

        private static IEnumerable<TestCaseData<Type, object?, Type, object?>> Create_T1_T2_TestCases()
        {
            yield return TestCaseData.Create(
                typeof(int), (object?)1,
                typeof(int), (object?)2);
            yield return TestCaseData.Create(
                typeof(double), (object?)1.0,
                typeof(short), (object?)short.MinValue);
            yield return TestCaseData.Create(
                typeof(string), (object?)"test",
                typeof(object), (object?)new TestType());

            int? test = null;

            yield return TestCaseData.Create(
                typeof(int?), (object?)test,
                typeof(TestType), (object?)new TestType());
        }

        [Test]
        [TestCaseSource(nameof(Create_T1_T2_TestCases))]
        public void Create_T1_T2_yields_TestCaseData_T1_T2(Type t1, object? argument1, Type t2, object? argument2)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(Create_T1_T2_yields_TestCaseData_T1_T2_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t1, t2);

            method.Invoke(this, [argument1, argument2]);
        }

        private void Create_T1_T2_yields_TestCaseData_T1_T2_impl<T1, T2>(T1 argument1, T2 argument2)
        {
            // Act
            TestCaseData<T1, T2> result = TestCaseData.Create(argument1, argument2);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseData<T1, T2>>());
        }

        #endregion

        #region Create<T1, T2, T3>

        private static IEnumerable<TestCaseData> Create_T1_T2_T3_TestCases()
        {
            yield return new TestCaseData(
                typeof(int), 1,
                typeof(int), 2,
                typeof(int), 3);
            yield return new TestCaseData(
                typeof(double), 1.0,
                typeof(short), short.MinValue,
                typeof(string), "test");
            yield return new TestCaseData(
                typeof(string), "test",
                typeof(object), new TestType(),
                typeof(TestType), new TestType());
            yield return new TestCaseData(
                typeof(int?), 1,
                typeof(DayOfWeek), DayOfWeek.Wednesday,
                typeof(bool), true);
        }

        [Test]
        [TestCaseSource(nameof(Create_T1_T2_T3_TestCases))]
        public void Create_T1_T2_T3_yields_TestCaseData_T1_T2_T3(Type t1, object? argument1, Type t2, object? argument2, Type t3, object? argument3)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(Create_T1_T2_T3_yields_TestCaseData_T1_T2_T3_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t1, t2, t3);

            method.Invoke(this, [argument1, argument2, argument3]);
        }

        private void Create_T1_T2_T3_yields_TestCaseData_T1_T2_T3_impl<T1, T2, T3>(T1 argument1, T2 argument2, T3 argument3)
        {
            // Act
            TestCaseData<T1, T2, T3> result = TestCaseData.Create(argument1, argument2, argument3);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseData<T1, T2, T3>>());
        }

        #endregion

        #region Create<T1, T2, T3, T4>

        private static IEnumerable<TestCaseData> Create_T1_T2_T3_T4_TestCases()
        {
            yield return new TestCaseData(
                typeof(int), 1,
                typeof(int), 2,
                typeof(int), 3,
                typeof(int), 4);
            yield return new TestCaseData(
                typeof(double), 1.0,
                typeof(DayOfWeek), DayOfWeek.Wednesday,
                typeof(string), "test",
                typeof(TestType), new TestType());
        }

        [Test]
        [TestCaseSource(nameof(Create_T1_T2_T3_T4_TestCases))]
        public void Create_T1_T2_T3_T4_yields_TestCaseData_T1_T2_T3_T4(Type t1, object? argument1, Type t2, object? argument2, Type t3, object? argument3, Type t4, object? argument4)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(Create_T1_T2_T3_T4_yields_TestCaseData_T1_T2_T3_T4_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t1, t2, t3, t4);

            method.Invoke(this, [argument1, argument2, argument3, argument4]);
        }

        private void Create_T1_T2_T3_T4_yields_TestCaseData_T1_T2_T3_T4_impl<T1, T2, T3, T4>(T1 argument1, T2 argument2, T3 argument3, T4 argument4)
        {
            // Act
            TestCaseData<T1, T2, T3, T4> result = TestCaseData.Create(argument1, argument2, argument3, argument4);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseData<T1, T2, T3, T4>>());
        }

        #endregion

        #region Create<T1, T2, T3, T4, T5>

        private static IEnumerable<TestCaseData> Create_T1_T2_T3_T4_T5_TestCases()
        {
            yield return new TestCaseData(
                typeof(int), 1,
                typeof(int), 2,
                typeof(int), 3,
                typeof(int), 4,
                typeof(int), 5);
            yield return new TestCaseData(
                typeof(double), 1.0,
                typeof(DayOfWeek), DayOfWeek.Wednesday,
                typeof(string), "test",
                typeof(TestType), new TestType(),
                typeof(bool?), null);
        }

        [Test]
        [TestCaseSource(nameof(Create_T1_T2_T3_T4_T5_TestCases))]
        public void Create_T1_T2_T3_T4_T5_yields_TestCaseData_T1_T2_T3_T4_T5(Type t1, object? argument1, Type t2, object? argument2, Type t3, object? argument3, Type t4, object? argument4, Type t5, object? argument5)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(Create_T1_T2_T3_T4_T5_yields_TestCaseData_T1_T2_T3_T4_T5_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t1, t2, t3, t4, t5);

            method.Invoke(this, [argument1, argument2, argument3, argument4, argument5]);
        }

        private void Create_T1_T2_T3_T4_T5_yields_TestCaseData_T1_T2_T3_T4_T5_impl<T1, T2, T3, T4, T5>(T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5)
        {
            // Act
            TestCaseData<T1, T2, T3, T4, T5> result = TestCaseData.Create(argument1, argument2, argument3, argument4, argument5);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseData<T1, T2, T3, T4, T5>>());
        }

        #endregion

        #region TestCaseData<T>.Returns

        [TestCaseSource(nameof(Create_T_TestCases))]
        public void TestCaseData_T_Returns_yields_TestCaseDataWithReturn_T_TReturn(Type t, object? argument)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(TestCaseData_T_Returns_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t);

            method.Invoke(this, [argument]);
        }

        private void TestCaseData_T_Returns_impl<T>(T argument)
        {
            // Arrange
            var sut = TestCaseData.Create(argument);
            int expectedResult = 1;

            // Act
            TestCaseDataWithReturn<T, int> result = sut.Returns(expectedResult);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseDataWithReturn<T, int>>());
            Assert.That(result.Arguments[0], Is.EqualTo(argument));
            Assert.That(result.ExpectedResult, Is.EqualTo(expectedResult));
        }

        #endregion

        #region TestCaseData<T1, T2>.Returns

        [TestCaseSource(nameof(Create_T1_T2_TestCases))]
        public void TestCaseData_T1_T2_Returns_yields_TestCaseDataWithReturn_T1_T2_TReturn(Type t1, object? argument1, Type t2, object? argument2)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(TestCaseData_T1_T2_Returns_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t1, t2);

            method.Invoke(this, [argument1, argument2]);
        }

        private void TestCaseData_T1_T2_Returns_impl<T1, T2>(T1 argument1, T2 argument2)
        {
            // Arrange
            var sut = TestCaseData.Create(argument1, argument2);
            int expectedResult = 1;

            // Act
            TestCaseDataWithReturn<T1, T2, int> result = sut.Returns(expectedResult);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseDataWithReturn<T1, T2, int>>());
            Assert.That(result.Arguments[0], Is.EqualTo(argument1));
            Assert.That(result.Arguments[1], Is.EqualTo(argument2));
            Assert.That(result.ExpectedResult, Is.EqualTo(expectedResult));
        }

        #endregion

        #region TestCaseData<T1, T2, T3>.Returns

        [TestCaseSource(nameof(Create_T1_T2_T3_TestCases))]
        public void TestCaseData_T1_T2_T3_Returns_yields_TestCaseDataWithReturn_T1_T2_T3_TReturn(Type t1, object? argument1, Type t2, object? argument2, Type t3, object? argument3)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(TestCaseData_T1_T2_T3_Returns_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t1, t2, t3);

            method.Invoke(this, [argument1, argument2, argument3]);
        }

        private void TestCaseData_T1_T2_T3_Returns_impl<T1, T2, T3>(T1 argument1, T2 argument2, T3 argument3)
        {
            // Arrange
            var sut = TestCaseData.Create(argument1, argument2, argument3);
            int expectedResult = 1;

            // Act
            TestCaseDataWithReturn<T1, T2, T3, int> result = sut.Returns(expectedResult);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseDataWithReturn<T1, T2, T3, int>>());
            Assert.That(result.Arguments[0], Is.EqualTo(argument1));
            Assert.That(result.Arguments[1], Is.EqualTo(argument2));
            Assert.That(result.Arguments[2], Is.EqualTo(argument3));
            Assert.That(result.ExpectedResult, Is.EqualTo(expectedResult));
        }

        #endregion

        #region TestCaseData<T1, T2, T3, T4>.Returns

        [TestCaseSource(nameof(Create_T1_T2_T3_T4_TestCases))]
        public void TestCaseData_T1_T2_T3_T4_Returns_yields_TestCaseDataWithReturn_T1_T2_T3_T4_TReturn(Type t1, object? argument1, Type t2, object? argument2, Type t3, object? argument3, Type t4, object? argument4)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(TestCaseData_T1_T2_T3_T4_Returns_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t1, t2, t3, t4);

            method.Invoke(this, [argument1, argument2, argument3, argument4]);
        }

        private void TestCaseData_T1_T2_T3_T4_Returns_impl<T1, T2, T3, T4>(T1 argument1, T2 argument2, T3 argument3, T4 argument4)
        {
            // Arrange
            var sut = TestCaseData.Create(argument1, argument2, argument3, argument4);
            int expectedResult = 1;

            // Act
            TestCaseDataWithReturn<T1, T2, T3, T4, int> result = sut.Returns(expectedResult);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseDataWithReturn<T1, T2, T3, T4, int>>());
            Assert.That(result.Arguments[0], Is.EqualTo(argument1));
            Assert.That(result.Arguments[1], Is.EqualTo(argument2));
            Assert.That(result.Arguments[2], Is.EqualTo(argument3));
            Assert.That(result.Arguments[3], Is.EqualTo(argument4));
            Assert.That(result.ExpectedResult, Is.EqualTo(expectedResult));
        }

        #endregion

        #region TestCaseData<T1, T2, T3, T4, T5>.Returns

        [TestCaseSource(nameof(Create_T1_T2_T3_T4_T5_TestCases))]
        public void TestCaseData_T1_T2_T3_T4_T5_Returns_yields_TestCaseDataWithReturn_T1_T2_T3_T4_T5_TReturn(Type t1, object? argument1, Type t2, object? argument2, Type t3, object? argument3, Type t4, object? argument4, Type t5, object? argument5)
        {
            var methodDeclaration = GetType()
                .GetMethod(nameof(TestCaseData_T1_T2_T3_T4_T5_Returns_impl), BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.That(methodDeclaration, Is.Not.Null);

            var method = methodDeclaration.MakeGenericMethod(t1, t2, t3, t4, t5);

            method.Invoke(this, [argument1, argument2, argument3, argument4, argument5]);
        }

        private void TestCaseData_T1_T2_T3_T4_T5_Returns_impl<T1, T2, T3, T4, T5>(T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5)
        {
            // Arrange
            var sut = TestCaseData.Create(argument1, argument2, argument3, argument4, argument5);
            int expectedResult = 1;

            // Act
            TestCaseDataWithReturn<T1, T2, T3, T4, T5, int> result = sut.Returns(expectedResult);

            // Assert
            Assert.That(result, Is.TypeOf<TestCaseDataWithReturn<T1, T2, T3, T4, T5, int>>());
            Assert.That(result.Arguments[0], Is.EqualTo(argument1));
            Assert.That(result.Arguments[1], Is.EqualTo(argument2));
            Assert.That(result.Arguments[2], Is.EqualTo(argument3));
            Assert.That(result.Arguments[3], Is.EqualTo(argument4));
            Assert.That(result.Arguments[4], Is.EqualTo(argument5));
            Assert.That(result.ExpectedResult, Is.EqualTo(expectedResult));
        }

        #endregion

        private class TestType
        {
        }
    }
}
