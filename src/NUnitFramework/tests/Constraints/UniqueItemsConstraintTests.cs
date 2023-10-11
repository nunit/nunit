// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Tests.TestUtilities.Collections;
using static NUnit.Framework.Constraints.UniqueItemsConstraint;

namespace NUnit.Framework.Tests.Constraints
{
    [TestFixture]
    public class UniqueItemsConstraintTests : ConstraintTestBase
    {
        protected override Constraint TheConstraint { get; } = new UniqueItemsConstraint();

        [SetUp]
        public void SetUp()
        {
            StringRepresentation = "<uniqueitems>";
            ExpectedDescription = "all items unique";
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly object[] SuccessData = { new[] { 1, 3, 17, -2, 34 }, Array.Empty<object>() };
        private static readonly object[] FailureData = { new object[] {
            new[] { 1, 3, 17, 3, 34 },
            "< 1, 3, 17, 3, 34 >" + Environment.NewLine + "  Not unique items: < 3 >" }
        };
#pragma warning restore IDE0052 // Remove unread private members

        [Test, SetCulture("")]
        [TestCaseSource(nameof(IgnoreCaseData))]
        public void HonorsIgnoreCase(IEnumerable actual)
        {
            var constraint = new UniqueItemsConstraint().IgnoreCase;
            var result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.False, $"{actual} should not be unique ignoring case");
        }

        private static readonly object[] IgnoreCaseData =
        {
            new object[] { new SimpleObjectCollection("x", "y", "z", "Z") },
            new object[] { new[] { 'A', 'B', 'C', 'c' } },
            new object[] { new[] { "a", "b", "c", "C" } }
        };

        private static readonly object[] DuplicateItemsData =
        {
            new object[] { new[] { 1, 2, 3, 2 }, new[] { 2 } },
            new object[] { new[] { 2, 1, 2, 3, 2 }, new[] { 2 } },
            new object[] { new[] { 2, 1, 2, 3, 3 }, new[] { 2, 3 } },
            new object[] { new[] { "x", null, "x" }, new[] { "x" } }
        };
        private static readonly IEnumerable<int> Range = Enumerable.Range(0, 10000);
        private static readonly TestCaseData[] PerformanceDataFastPath =
        {
            // Generic container
            new(Range, false),
            new(new List<int>(Range), false),
            new(new List<double>(Range.Select(v => (double)v)), false),
            new(new List<string>(Range.Select(v => v.ToString())), false),
            new(new List<string>(Range.Select(v => v.ToString())), true),

            // Non-generic container
            new(new SimpleObjectCollection(Range), false)
            {
                ArgDisplayNames = new[] { "IEnumerable<int>", "false" }
            },
            new(new SimpleObjectCollection(Range.Cast<object>()), false)
            {
                ArgDisplayNames = new[] { "IEnumerable<object>", "false" },
            },
            new(new SimpleObjectCollection(Range.Select(v => (double)v).Cast<object>()), false)
            {
                ArgDisplayNames = new[] { "IEnumerable<double>", "false" }
            },
            new(new SimpleObjectCollection(Range.Select(v => v.ToString())), false)
            {
                ArgDisplayNames = new[] { "IEnumerable<string>", "false" }
            },
            new(new SimpleObjectCollection(Range.Select(v => v.ToString())), true)
            {
                ArgDisplayNames = new[] { "IEnumerable<string>", "true" }
            },
            new(new SimpleObjectCollection(Range.Select(v => new TestReferenceType() { A = v })), true)
            {
                ArgDisplayNames = new[] { "IEnumerable<TestReferenceType>", "true" }
            },
        };

        private static TestCaseData[] PerformanceDataFastPathMixedTypes
        {
            get
            {
                var refTypes = Range.Take(5000).Select(o => new TestReferenceType() { A = o }).Cast<object>();
                var valueTypes = Range.Skip<int>(5000).Select(o => new TestValueType() { A = o }).Cast<object>();
                var container = new List<object>();

                container.AddRange(refTypes);
                container.AddRange(valueTypes);

                return new[] {
                    new TestCaseData(new SimpleObjectCollection(container), true)
                    {
                        ArgDisplayNames = new[] { "IEnumerable<dynamic>", "true" }
                    }
                };
            }
        }

        [TestCaseSource(nameof(PerformanceDataFastPath))]
        [TestCaseSource(nameof(PerformanceDataFastPathMixedTypes))]
        public void PerformanceTests_FastPath(IEnumerable values, bool ignoreCase)
        {
            Warn.Unless(() =>
            {
                if (ignoreCase)
                    Assert.That(values, Is.Unique.IgnoreCase);
                else
                    Assert.That(values, Is.Unique);
            }, HelperConstraints.HasMaxTime(100));
        }

