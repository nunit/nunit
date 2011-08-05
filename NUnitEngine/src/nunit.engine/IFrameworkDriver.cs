using System;
using System.Collections.Generic;
using System.Xml;

namespace NUnit.Engine
{
    public interface IFrameworkDriver
    {
        TestEngineResult Load(string assemblyName, IDictionary<string,object> options);

        void Unload();

        TestEngineResult Run(ITestEventHandler listener, TestFilter filter);

        TestEngineResult Explore(string assemblyName, IDictionary<string, object> options);
    }
}
