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
using System.Reflection;
using System.Collections;

namespace NUnit.Core.Extensibility
{
    public class FrameworkRegistry : ExtensionPoint, IFrameworkRegistry
    {
		#region Constructor
		public FrameworkRegistry( IExtensionHost host )
			: base( "FrameworkRegistry", host ) { }
		#endregion Constructor

        #region Instance Fields
        /// <summary>
        /// List of FrameworkInfo structs for supported frameworks
        /// </summary>
        private Hashtable testFrameworks = new Hashtable();
        #endregion

        #region IFrameworkRegistry Members
        /// <summary>
        /// Register a framework. NUnit registers itself using this method. Add-ins that
        /// work with or emulate a different framework may register themselves as well.
        /// </summary>
        /// <param name="frameworkName">The name of the framework</param>
        /// <param name="assemblyName">The name of the assembly that framework users reference</param>
        public void Register(string frameworkName, string assemblyName)
        {
            testFrameworks[frameworkName] = new TestFramework(frameworkName, assemblyName);
        }
		#endregion

		#region ExtensionPoint overrides
        protected override bool IsValidExtension(object extension)
		{
			return extension is TestFramework;
		}

		#endregion

		#region Other Methods
        /// <summary>
        /// Get a list of known frameworks referenced by an assembly
        /// </summary>
        /// <param name="assembly">The assembly to be examined</param>
        /// <returns>A list of AssemblyNames</returns>
        public IList GetReferencedFrameworks(Assembly assembly)
        {
            ArrayList referencedAssemblies = new ArrayList();

            foreach (AssemblyName assemblyRef in assembly.GetReferencedAssemblies())
            {
                foreach (TestFramework info in testFrameworks.Values)
                {
                    if (assemblyRef.Name == info.AssemblyName)
                    {
                        referencedAssemblies.Add(assemblyRef);
                        break;
                    }
                }
            }

            return referencedAssemblies;
        }
        #endregion
    }
}
