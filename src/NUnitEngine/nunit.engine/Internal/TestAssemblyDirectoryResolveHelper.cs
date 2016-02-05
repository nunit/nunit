using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace NUnit.Engine.Internal
{
    public class TestAssemblyDirectoryResolveHelper
    {
        public static bool AssemblyHasAttribute(string assemblyPath)
        {
            var domain = AppDomain.CreateDomain("TestAssemblyDirectoryResolveHelperCheckDomain");
            try
            {
                var agentType = typeof(AttributeCheckAgent);
                var agent = domain.CreateInstanceFromAndUnwrap(agentType.Assembly.CodeBase, agentType.FullName) as AttributeCheckAgent;
                bool helperRequested = agent.HelperRequested(assemblyPath);
                return helperRequested;
                
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }

        private class AttributeCheckAgent : MarshalByRefObject
        {
            private const string ATTRIBUTE = "NUnit.Framework.TestAssemblyDirectoryResolveAttribute";

            public bool HelperRequested(string assemblyPath)
            {
                // You can't get custom attributes when the assembly is loaded reflection only
                var testAssembly = Assembly.LoadFrom(assemblyPath);
                var allAttrs = testAssembly.GetCustomAttributes(false);
                bool hasAttr = Array.Exists(allAttrs, attr => attr.GetType().FullName == ATTRIBUTE);
                return hasAttr;
            }
        }
    }
}
