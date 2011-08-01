using System;
using System.Collections.Generic;
using System.Xml;

namespace NUnit.Engine
{
    public interface IFrameworkDriver
    {
        TestEngineResult Load(string assemblyName, IDictionary<string,object> options);

        void Unload();

        TestEngineResult Run(IDictionary<string,object> options, ITestEventHandler listener);

        TestEngineResult Explore(string assemblyName, IDictionary<string, object> options);
    }
}
