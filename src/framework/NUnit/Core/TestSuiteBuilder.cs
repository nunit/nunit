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

using NUnit.Core.Builders;
using System.Collections;

namespace NUnit.Core
{
	/// <summary>
	/// TestSuiteBuilder builds a test suite from one or more assemblies 
    /// specified in a TestPackage.
	/// </summary>
	public class TestSuiteBuilder
	{
		#region Instance Variables

		private ObjectList builders = new ObjectList();

		#endregion

		#region Properties
        /// <summary>
        /// Gets information about all loaded assemblies
        /// </summary>
		public TestAssemblyInfo[] AssemblyInfo
		{
			get
			{
                TestAssemblyInfo[] info = new TestAssemblyInfo[builders.Count];
                int index = 0;
				foreach( TestAssemblyBuilder builder in this.builders )
					info[index++] = builder.AssemblyInfo;

				return info;
			}
		}
		#endregion

		#region Build Method
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

					TestSuite testAssembly =  builder.Build( assemblyName, package.TestName, autoNamespaceSuites && !mergeAssemblies );

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

            if (package.IsSingleAssembly)
                return (TestSuite)rootSuite.Tests[0];

			return rootSuite;
		}
		#endregion
	}
}
