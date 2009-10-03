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

namespace NUnit.Core
{
	using NUnit.Core.Builders;
	using System.Collections;
	using System.Reflection;

	/// <summary>
	/// This is the master suite builder for NUnit. It builds a test suite from
	/// one or more assemblies using a list of internal and external suite builders 
	/// to create fixtures from the qualified types in each assembly. It implements
	/// the ISuiteBuilder interface itself, allowing it to be used by other classes
	/// for queries and suite construction.
	/// </summary>D:\Dev\NUnit\nunit20\src\NUnitFramework\core\TestBuilderAttribute.cs
	public class TestSuiteBuilder
	{
		#region Instance Variables

		private ArrayList builders = new ArrayList();

		#endregion

		#region Properties
		public IList Assemblies
		{
			get 
			{
				ArrayList assemblies = new ArrayList();
				foreach( TestAssemblyBuilder builder in builders )
					assemblies.Add( builder.Assembly );
				return assemblies; 
			}
		}

		public IList AssemblyInfo
		{
			get
			{
				ArrayList info = new ArrayList();
				foreach( TestAssemblyBuilder builder in this.builders )
					info.Add( builder.AssemblyInfo );

				return info;
			}
		}
		#endregion

		#region Build Methods
		/// <summary>
		/// Build a suite based on a TestPackage
		/// </summary>
		/// <param name="package">The TestPackage</param>
		/// <returns>A TestSuite</returns>
		public TestSuite Build( TestPackage package )
		{
			bool autoNamespaceSuites = package.GetSetting( "AutoNamespaceSuites", true );
			bool mergeAssemblies = package.GetSetting( "MergeAssemblies", false );
            TestContext.TestCaseTimeout = package.GetSetting("DefaultTimeout", 0);

			if ( package.IsSingleAssembly )
				return BuildSingleAssembly( package );
			string targetAssemblyName = null;
			if( package.TestName != null && package.Assemblies.Contains( package.TestName ) )
			{
				targetAssemblyName = package.TestName;
				package.TestName = null;
			}
			
			TestSuite rootSuite = new TestSuite( package.FullName );
			NamespaceTreeBuilder namespaceTree = 
				new NamespaceTreeBuilder( rootSuite );

			builders.Clear();
			foreach(string assemblyName in package.Assemblies)
			{
				if ( targetAssemblyName == null || targetAssemblyName == assemblyName )
				{
					TestAssemblyBuilder builder = new TestAssemblyBuilder();
					builders.Add( builder );

					Test testAssembly =  builder.Build( assemblyName, package.TestName, autoNamespaceSuites && !mergeAssemblies );

					if ( testAssembly != null )
					{
						if (!mergeAssemblies)
						{
							rootSuite.Add(testAssembly);
						}
						else if (autoNamespaceSuites)
						{
							namespaceTree.Add(testAssembly.Tests);
							rootSuite = namespaceTree.RootSuite;
						}
						else
						{
							foreach (Test test in testAssembly.Tests)
								rootSuite.Add(test);
						}
					}
				}
			}

            ProviderCache.Clear();
            
            if (rootSuite.Tests.Count == 0)
				return null;

			return rootSuite;
		}

		private TestSuite BuildSingleAssembly( TestPackage package )
		{
			TestAssemblyBuilder builder = new TestAssemblyBuilder();
			builders.Clear();
			builders.Add( builder );

			TestSuite suite = (TestSuite)builder.Build( 
				package.FullName, 
				package.TestName, package.GetSetting( "AutoNamespaceSuites", true ) );

            ProviderCache.Clear();

            return suite;
		}
		#endregion
	}
}
