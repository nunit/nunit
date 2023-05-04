// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NUnit.Framework.Internal
{
    public class GenericMethodHelperTests
    {
        private static TestCaseData[] TypeArgData = new[] {
            new TestCaseData(nameof(MethodWithOneTypeAndOneParameter),
                ArgList(42),
                TypeArgs<int>()),
            new TestCaseData(nameof(MethodWithOneTypeAndTwoParameters),
                ArgList(42, 99),
                TypeArgs<int>()),
            new TestCaseData(nameof(MethodWithOneTypeAndTwoParameters),
                ArgList(42.0, 99.0),
                TypeArgs<double>()),
            new TestCaseData(nameof(MethodWithOneTypeAndTwoParameters),
                ArgList(42, 99.0),
                TypeArgs<double>()),
            new TestCaseData(nameof(MethodWithOneTypeAndTwoParameters),
                ArgList(42.0, 99),
                TypeArgs<double>()),
            new TestCaseData(nameof(MethodWithOneTypeAndThreeParameters),
                ArgList(42, -1, 7),
                TypeArgs<int>()),
            new TestCaseData(nameof(MethodWithTwoTypesAndTwoParameters),
                ArgList(42, "Answer"),
                TypeArgs<int,string>()),
            new TestCaseData(nameof(MethodWithTwoTypesAndTwoParameters_Reversed),
                ArgList("Answer", 42),
                TypeArgs<int,string>()),
            new TestCaseData(nameof(MethodWithTwoTypesAndThreeParameters),
                ArgList(42, "Answer", 42),
                TypeArgs<int,string>()),
            new TestCaseData(nameof(MethodWithTwoTypesAndFourParameters),
                ArgList("Question", 1, "Answer", 42),
                TypeArgs<int,string>()),
            new TestCaseData(nameof(MethodWithThreeTypes_Order123),
                ArgList(42, 42.0, "forty-two"),
                TypeArgs<int,double,string>()),
            new TestCaseData(nameof(MethodWithThreeTypes_Order132),
                ArgList(42, "forty-two", 42.0),
                TypeArgs<int,double,string>()),
            new TestCaseData(nameof(MethodWithThreeTypes_Order321),
                ArgList("forty-two", 42.0, 42),
                TypeArgs<int,double,string>()),
            new TestCaseData(nameof(MethodWithThreeTypes_Order213),
                ArgList(42.0, 42, "forty-two"),
                TypeArgs<int,double,string>()),
            new TestCaseData(nameof(MethodWithOneTypeAndOneParameter),
                ArgList(new[] { 1, 2, 3 }),
                TypeArgs<int[]>()),
            new TestCaseData(nameof(MethodWithGenericListOfType),
                ArgList(new List<int>()),
                TypeArgs<int>()),
            new TestCaseData(nameof(MethodWithGenericListOfType),
                ArgList(new LinkedList<int>()),
                null),
            new TestCaseData(nameof(MethodWithGenericListOfLists),
                ArgList(new List<List<int>>()),
                TypeArgs<int>()),
            new TestCaseData(nameof(MethodWithGenericEnumerableOfType),
                ArgList(new List<int>(new[] { 1, 2, 3 })),
                TypeArgs<int>()),
            new TestCaseData(nameof(MethodWithGenericEnumerableOfType),
                ArgList(new[] { 1, 2, 3 }),
                TypeArgs<int>()),
            new TestCaseData(nameof(MethodWithGenericEnumerableOfTypeAsSecondArg),
                ArgList("X", new int[] { } ),
                TypeArgs<string,int>()),
            new TestCaseData(nameof(MethodTakingDictionary),
                ArgList(new Dictionary<string, object>()),
                TypeArgs<string,object>()),
            new TestCaseData(nameof(MethodWithNestedTypes),
                ArgList(new List<Dictionary<string, int>>(), new Dictionary<int, List<string[]>>() ),
                TypeArgs<string,int,string[]>()),
            new TestCaseData(nameof(MethodWithOneTypeUsedDirectlyAndAsAnArray),
                ArgList(10, new int[40]),
                TypeArgs<int>()),
            new TestCaseData(nameof(MethodWithOneTypeUsedAsAnArrayAndDirectly),
                ArgList(new int[40], 10),
                TypeArgs<int>())
        };

        [TestCaseSource(nameof(TypeArgData))]
        public void GetTypeArgumentsForMethodTests(string methodName, object[] args, Type[] typeArgs)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(new GenericMethodHelper(method).TryGetTypeArguments(args, out var typeArguments) ? typeArguments : null, Is.EqualTo(typeArgs));
        }

        private static object[] ArgList(params object[] args) { return args; }

        private static Type[] TypeArgs<T>() { return new[] { typeof(T) }; }
        private static Type[] TypeArgs<T1, T2>() { return new[] { typeof(T1), typeof(T2) }; }
        private static Type[] TypeArgs<T1, T2, T3>() { return new[] { typeof(T1), typeof(T2), typeof(T3) }; }

        private void MethodWithOneTypeAndOneParameter<T>(T x) { }

        private void MethodWithOneTypeAndTwoParameters<T>(T x, T y) { }

        private void MethodWithOneTypeAndThreeParameters<T>(T x, T y, T z) { }

        private void MethodWithTwoTypesAndTwoParameters<T, U>(T x, U y) { }

        private void MethodWithTwoTypesAndTwoParameters_Reversed<T, U>(U x, T y) { }

        private void MethodWithTwoTypesAndThreeParameters<T, U>(T x, U y, T z) { }

        private void MethodWithTwoTypesAndFourParameters<T, U>(U q, T x, U y, T z) { }

        private void MethodWithThreeTypes_Order123<T, U, V>(T x, U y, V z) { }

        private void MethodWithThreeTypes_Order132<T, U, V>(T x, V y, U z) { }

        private void MethodWithThreeTypes_Order321<T, U, V>(V x, U y, T z) { }

        private void MethodWithThreeTypes_Order213<T, U, V>(U x, T y, V z) { }

        private void MethodWithGenericListOfType<T>(List<T> c) { }

        private void MethodWithGenericListOfLists<T>(List<List<T>> c) { }

        private void MethodWithGenericEnumerableOfType<T>(IEnumerable<T> c) { }

        private void MethodWithGenericEnumerableOfTypeAsSecondArg<T, U>(T x, IEnumerable<U> c) { }

        private void MethodTakingDictionary<T, U>(Dictionary<T, U> d) { }

        private void MethodWithNestedTypes<T, U, V>(List<Dictionary<T, U>> x, Dictionary<U, List<V>> z) { }

        private void MethodWithOneTypeUsedDirectlyAndAsAnArray<T>(T value, T[] array) { }

        private void MethodWithOneTypeUsedAsAnArrayAndDirectly<T>(T[] array, T value) { }
    }
}
