// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Tests.Constraints
{
    public static class AssignableTestScenarios
    {
        public static IEnumerable<(Type from, Type to)> GetAssignableTestScenarios()
        {
            yield return (from: typeof(int), to: typeof(long));
            yield return (from: typeof(float), to: typeof(double));
            yield return (from: typeof(D2), to: typeof(D1));
            yield return (from: typeof(D3), to: typeof(D1));
        }

        public class D1
        {
        }

        public class D2 : D1
        {
        }

        public class D3
        {
            public static implicit operator D1(D3 _) => new();
        }

        public class B
        {
        }
    }
}
