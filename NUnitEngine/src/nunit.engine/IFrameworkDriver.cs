using System;
#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
#endif
using System.Xml;

namespace NUnit.Engine
{
    public interface IFrameworkDriver
    {
#if CLR_2_0 || CLR_4_0
        XmlNode ExploreTests(string assemblyName, IDictionary<string,object> options);

        bool Load(string assemblyName, IDictionary<string,object> options);

        void Unload();

        TestResult Run(IDictionary<string,object> options);
#else
        XmlNode ExploreTests(string assemblyName, IDictionary options);

        bool Load(string assemblyName, IDictionary options);

        void Unload();

        TestResult Run(IDictionary options);
#endif
    }
}
