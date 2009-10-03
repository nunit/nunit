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
    public class CombinatorialStrategy : CombiningStrategy
    {
        public CombinatorialStrategy(IEnumerable[] sources) : base(sources) { }

        public override IEnumerable GetTestCases()
        {
            IEnumerator[] enumerators = new IEnumerator[Sources.Length];
            int index = -1;

			ArrayList testCases = new ArrayList();

            for (; ; )
            {
                while (++index < Sources.Length)
                {
                    enumerators[index] = Sources[index].GetEnumerator();
                    if (!enumerators[index].MoveNext())
						return testCases;
                }

                object[] testdata = new object[Sources.Length];

                for (int i = 0; i < Sources.Length; i++)
                    testdata[i] = enumerators[i].Current;

				testCases.Add(testdata);

                index = Sources.Length;

                while (--index >= 0 && !enumerators[index].MoveNext()) ;

                if (index < 0) break;
            }

			return testCases;
        }
    }
}
