// ***********************************************************************
// Copyright (c) 2008-2014 Charlie Poole
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
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// NUnitTestCaseBuilder is a utility class used by attributes
    /// that build test cases.
    /// </summary>
    public class NUnitTestCaseBuilder
    {
        private readonly Randomizer randomizer = Randomizer.CreateRandomizer();

        /// <summary>
        /// Builds a single NUnitTestMethod, either as a child of the fixture 
        /// or as one of a set of test cases under a ParameterizedTestMethodSuite.
        /// </summary>
        /// <param name="method">The MethodInfo from which to construct the TestMethod</param>
        /// <param name="parentSuite">The suite or fixture to which the new test will be added</param>
        /// <param name="parms">The ParameterSet to be used, or null</param>
        /// <returns></returns>
        public TestMethod BuildTestMethod(MethodInfo method, Test parentSuite, ParameterSet parms)
        {
            var testMethod = new TestMethod(method, parentSuite)
            {
                Seed = randomizer.Next()
            };

            string prefix = method.ReflectedType.FullName;

            // Needed to give proper fullname to test in a parameterized fixture.
            // Without this, the arguments to the fixture are not included.
            if (parentSuite != null)
                prefix = parentSuite.FullName;

            if (CheckTestMethodSignature(testMethod, parms))
            {
                if (parms == null || parms.Arguments == null)
                    testMethod.ApplyAttributesToTest(method);
            }

            if (parms != null)
            {
                // NOTE: After the call to CheckTestMethodSignature, the Method
                // property of testMethod may no longer be the same as the
                // original MethodInfo, so we reassign it here.
                method = testMethod.Method;

                if (parms.TestName != null)
                {
                    testMethod.Name = parms.TestName;
                    testMethod.FullName = prefix + "." + parms.TestName;
                }
                else if (parms.OriginalArguments != null)
                {
                    string name = MethodHelper.GetDisplayName(method, parms.OriginalArguments);
                    testMethod.Name = name;
                    testMethod.FullName = prefix + "." + name;
                }

                parms.ApplyToTest(testMethod);
            }

            return testMethod;
        }

        #region Helper Methods

        /// <summary>
        /// Helper method that checks the signature of a TestMethod and
        /// any supplied parameters to determine if the test is valid.
        /// 
        /// Currently, NUnitTestMethods are required to be public, 
        /// non-abstract methods, either static or instance,
        /// returning void. They may take arguments but the _values must
        /// be provided or the TestMethod is not considered runnable.
        /// 
        /// Methods not meeting these criteria will be marked as
        /// non-runnable and the method will return false in that case.
        /// </summary>
        /// <param name="testMethod">The TestMethod to be checked. If it
        /// is found to be non-runnable, it will be modified.</param>
        /// <param name="parms">Parameters to be used for this test, or null</param>
        /// <returns>True if the method signature is valid, false if not</returns>
        private static bool CheckTestMethodSignature(TestMethod testMethod, ParameterSet parms)
        {
            if (testMethod.Method.IsAbstract)
            {
                return MarkAsNotRunnable(testMethod, "Method is abstract");
            }

            if (!testMethod.Method.IsPublic)
            {
                return MarkAsNotRunnable(testMethod, "Method is not public");
            }

            ParameterInfo[] parameters;
#if NETCF
            if (testMethod.Method.IsGenericMethodDefinition)
            {
                if (parms != null && parms.Arguments != null)
                {
                    testMethod.Method = testMethod.Method.MakeGenericMethodEx(parms.Arguments);
                    if (testMethod.Method == null)
                        return MarkAsNotRunnable(testMethod, "Cannot determine generic types by probing");
                    parameters = testMethod.Method.GetParameters();
                }
                else
                    parameters = new ParameterInfo[0];
            }
            else
#endif
            parameters = testMethod.Method.GetParameters();

            int argsNeeded = parameters.Length;

            object[] arglist = null;
            int argsProvided = 0;

            if (parms != null)
            {
                testMethod.parms = parms;
                testMethod.RunState = parms.RunState;

                arglist = parms.Arguments;

                if (arglist != null)
                    argsProvided = arglist.Length;

                if (testMethod.RunState != RunState.Runnable)
                    return false;
            }

            Type returnType = testMethod.Method.ReturnType;

#if NET_4_0 || NET_4_5
            if (AsyncInvocationRegion.IsAsyncOperation(testMethod.Method))
            {
                if (returnType == typeof (void))
                    return MarkAsNotRunnable(testMethod, "Async test method must have non-void return type");

                var returnsGenericTask = returnType.IsGenericType &&
                                         returnType.GetGenericTypeDefinition() == typeof (System.Threading.Tasks.Task<>);

                if (returnsGenericTask && (parms == null || !parms.HasExpectedResult))
                    return MarkAsNotRunnable(testMethod,
                        "Async test method must have non-generic Task return type when no result is expected");

                if (!returnsGenericTask && parms != null && parms.HasExpectedResult)
                    return MarkAsNotRunnable(testMethod,
                        "Async test method must have Task<T> return type when a result is expected");
            }
            else
#endif
            if (returnType == typeof (void))
            {
                if (parms != null && parms.HasExpectedResult)
                    return MarkAsNotRunnable(testMethod, "Method returning void cannot have an expected result");
            }
            else if (parms == null || !parms.HasExpectedResult)
                return MarkAsNotRunnable(testMethod, "Method has non-void return value, but no result is expected");

            if (argsProvided > 0 && argsNeeded == 0)
            {
                return MarkAsNotRunnable(testMethod, "Arguments provided for method not taking any");
            }

            if (argsProvided == 0 && argsNeeded > 0)
            {
                return MarkAsNotRunnable(testMethod, "No arguments were provided");
            }

            if (argsProvided != argsNeeded)
            {
                return MarkAsNotRunnable(testMethod, "Wrong number of arguments provided");
            }

            if (testMethod.Method.IsGenericMethodDefinition)
            {
                Type[] typeArguments = GetTypeArgumentsForMethod(testMethod.Method, arglist);
                foreach (object o in typeArguments)
                    if (o == null)
                    {
                        return MarkAsNotRunnable(testMethod, "Unable to determine type arguments for method");
                    }

                testMethod.Method = testMethod.Method.MakeGenericMethod(typeArguments);
                parameters = testMethod.Method.GetParameters();
            }

            if (arglist != null && parameters != null)
                TypeHelper.ConvertArgumentList(arglist, parameters);

            return true;
        }

        private static Type[] GetTypeArgumentsForMethod(MethodInfo method, object[] arglist)
        {
            Type[] typeParameters = method.GetGenericArguments();
            Type[] typeArguments = new Type[typeParameters.Length];
            ParameterInfo[] parameters = method.GetParameters();

            for (int typeIndex = 0; typeIndex < typeArguments.Length; typeIndex++)
            {
                Type typeParameter = typeParameters[typeIndex];

                for (int argIndex = 0; argIndex < parameters.Length; argIndex++)
                {
                    if (parameters[argIndex].ParameterType.Equals(typeParameter))
                    {
                        // If a null arg is provided, pass null as the Type
                        // BestCommonType knows how to deal with this
                        Type argType = arglist[argIndex] != null
                            ? arglist[argIndex].GetType()
                            : null;
                        typeArguments[typeIndex] = TypeHelper.BestCommonType(
                            typeArguments[typeIndex],
                            argType);
                    }
                }
            }

            return typeArguments;
        }

        private static bool MarkAsNotRunnable(TestMethod testMethod, string reason)
        {
            testMethod.RunState = RunState.NotRunnable;
            testMethod.Properties.Set(PropertyNames.SkipReason, reason);
            return false;
        }

        #endregion
    }
}
