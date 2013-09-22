using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Engine;
using NUnit.Framework;

namespace NUnit.ConsoleRunner.Tests
{
    /// <summary>
    /// This is the abstract base for all XML output tests,
    /// which need to have a copy of the test engine. Creating
    /// more than one test engine in the test domain causes
    /// an error, so this class supports a single instance of it.
    /// </summary>
    public abstract class XmlOutputTest
    {
        private static ITestEngine engine;

        public static ITestEngine TestEngine
        {
            get
            {
                if (engine == null)
                    engine = TestEngineActivator.CreateInstance(null, InternalTraceLevel.Off);

                return engine;
            }
        }
    }
}
