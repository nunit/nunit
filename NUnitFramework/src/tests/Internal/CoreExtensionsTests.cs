// ***********************************************************************
// Copyright (c) 2007 Charlie Poole
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
using System.Text;
using System.Reflection;
using NUnit.Framework.Extensibility;

namespace NUnit.Framework.Internal
{
	[TestFixture]
	public class CoreExtensionsTests
	{
		private CoreExtensions host;

		[SetUp]
		public void CreateHost()
		{
			host = new CoreExtensions();
		}

		[Test]
		public void HasSuiteBuildersExtensionPoint()
		{
			IExtensionPoint ep = host.GetExtensionPoint( "SuiteBuilders" );
			Assert.IsNotNull( ep );
			Assert.AreEqual( "SuiteBuilders", ep.Name );
			Assert.AreEqual( typeof( SuiteBuilderCollection ), ep.GetType() );
		}

		[Test]
		public void HasTestCaseBuildersExtensionPoint()
		{
			IExtensionPoint ep = host.GetExtensionPoint( "TestCaseBuilders" );
			Assert.IsNotNull( ep );
			Assert.AreEqual( "TestCaseBuilders", ep.Name );
			Assert.AreEqual( typeof( TestCaseBuilderCollection ), ep.GetType() );
		}

		[Test]
		public void HasEventListenerExtensionPoint()
		{
			IExtensionPoint ep = host.GetExtensionPoint( "EventListeners" );
			Assert.IsNotNull( ep );
			Assert.AreEqual( "EventListeners", ep.Name );
			Assert.AreEqual( typeof( EventListenerCollection ), ep.GetType() );
		}

        //public void CanAddSuiteBuilder()
        //{
        //    DynamicMock mock = new DynamicMock( typeof(ISuiteBuilder) );
        //    mock.ExpectAndReturn( "CanBuildFrom", true, null );
        //    mock.Expect( "BuildFrom" );
			
        //    IExtensionPoint ep = host.GetExtensionPoint("SuiteBuilders");
        //    ep.Install( mock.MockInstance );
        //    ISuiteBuilder builders = (ISuiteBuilder)ep;
        //    builders.BuildFrom( null );

        //    mock.Verify();
        //}

        //[Test]
        //public void CanAddTestCaseBuilder()
        //{
        //    DynamicMock mock = new DynamicMock(typeof(ITestCaseBuilder));
        //    mock.ExpectAndReturn("CanBuildFrom", true, null);
        //    mock.Expect("BuildFrom");

        //    IExtensionPoint ep = host.GetExtensionPoint("TestCaseBuilders");
        //    ep.Install(mock.MockInstance);
        //    ITestCaseBuilder builders = (ITestCaseBuilder)ep;
        //    builders.BuildFrom(null);

        //    mock.Verify();
        //}

        //[Test]
        //public void CanAddTestCaseBuilder2()
        //{
        //    DynamicMock mock = new DynamicMock(typeof(ITestCaseBuilder2));
        //    mock.ExpectAndReturn("CanBuildFrom", true, null);
        //    mock.Expect("BuildFrom");

        //    IExtensionPoint ep = host.GetExtensionPoint("TestCaseBuilders");
        //    ep.Install(mock.MockInstance);
        //    ITestCaseBuilder2 builders = (ITestCaseBuilder2)ep;
        //    builders.BuildFrom(null, null);

        //    mock.Verify();
        //}

        //[Test]
        //public void CanAddEventListener()
        //{
        //    DynamicMock mock = new DynamicMock( typeof(EventListener) );
        //    mock.Expect( "RunStarted" );
        //    mock.Expect( "RunFinished" );

        //    IExtensionPoint ep = host.GetExtensionPoint("EventListeners");
        //    ep.Install( mock.MockInstance );
        //    EventListener listeners = (EventListener)ep;
        //    listeners.RunStarted( "test", 0 );
        //    listeners.RunFinished( new TestResult( new TestInfo( new TestSuite( "test" ) ) ) );

        //    mock.Verify();
        //}
	}
}
