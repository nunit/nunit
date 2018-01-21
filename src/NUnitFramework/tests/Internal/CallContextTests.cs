// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
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

#if NET20 || NET35 || NET40 || NET45
using System;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using NUnit.TestData.TestFixtureTests;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    // TODO: Make this work for .NET 2.0
    [TestFixture]
    public class CallContextTests
    {
        const string CONTEXT_DATA = "MyContextData";
//		IPrincipal savedPrincipal;

//		[SetUp]
//		public void SaveCurrentPrincipal()
//		{
//			savedPrincipal = System.Threading.Thread.CurrentPrincipal;
//		}
//
//		[TearDown]
//		public void RestoreCurrentPrincipal()
//		{
//			System.Threading.Thread.CurrentPrincipal = savedPrincipal;
//			CallContext.FreeNamedDataSlot(CONTEXT_DATA);
//		}

        [TearDown]
        public void FreeCallContextDataSlot()
        {
            // NOTE: We don't want possible side effects on other cross context tests.
            CallContext.FreeNamedDataSlot(CONTEXT_DATA);
        }

        [Test]
        public void ILogicalThreadAffinativeTest()
        {
            CallContext.SetData( CONTEXT_DATA, new EmptyCallContextData() );
        }

        [Test]
        public void ILogicalThreadAffinativeTestConsole()
        {
            CallContext.SetData( CONTEXT_DATA, new EmptyCallContextData() );
            // TODO: make this Assertable
            // Console.WriteLine("ILogicalThreadAffinativeTest");
            Console.Out.Flush();
        }

        [Test]
        public void GenericPrincipalTest()
        {
            GenericIdentity ident = new GenericIdentity("Bob");
            GenericPrincipal prpal = new GenericPrincipal(ident,
                    new string[] {"Level1"});

            CallContext.SetData( CONTEXT_DATA, new PrincipalCallContextData( prpal ) );
        }

        [Test]
        public void SetGenericPrincipalOnThread()
        {
            GenericIdentity ident = new GenericIdentity("Bob");
            GenericPrincipal prpal = new GenericPrincipal(ident,
                    new string[] {"Level1"});

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
            TestIdentity ident = new TestIdentity( "test" );
            GenericPrincipal principal = new GenericPrincipal( ident, new string[] { "Level1" } );

            System.Threading.Thread.CurrentPrincipal = principal;
        }

        [Test]
        public void ShouldRestoreCurrentPrincipalAfterTestRun()
        {
            IPrincipal principal = Thread.CurrentPrincipal;

            TestBuilder.RunTestFixture( typeof( FixtureThatChangesTheCurrentPrincipal ) );

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
        public PrincipalCallContextData( IPrincipal principal )
        {
        }
    }

    [Serializable]
    public class MyPrincipal : IPrincipal
    {
        public IIdentity Identity
        {
            get
            {
                // TODO:  Add MyPrincipal.Identity getter implementation
                return null;
            }
        }

        public bool IsInRole(string role)
        {
            // TODO:  Add MyPrincipal.IsInRole implementation
            return false;
        }
    }

    [Serializable]
    public class TestIdentity : GenericIdentity
    {
        public TestIdentity( string name ) : base( name )
        {
        }
    }

}
#endif
