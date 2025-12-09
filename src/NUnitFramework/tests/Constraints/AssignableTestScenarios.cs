// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;

namespace NUnit.Framework.Tests.Constraints
{
    public static class AssignableTestScenarios
    {
        public static IEnumerable<TestFixtureData> GetAssignableTestScenarios()
        {
            yield return new TestFixtureData() { TypeArgs = [typeof(int), typeof(long)] };
            yield return new TestFixtureData() { TypeArgs = [typeof(int), typeof(int)] };
            yield return new TestFixtureData() { TypeArgs = [typeof(float), typeof(double)] };
            yield return new TestFixtureData() { TypeArgs = [typeof(D2), typeof(D1)] };
            yield return new TestFixtureData() { TypeArgs = [typeof(D3), typeof(D1)] };
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
