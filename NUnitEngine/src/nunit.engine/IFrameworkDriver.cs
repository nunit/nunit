using System;
using System.Collections.Generic;
using System.Xml;

namespace NUnit.Engine
{
    public interface IFrameworkDriver
    {
        XmlNode ExploreTests(string assemblyName, IDictionary<string,object> options);

        bool Load(string assemblyName, IDictionary<string,object> options);

        void Unload();

        TestResult Run(IDictionary<string,object> options, ITestEventHandler listener);
    }
}
