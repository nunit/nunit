// ***********************************************************************
// Copyright (c) 2007 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.TestUtilities;
using NUnit.TestUtilities.Collections;
using static NUnit.Framework.Constraints.UniqueItemsConstraint;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class UniqueItemsTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new UniqueItemsConstraint();
            StringRepresentation = "<uniqueitems>";
            ExpectedDescription = "all items unique";
        }

        static object[] SuccessData = new object[] { new int[] { 1, 3, 17, -2, 34 }, new object[0] };
        static object[] FailureData = new object[] { new object[] { new int[] { 1, 3, 17, 3, 34 }, "non-unique: < 3 >" } };

        [Test]
        [TestCaseSource( nameof(IgnoreCaseData) )]
        public void HonorsIgnoreCase( IEnumerable actual )
        {
            Assert.That( new UniqueItemsConstraint().IgnoreCase.ApplyTo( actual ).IsSuccess, Is.False, "{0} should not be unique ignoring case", actual );
        }

        private static readonly object[] IgnoreCaseData =
        {
            new object[] {new SimpleObjectCollection("x", "y", "z", "Z")},
            new object[] {new[] {'A', 'B', 'C', 'c'}},
            new object[] {new[] {"a", "b", "c", "C"}}
        };

        private static readonly object[] DuplicateItemsData =
        {
            new object[] {new[] { 1, 2, 3, 2 }, new[] { 2 }},
            new object[] {new[] { 2, 1, 2, 3, 2 }, new[] { 2 }},
            new object[] {new[] { 2, 1, 2, 3, 3 }, new[] { 2, 3 }}
        };

        static readonly IEnumerable<int> RANGE = Enumerable.Range(0, 10000);

        static readonly TestCaseData[] PerformanceData =
        {
            new TestCaseData(RANGE, false),
            new TestCaseData(new List<int>(RANGE), false),
            new TestCaseData(new List<double>(RANGE.Select(v => (double)v)), false),
            new TestCaseData(new List<string>(RANGE.Select(v => v.ToString())), false),
            new TestCaseData(new List<string>(RANGE.Select(v => v.ToString())), true)
        };

        [TestCaseSource(nameof(PerformanceData))]
        public void PerformanceTests(IEnumerable values, bool ignoreCase)
        {
            Warn.Unless(() =>
            {
                if (ignoreCase)
                    Assert.That(values, Is.Unique.IgnoreCase);
                else
                    Assert.That(values, Is.Unique);
            }, HelperConstraints.HasMaxTime(100));
        }

        [TestCaseSource(nameof(DuplicateItemsData))]
        public void DuplicateItemsTests(IEnumerable items, IEnumerable expectedFailures)
        {
            var result = Is.Unique.ApplyTo(items) as UniqueItemsContstraintResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.NonUniqueItems, Is.EqualTo(expectedFailures));
        }
    }
}
