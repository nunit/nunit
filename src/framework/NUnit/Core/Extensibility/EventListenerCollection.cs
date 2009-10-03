// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************
using System;
using System.Collections;

namespace NUnit.Core.Extensibility
{
	/// <summary>
	/// EventListenerCollection holds multiple event listeners
	/// and relays all event calls to each of them.
	/// </summary>
	public class EventListenerCollection : ExtensionPoint, ITestListener
	{
		#region Constructor
		public EventListenerCollection( IExtensionHost host )
			: base( "EventListeners", host ) { }
		#endregion

		#region EventListener Members
		public void RunStarted(TestName testName, int testCount)
		{
			foreach( ITestListener listener in Extensions )
				listener.RunStarted( testName, testCount );
		}

		public void RunFinished(TestResult result)
		{
			foreach( ITestListener listener in Extensions )
				listener.RunFinished( result );
		}

		public void RunFinished(Exception exception)
		{
			foreach( ITestListener listener in Extensions )
				listener.RunFinished( exception );
		}

		public void SuiteStarted(TestName testName)
		{
			foreach( ITestListener listener in Extensions )
				listener.SuiteStarted( testName );
		}

		public void SuiteFinished(TestResult result)
		{
			foreach( ITestListener listener in Extensions )
				listener.SuiteFinished( result );
		}

		public void TestStarted(TestName testName)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestStarted( testName );
		}

		public void TestFinished(TestResult result)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestFinished( result );
		}

		public void UnhandledException(Exception exception)
		{
			foreach( ITestListener listener in Extensions )
				listener.UnhandledException( exception );
		}

		public void TestOutput(TestOutput testOutput)
		{
			foreach( ITestListener listener in Extensions )
				listener.TestOutput( testOutput );
		}

		#endregion

		#region ExtensionPoint Overrides
		protected override bool IsValidExtension(object extension)
		{
			return extension is ITestListener; 
		}
		#endregion
	}
}
