using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework.Internal;

#if NET20 || NET35 || NET40 || NET45
using System.Runtime.Remoting.Messaging;
#endif

namespace NUnit.Framework.Assertions
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
            Exception testException = null;

            try
            {
                // Currently, we know all the tests are static, without arguments
                method.Invoke(null, null);
            }
            catch(Exception ex)
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
            if (testException != null)
                throw testException;
        }

#if !(NET20 || NET35 || NET40 || NET45)
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
        private static readonly string CONTEXT_KEY = "NUnit.Framework.TestContext";

        private TestExecutionContext ClearExecutionContext()
        {
            var savedContext = TestExecutionContext.CurrentContext;
            CallContext.FreeNamedDataSlot(CONTEXT_KEY);
            return savedContext;
        }

        private void RestoreExecutionContext(TestExecutionContext savedContext)
        {
            CallContext.SetData(CONTEXT_KEY, savedContext);
        }
#endif

        static class AdhocTests
        {
            private static readonly MethodInfo[] _methods =
                typeof(AdhocTests).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            public static IEnumerable<MethodInfo> TestMethods
            {
                get
                {
                    foreach (var method in _methods)
                        if (method.Name.StartsWith("Test"))
                            yield return method;
                }
            }

            static public void TestValidContext()
            {
                Assert.NotNull(TestExecutionContext.CurrentContext);
                Assert.That(TestExecutionContext.CurrentContext, Is.TypeOf<TestExecutionContext.AdhocContext>());
            }

            static public void TestPassingAssert()
            {
                Assert.That(true, Is.True);
            }

            static public void TestPassingAssumption()
            {
                Assume.That(true, Is.True);
            }

            static public void TestPassingWarning()
            {
                Warn.Unless(true, Is.True);
            }

            static public void TestFailingAssertion()
            {
                Assert.That(() => Assert.That(true, Is.False), Throws.TypeOf<AssertionException>());
            }

            static public void TestFailingAssumption()
            {
                Assert.That(() => Assume.That(true, Is.False), Throws.TypeOf<InconclusiveException>());
            }

            static public void TestFailingWarning()
            {
                // Warnings don't throw at all. They are of no use in adhoc execution.
                Assert.That(() => Warn.Unless(true, Is.False), Throws.Nothing);
            }

            static public void TestAssertPass()
            {
                Assert.That(() => Assert.Pass(), Throws.TypeOf<SuccessException>());
            }

            static public void TestAssertInconclusive()
            {
                Assert.That(() => Assert.Inconclusive(), Throws.TypeOf<InconclusiveException>());
            }

            static public void TestAssertIgnore()
            {
                Assert.That(() => Assert.Ignore(), Throws.TypeOf<IgnoreException>());
            }

            static public void TestAssertFail()
            {
                Assert.That(() => Assert.Fail(), Throws.TypeOf<AssertionException>());
            }

            static public void TestAssertMultiple_AllAssertsPassing()
            {
                Assert.Multiple(() =>
                {
                    Assert.That(2 + 2, Is.EqualTo(4));
                    Assert.That(2 * 2, Is.EqualTo(4));
                    Assert.That(2 - 2, Is.EqualTo(0));
                    Assert.That(2 / 2, Is.EqualTo(1));
                });
            }

            static public void TestAssertMultiple_OneAssertFailing()
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
