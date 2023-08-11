// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Assertions
{
    public class AdhocTestExecutionTests
    {
        // These unit tests demonstrate that the user can call a test directly
        // and have it execute. Only information from exceptions thrown is
        // available in this case, but it appears that a number of users do this.
        // When a user calls a test method directly, without use of NUnit,
        // the TestExecutionContext is null. We provide an AdhocContext in
        // that case, so that the tests can run. The technique is not useful
        // with warnings or multiple asserts, but will at least run without
        // crashing.

        [TestCaseSource(typeof(AdhocTests), nameof(AdhocTests.TestMethods))]
        public void CanCallAssertWithoutTestExecutionContext(MethodInfo method)
        {
            var savedContext = ClearExecutionContext();
            Exception? testException = null;

            try
            {
                // Currently, we know all the tests are static, without arguments
                method.Invoke(null, null);
            }
            catch (Exception ex)
            {
                testException = ex is TargetInvocationException
                    ? ex.InnerException
                    : ex;
            }
            finally
            {
                RestoreExecutionContext(savedContext);
            }

            // Throw any exception we got only after context is restored
            if (testException is not null)
                throw testException;
        }

#if !NETFRAMEWORK
        private TestExecutionContext ClearExecutionContext()
        {
            var savedContext = TestExecutionContext.CurrentContext;
            TestExecutionContext.CurrentContext = null;
            return savedContext;
        }

        private void RestoreExecutionContext(TestExecutionContext savedContext)
        {
            TestExecutionContext.CurrentContext = savedContext;
        }
#else
        private TestExecutionContext ClearExecutionContext()
        {
            var savedContext = TestExecutionContext.CurrentContext;
            System.Runtime.Remoting.Messaging.CallContext.FreeNamedDataSlot(NUnitCallContext.TestExecutionContextKey);
            return savedContext;
        }

        private void RestoreExecutionContext(TestExecutionContext savedContext)
        {
            System.Runtime.Remoting.Messaging.CallContext.SetData(NUnitCallContext.TestExecutionContextKey, savedContext);
        }
#endif

        private static class AdhocTests
        {
            private static readonly MethodInfo[] Methods =
                typeof(AdhocTests).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            public static IEnumerable<MethodInfo> TestMethods
            {
                get
                {
                    foreach (var method in Methods)
                    {
                        if (method.Name.StartsWith("Test"))
                            yield return method;
                    }
                }
            }

            public static void TestValidContext()
            {
                Assert.That(TestExecutionContext.CurrentContext, Is.Not.Null);
                Assert.That(TestExecutionContext.CurrentContext, Is.TypeOf<TestExecutionContext.AdhocContext>());
            }

            public static void TestPassingAssert()
            {
                Assert.That(true, Is.True);
            }

            public static void TestPassingAssumption()
            {
                Assume.That(true, Is.True);
            }

            public static void TestPassingWarning()
            {
                Warn.Unless(true, Is.True);
            }

            public static void TestFailingAssertion()
            {
                Assert.That(() => Assert.That(true, Is.False), Throws.TypeOf<AssertionException>());
            }

            public static void TestFailingAssumption()
            {
                Assert.That(() => Assume.That(true, Is.False), Throws.TypeOf<InconclusiveException>());
            }

            public static void TestFailingWarning()
            {
                // Warnings don't throw at all. They are of no use in ad-hoc execution.
                Assert.That(() => Warn.Unless(true, Is.False), Throws.Nothing);
            }

            public static void TestAssertPass()
            {
                Assert.That(() => Assert.Pass(), Throws.TypeOf<SuccessException>());
            }

            public static void TestAssertInconclusive()
            {
                Assert.That(() => Assert.Inconclusive(), Throws.TypeOf<InconclusiveException>());
            }

            public static void TestAssertIgnore()
            {
                Assert.That(() => Assert.Ignore(), Throws.TypeOf<IgnoreException>());
            }

            public static void TestAssertFail()
            {
                Assert.That(() => Assert.Fail(), Throws.TypeOf<AssertionException>());
            }

            public static void TestAssertMultiple_AllAssertsPassing()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4));
                    Assert.That(2 * 2, Is.EqualTo(4));
                    Assert.That(2 - 2, Is.EqualTo(0));
                    Assert.That(2 / 2, Is.EqualTo(1));
                });
            }

            public static void TestAssertMultiple_OneAssertFailing()
            {
                Assert.That(() =>
                    Assert.Multiple(() =>
                    {
                        Assert.That(2 + 2, Is.EqualTo(5));
                        Assert.That(2 * 2, Is.EqualTo(4));
                        Assert.That(2 - 2, Is.EqualTo(0));
                        Assert.That(2 / 2, Is.EqualTo(1));
                    }),
                    Throws.TypeOf<MultipleAssertException>());
            }
        }
    }
}
