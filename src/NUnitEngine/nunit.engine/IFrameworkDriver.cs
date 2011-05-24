using System;
using System.Collections;
using System.Xml;

namespace NUnit.Engine
{
    public interface IFrameworkDriver
    {
        XmlNode ExploreTests(string assemblyName, IDictionary options);

        bool Load(string assemblyName, IDictionary options);

        void Unload();

        TestResult Run(IDictionary options);
    }
}
