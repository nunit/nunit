// ***********************************************************************
// Copyright (c) 2008-2014 Charlie Poole, Rob Prouse
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

#nullable enable

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal.Builders
{
    /// <summary>
    /// NUnitTestCaseBuilder is a utility class used by attributes
    /// that build test cases.
    /// </summary>
    public class NUnitTestCaseBuilder
    {
        private readonly Randomizer _randomizer = Randomizer.CreateRandomizer();
        private readonly TestNameGenerator _nameGenerator;

        /// <summary>
        /// Constructs an <see cref="NUnitTestCaseBuilder"/>
        /// </summary>
        public NUnitTestCaseBuilder()
        {
            _nameGenerator = new TestNameGenerator();
        }

        /// <summary>
        /// Builds a single NUnitTestMethod, either as a child of the fixture
        /// or as one of a set of test cases under a ParameterizedTestMethodSuite.
        /// </summary>
        /// <param name="method">The MethodInfo from which to construct the TestMethod</param>
        /// <param name="parentSuite">The suite or fixture to which the new test will be added</param>
        /// <param name="parms">The ParameterSet to be used, or null</param>
        public TestMethod BuildTestMethod(IMethodInfo method, Test? parentSuite, TestCaseParameters? parms)
        {
            var testMethod = new TestMethod(method, parentSuite)
            {
                Seed = _randomizer.Next()
            };

            var metadata = MethodInfoCache.Get(method);

            CheckTestMethodAttributes(testMethod, metadata);

            CheckTestMethodSignature(testMethod, metadata, parms);

            if (parms == null || parms.Arguments.Length == 0)
                testMethod.ApplyAttributesToTest(method.MethodInfo);

            // NOTE: After the call to CheckTestMethodSignature, the Method
            // property of testMethod may no longer be the same as the
            // original MethodInfo, so we don't use it here.
            string prefix = testMethod.Method.TypeInfo.FullName;

            // Needed to give proper full name to test in a parameterized fixture.
            // Without this, the arguments to the fixture are not included.
            if (parentSuite != null)
                prefix = parentSuite.FullName;

            if (parms != null)
            {
                parms.ApplyToTest(testMethod);

                if (parms.TestName != null)
                {
                    // The test is simply for efficiency
                    testMethod.Name = parms.TestName.Contains("{")
                        ? new TestNameGenerator(parms.TestName).GetDisplayName(testMethod, parms.OriginalArguments)
                        : parms.TestName;
                }
                else if (parms.ArgDisplayNames != null)
                {
                    testMethod.Name = testMethod.Name + '(' + string.Join(", ", parms.ArgDisplayNames) + ')';
                }
                else
                {
                    testMethod.Name = _nameGenerator.GetDisplayName(testMethod, parms.OriginalArguments);
                }
            }
            else
            {
                testMethod.Name = _nameGenerator.GetDisplayName(testMethod, null);
            }

            testMethod.FullName = prefix + "." + testMethod.Name;

            return testMethod;
        }

        #region Helper Methods

        /// <summary>
        /// Checks to see if we have valid combinations of attributes.
        /// </summary>
        /// <param name="testMethod">The TestMethod to be checked. If it
        /// is found to be non-runnable, it will be modified.</param>
        /// <param name="metadata">Metadata for this TestMethod.</param>
        /// <returns>True if the method signature is valid, false if not</returns>
        private static bool CheckTestMethodAttributes(TestMethod testMethod, MethodInfoCache.TestMethodMetadata metadata)
        {
            if (metadata.RepeatTestAttributes.Length > 1)
                return MarkAsNotRunnable(testMethod, "Multiple attributes that repeat a test may cause issues.");

            return true;
        }

        /// <summary>
        /// Helper method that checks the signature of a TestMethod and
        /// any supplied parameters to determine if the test is valid.
        ///
        /// Currently, NUnitTestMethods are required to be public,
        /// non-abstract methods, either static or instance,
        /// returning void. They may take arguments but the values must
        /// be provided or the TestMethod is not considered runnable.
        ///
        /// Methods not meeting these criteria will be marked as
        /// non-runnable and the method will return false in that case.
        /// </summary>
        /// <param name="testMethod">The TestMethod to be checked. If it
        /// is found to be non-runnable, it will be modified.</param>
        /// <param name="metadata">Metadata for this TestMethod.</param>
        /// <param name="parms">Parameters to be used for this test, or null</param>
        /// <returns>True if the method signature is valid, false if not</returns>
        /// <remarks>
        /// The return value is no longer used internally, but is retained
        /// for testing purposes.
        /// </remarks>
        private static bool CheckTestMethodSignature(TestMethod testMethod, MethodInfoCache.TestMethodMetadata metadata, TestCaseParameters? parms)
        {
            if (testMethod.Method.IsAbstract)
                return MarkAsNotRunnable(testMethod, "Method is abstract");

            if (!testMethod.Method.IsPublic)
                return MarkAsNotRunnable(testMethod, "Method is not public");

            IParameterInfo[] parameters = metadata.Parameters;
            int minArgsNeeded = 0;
            foreach (var parameter in parameters)
            {
                // IsOptional is supported since .NET 1.1
                if (!parameter.IsOptional)
                    minArgsNeeded++;
            }

            int maxArgsNeeded = parameters.Length;

            object?[]? arglist = null;
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

            var returnType = testMethod.Method.ReturnType.Type;

            if (metadata.IsAsyncOperation)
            {
                if (returnType == typeof(void))
                    return MarkAsNotRunnable(testMethod, "Async test method must have non-void return type");

                var voidResult = Reflect.IsVoidOrUnit(AwaitAdapter.GetResultType(returnType));

                if (!voidResult && (parms == null || !parms.HasExpectedResult))
                    return MarkAsNotRunnable(testMethod,
                        "Async test method must return an awaitable with a void result when no result is expected");

                if (voidResult && parms != null && parms.HasExpectedResult)
                    return MarkAsNotRunnable(testMethod,
                        "Async test method must return an awaitable with a non-void result when a result is expected");
            }
            else if (metadata.IsVoidOrUnit)
            {
                if (parms != null && parms.HasExpectedResult)
                    return MarkAsNotRunnable(testMethod, "Method returning void cannot have an expected result");
            }
            else if (parms == null || !parms.HasExpectedResult)
                return MarkAsNotRunnable(testMethod, "Method has non-void return value, but no result is expected");

            if (argsProvided > 0 && maxArgsNeeded == 0)
                return MarkAsNotRunnable(testMethod, "Arguments provided for method with no parameters");

            if (argsProvided == 0 && minArgsNeeded > 0)
                return MarkAsNotRunnable(testMethod, "No arguments were provided");

            if (argsProvided < minArgsNeeded)
                return MarkAsNotRunnable(testMethod, string.Format("Not enough arguments provided, provide at least {0} arguments.", minArgsNeeded));

            if (argsProvided > maxArgsNeeded)
                return MarkAsNotRunnable(testMethod, string.Format("Too many arguments provided, provide at most {0} arguments.", maxArgsNeeded));

            if (testMethod.Method.IsGenericMethodDefinition && arglist != null)
            {
                if (!new GenericMethodHelper(testMethod.Method.MethodInfo).TryGetTypeArguments(arglist, out var typeArguments))
                    return MarkAsNotRunnable(testMethod, "Unable to determine type arguments for method");

                testMethod.Method = testMethod.Method.MakeGenericMethod(typeArguments);
                parameters = testMethod.Method.GetParameters();
            }

            if (parms != null && parms.TestName != null && parms.TestName.Trim() == "")
                return MarkAsNotRunnable(testMethod, "Test name cannot be all white-space or empty.");

            if (arglist != null && parameters != null)
                TypeHelper.ConvertArgumentList(arglist, parameters);

            return true;
        }

        private static bool MarkAsNotRunnable(TestMethod testMethod, string reason)
        {
            testMethod.MakeInvalid(reason);
            return false;
        }

        #endregion
    }
}
