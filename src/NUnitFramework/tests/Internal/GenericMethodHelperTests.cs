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

#if !NETCF // Doesn't work under CF - But also isn't needed because we construct a closed method
using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Internal
{
    public class GenericMethodHelperTests
    {
        static TestCaseData[] TypeArgData = new TestCaseData[] {
            new TestCaseData("MethodWithOneTypeAndOneParameter", 
                ArgList(42), 
                TypeArgs<int>()),
            new TestCaseData("MethodWithOneTypeAndTwoParameters", 
                ArgList(42, 99), 
                TypeArgs<int>()),
            new TestCaseData("MethodWithOneTypeAndTwoParameters", 
                ArgList(42.0, 99.0), 
                TypeArgs<double>()),
            new TestCaseData("MethodWithOneTypeAndTwoParameters", 
                ArgList(42, 99.0), 
                TypeArgs<double>()),
            new TestCaseData("MethodWithOneTypeAndTwoParameters", 
                ArgList(42.0, 99), 
                TypeArgs<double>()),
            new TestCaseData("MethodWithOneTypeAndThreeParameters", 
                ArgList(42, -1, 7), 
                TypeArgs<int>()),
            new TestCaseData("MethodWithTwoTypesAndTwoParameters", 
                ArgList(42, "Answer"), 
                TypeArgs<int,string>()),
            new TestCaseData("MethodWithTwoTypesAndTwoParameters_Reversed", 
                ArgList("Answer", 42), 
                TypeArgs<int,string>()),
            new TestCaseData("MethodWithTwoTypesAndThreeParameters", 
                ArgList(42, "Answer", 42), 
                TypeArgs<int,string>()),
            new TestCaseData("MethodWithTwoTypesAndFourParameters", 
                ArgList("Question", 1, "Answer", 42), 
                TypeArgs<int,string>()),
            new TestCaseData("MethodWithThreeTypes_Order123", 
                ArgList(42, 42.0, "forty-two"), 
                TypeArgs<int,double,string>()),
            new TestCaseData("MethodWithThreeTypes_Order132", 
                ArgList(42, "forty-two", 42.0), 
                TypeArgs<int,double,string>()),
            new TestCaseData("MethodWithThreeTypes_Order321", 
                ArgList("forty-two", 42.0, 42), 
                TypeArgs<int,double,string>()),
            new TestCaseData("MethodWithThreeTypes_Order213", 
                ArgList(42.0, 42, "forty-two"), 
                TypeArgs<int,double,string>()),
            new TestCaseData("MethodWithOneTypeAndOneParameter", 
                ArgList(new int[] { 1, 2, 3 }), 
                TypeArgs<int[]>()),
            new TestCaseData("MethodWithGenericListOfType", 
                ArgList(new List<int>()),
                TypeArgs<int>()),
            new TestCaseData("MethodWithGenericListOfType", 
                ArgList(new LinkedList<int>()),
                new Type[] { null } ),
            new TestCaseData("MethodWithGenericListOfLists", 
                ArgList(new List<List<int>>()), 
                TypeArgs<int>()),
            new TestCaseData("MethodWithGenericEnumerableOfType", 
                ArgList(new List<int>(new int[] { 1, 2, 3 })), 
                TypeArgs<int>()),
            new TestCaseData("MethodWithGenericEnumerableOfType", 
                ArgList(new int[] { 1, 2, 3 }), 
                TypeArgs<int>()),
            new TestCaseData("MethodWithGenericEnumerableOfTypeAsSecondArg", 
                ArgList("X", new int[] { } ), 
                TypeArgs<string,int>()),
            new TestCaseData("MethodTakingDictionary",                       
                ArgList(new Dictionary<string, object>()), 
                TypeArgs<string,object>()),
            new TestCaseData("MethodWithNestedTypes",                        
                ArgList(new List<Dictionary<string, int>>(), new Dictionary<int, List<string[]>>() ), 
                TypeArgs<string,int,string[]>()) 
        };

        [TestCaseSource("TypeArgData")]
        public void GetTypeArgumentsForMethodTests(string methodName, object[] args, Type[] typeArgs)
        {
            MethodInfo method = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(new GenericMethodHelper(method).GetTypeArguments(args), Is.EqualTo(typeArgs));
        }

        private static object[] ArgList(params object[] args) { return args; }

        private static Type[] TypeArgs<T>() { return new Type[] { typeof(T) }; }
        private static Type[] TypeArgs<T1, T2>() { return new Type[] { typeof(T1), typeof(T2) }; }
        private static Type[] TypeArgs<T1, T2, T3>() { return new Type[] { typeof(T1), typeof(T2), typeof(T3) }; }

        void MethodWithOneTypeAndOneParameter<T>(T x) { }
        void MethodWithOneTypeAndTwoParameters<T>(T x, T y) { }
        void MethodWithOneTypeAndThreeParameters<T>(T x, T y, T z) { }

        void MethodWithTwoTypesAndTwoParameters<T, U>(T x, U y) { }
        void MethodWithTwoTypesAndTwoParameters_Reversed<T, U>(U x, T y) { }
        void MethodWithTwoTypesAndThreeParameters<T, U>(T x, U y, T z) { }
        void MethodWithTwoTypesAndFourParameters<T, U>(U q, T x, U y, T z) { }

        void MethodWithThreeTypes_Order123<T, U, V>(T x, U y, V z) { }
        void MethodWithThreeTypes_Order132<T, U, V>(T x, V y, U z) { }
        void MethodWithThreeTypes_Order321<T, U, V>(V x, U y, T z) { }
        void MethodWithThreeTypes_Order213<T, U, V>(U x, T y, V z) { }

        void MethodWithGenericListOfType<T>(List<T> c) { }
        void MethodWithGenericListOfLists<T>(List<List<T>> c) { }
        void MethodWithGenericEnumerableOfType<T>(IEnumerable<T> c) { }
        void MethodWithGenericEnumerableOfTypeAsSecondArg<T, U>(T x, IEnumerable<U> c) { }

        void MethodTakingDictionary<T, U>(Dictionary<T, U> d) { }
        void MethodWithNestedTypes<T, U, V>(List<Dictionary<T, U>> x, Dictionary<U, List<V>> z) { }
    }
}
#endif

