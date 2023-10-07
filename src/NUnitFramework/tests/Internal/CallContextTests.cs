// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

#if NETFRAMEWORK
using System;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Threading;
using NUnit.TestData.TestFixtureTests;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class CallContextTests
    {
        private const string CONTEXT_DATA = "MyContextData";
        ////IPrincipal savedPrincipal;

        ////[SetUp]
        ////public void SaveCurrentPrincipal()
        ////{
        ////    savedPrincipal = System.Threading.Thread.CurrentPrincipal;
        ////}

        ////[TearDown]
        ////public void RestoreCurrentPrincipal()
        ////{
        ////    System.Threading.Thread.CurrentPrincipal = savedPrincipal;
        ////    CallContext.FreeNamedDataSlot(CONTEXT_DATA);
        ////}

        [TearDown]
        public void FreeCallContextDataSlot()
        {
            // NOTE: We don't want possible side effects on other cross context tests.
            CallContext.FreeNamedDataSlot(CONTEXT_DATA);
        }

        [Test]
        public void ILogicalThreadAffinativeTest()
        {
            CallContext.SetData(CONTEXT_DATA, new EmptyCallContextData());
        }

        [Test]
        public void ILogicalThreadAffinativeTestConsole()
        {
            CallContext.SetData(CONTEXT_DATA, new EmptyCallContextData());
            // TODO: make this Assertable
            // Console.WriteLine("ILogicalThreadAffinativeTest");
            Console.Out.Flush();
        }

        [Test]
        public void GenericPrincipalTest()
        {
            GenericIdentity ident = new GenericIdentity("Bob");
            GenericPrincipal prpal = new GenericPrincipal(ident,
                    new[] { "Level1" });

            CallContext.SetData(CONTEXT_DATA, new PrincipalCallContextData(prpal));
        }

        [Test]
        public void SetGenericPrincipalOnThread()
        {
            GenericIdentity ident = new GenericIdentity("Bob");
            GenericPrincipal prpal = new GenericPrincipal(ident,
                    new[] { "Level1" });

            System.Threading.Thread.CurrentPrincipal = prpal;
        }

        [Test]
        public void SetCustomPrincipalOnThread()
        {
            MyPrincipal prpal = new MyPrincipal();

            System.Threading.Thread.CurrentPrincipal = prpal;
        }

        [Test]
        public void UseCustomIdentity()
        {
            TestIdentity ident = new TestIdentity("test");
            GenericPrincipal principal = new GenericPrincipal(ident, new[] { "Level1" });

            System.Threading.Thread.CurrentPrincipal = principal;
        }

        [Test]
        public void ShouldRestoreCurrentPrincipalAfterTestRun()
        {
            IPrincipal? principal = Thread.CurrentPrincipal;

            TestBuilder.RunTestFixture(typeof(FixtureThatChangesTheCurrentPrincipal));

            Assert.That(
                Thread.CurrentPrincipal,
                Is.SameAs(principal),
                "The Thread Principal was not restored after the test was run");
        }
    }

    /// <summary>
    /// Helper class that implements ILogicalThreadAffinative
    /// but holds no data at all
    /// </summary>
    [Serializable]
    public class EmptyCallContextData : ILogicalThreadAffinative
    {
    }

    [Serializable]
    public class PrincipalCallContextData : ILogicalThreadAffinative
    {
        public PrincipalCallContextData(IPrincipal principal)
        {
        }
    }

    [Serializable]
    public class MyPrincipal : IPrincipal
    {
        public IIdentity? Identity =>
            // TODO:  Add MyPrincipal.Identity getter implementation
            null;

        public bool IsInRole(string role)
        {
            // TODO:  Add MyPrincipal.IsInRole implementation
            return false;
        }
    }

    [Serializable]
    public class TestIdentity : GenericIdentity
    {
        public TestIdentity(string name) : base(name)
        {
        }
    }
}
#endif
