// ****************************************************************
// Copyright 2008, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;
using System.Reflection;
using NUnit.Core.Extensibility;

namespace NUnit.Core.Builders
{
    public class SequentialStrategy : CombiningStrategy
    {
        public SequentialStrategy(IEnumerable[] sources) : base(sources) { }

        public override IEnumerable GetTestCases()
        {
			ArrayList testCases = new ArrayList();

            for (; ; )
            {
                bool gotData = false;
                object[] testdata = new object[Sources.Length];

                for (int i = 0; i < Sources.Length; i++)
                    if (Enumerators[i].MoveNext())
                    {
                        testdata[i] = Enumerators[i].Current;
                        gotData = true;
                    }
                    else
                        testdata[i] = null;

                if (!gotData)
                    break;

                testCases.Add(testdata);
            }

            return testCases;
        }
    }
}