        private static readonly IEnumerable<int> RangeSlowpath = Enumerable.Range(0, 750);
        private static readonly TestCaseData[] SlowpathData =
        {
            new(RangeSlowpath.Select(o => o.ToString()))
            {
                ArgDisplayNames = new[] { "IEnumerable<string>" }
            },
            new(RangeSlowpath.Select(o => new DateTimeOffset(o, TimeSpan.Zero)).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<DateTimeOffset>" }
            },
            new(RangeSlowpath.Select(o => (char)o).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<char>" }
            },
            new(RangeSlowpath.Select(o => (double)o).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<double>" }
            },
            new(RangeSlowpath.Select(o => new KeyValuePair<int, int>(o, o)).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<KeyValuePair<,>>" }
            },
            new(RangeSlowpath.Select(o => new DictionaryEntry(o, o)).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<DictionaryEntry>" }
            }
        };

        [TestCaseSource(nameof(SlowpathData))]
        public void SlowPath_TakenWhenSpecialTypes(IEnumerable<object> testData)
        {
            var allData = new List<object> { new TestValueType() { A = 1 } };
            allData.AddRange(testData);

            var items = new SimpleObjectCollection(allData);
            var constraint = new UniqueItemsConstraint();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            constraint.ApplyTo(items);
            stopwatch.Stop();

            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(30));
        }

        [TestCaseSource(nameof(DuplicateItemsData))]
        public void DuplicateItemsTests(IEnumerable items, IEnumerable expectedFailures)
        {
            var constraint = new UniqueItemsConstraint().IgnoreCase;
            var result = constraint.ApplyTo(items) as UniqueItemsConstraintResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.NonUniqueItems, Is.EqualTo(expectedFailures));
        }

        [Test]
        public void RespectsCultureWhenCaseIgnored()
        {
            var constraint = new UniqueItemsConstraint().IgnoreCase;
            var items = new[] { "r\u00E9sum\u00E9", "re\u0301sume\u0301" };

            var result = constraint.ApplyTo(items) as UniqueItemsConstraintResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void DoesntRespectCultureWhenCasingMatters()
        {
            var constraint = new UniqueItemsConstraint();
            var items = new[] { "r\u00E9sum\u00E9", "re\u0301sume\u0301" };

            var result = constraint.ApplyTo(items) as UniqueItemsConstraintResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.True);
        }

        private static TestCaseData[] RequiresDefaultComparer
        {
            get
            {
                var sameRef = new TestReferenceType() { A = 1 };

                return new[] {
                    new TestCaseData() {
                        Arguments = new object[]
                        {
                            new SimpleObjectCollection(new TestValueType() { A = 1 }, new TestValueType() { A = 2 }),
                            true
                        },
                        ArgDisplayNames = new[] { "ValueTypes", "true" }
                    },

                    new TestCaseData() {
                        Arguments = new object[]
                        {
                            new SimpleObjectCollection(new TestValueType() { A = 1 }, new TestValueType() { A = 1 }),
                            false
                        },
                        ArgDisplayNames = new[] { "ValueTypes", "false" }
                    },

                    new TestCaseData() {
                        Arguments = new object[]
                        {
                            new SimpleObjectCollection(new TestReferenceType() { A = 1 }, new TestReferenceType() { A = 1 }),
                            true
                        },
                        ArgDisplayNames = new[] { "ReferenceTypes", "true" }
                    },

                    new TestCaseData() {
                        Arguments = new object[]
                        {
                            new SimpleObjectCollection(sameRef, sameRef),
                            false
                        },
                        ArgDisplayNames = new[] { "ReferenceTypes", "false" }
                    },

                    new TestCaseData() {
                        Arguments = new object[]
                        {
                            new SimpleObjectCollection(
                                new TestReferenceTypeOverridesEquals() { A = 1 },
                                new TestReferenceTypeOverridesEquals() { A = 1 }),
                            false
                        },
                        ArgDisplayNames = new[] { "ReferenceTypesOverridesEquals", "false" }
                    },

                    new TestCaseData() {
                        Arguments = new object[]
                        {
                            new SimpleObjectCollection(new TestValueType() { A = 1 }, new TestReferenceType() { A = 1 }),
                            true
                        },
                        ArgDisplayNames = new[] { "MixedTypes", "true" }
                    },

                    new TestCaseData() {
                        Arguments = new object[]
                        {
                            new SimpleObjectCollection(new TestValueType() { A = 1 }, sameRef, sameRef),
                            false
                        },
                        ArgDisplayNames = new[] { "MixedTypes", "false" }
                    }
                };
            }
        }

        [TestCaseSource(nameof(RequiresDefaultComparer))]
        public void DuplicateItemsTests_RequiresDefaultComparer(IEnumerable items, bool success)
        {
            var constraint = new UniqueItemsConstraint();
            var result = constraint.ApplyTo(items) as UniqueItemsConstraintResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsSuccess, Is.EqualTo(success));
        }

        private sealed class TestReferenceType
        {
            public int A { get; set; }
        }

        private sealed class TestReferenceTypeOverridesEquals
        {
            public int A { get; set; }

            public override bool Equals(object? obj)
            {
                if (obj is TestReferenceTypeOverridesEquals other)
                    return other.A == A;
                return false;
            }

            public override int GetHashCode()
            {
                return A.GetHashCode();
            }
        }

        private struct TestValueType
        {
            public int A { get; set; }
        }
    }
}
