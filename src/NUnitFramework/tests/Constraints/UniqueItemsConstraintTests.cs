// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using static NUnit.Framework.Constraints.UniqueItemsConstraint;

namespace NUnit.Framework.Constraints
{
    [TestFixture]
    public class UniqueItemsConstraintTests : ConstraintTestBase
    {
        [SetUp]
        public void SetUp()
        {
            TheConstraint = new UniqueItemsConstraint();
            StringRepresentation = "<uniqueitems>";
            ExpectedDescription = "all items unique";
        }

        static object[] SuccessData = new object[] { new int[] { 1, 3, 17, -2, 34 }, Array.Empty<object>() };
        static object[] FailureData = new object[] { new object[] {
            new int[] { 1, 3, 17, 3, 34 },
            "< 1, 3, 17, 3, 34 >" + Environment.NewLine + "  Not unique items: < 3 >" }
        };

        [Test, SetCulture("")]
        [TestCaseSource(nameof(IgnoreCaseData))]
        public void HonorsIgnoreCase(IEnumerable actual)
        {
            var constraint = new UniqueItemsConstraint().IgnoreCase;
            var result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.False, "{0} should not be unique ignoring case", actual);
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
            new object[] {new[] { 2, 1, 2, 3, 3 }, new[] { 2, 3 }},
            new object[] {new[] { "x", null, "x" }, new[] { "x" }}
        };

        static readonly IEnumerable<int> RANGE = Enumerable.Range(0, 10000);

        static readonly TestCaseData[] PerformanceData_FastPath =
        {
            // Generic container
            new TestCaseData(RANGE, false),
            new TestCaseData(new List<int>(RANGE), false),
            new TestCaseData(new List<double>(RANGE.Select(v => (double)v)), false),
            new TestCaseData(new List<string>(RANGE.Select(v => v.ToString())), false),
            new TestCaseData(new List<string>(RANGE.Select(v => v.ToString())), true),
            
            // Non-generic container
            new TestCaseData(new SimpleObjectCollection(RANGE), false)
            {
                ArgDisplayNames = new[] { "IEnumerable<int>", "false" }
            },
            new TestCaseData(new SimpleObjectCollection(RANGE.Cast<object>()), false)
            {
                ArgDisplayNames = new[] { "IEnumerable<object>", "false" },
            },
            new TestCaseData(new SimpleObjectCollection(RANGE.Select(v => (double)v).Cast<object>()), false)
            {
                ArgDisplayNames = new[] { "IEnumerable<double>", "false" }
            },
            new TestCaseData(new SimpleObjectCollection(RANGE.Select(v => v.ToString()).Cast<object>()), false)
            {
                ArgDisplayNames = new[] { "IEnumerable<string>", "false" }
            },
            new TestCaseData(new SimpleObjectCollection(RANGE.Select(v => v.ToString()).Cast<object>()), true)
            {
                ArgDisplayNames = new[] { "IEnumerable<string>", "true" }
            },
            new TestCaseData(new SimpleObjectCollection(RANGE.Select(v => new TestReferenceType() { A = v }).Cast<object>()), true)
            {
                ArgDisplayNames = new[] { "IEnumerable<TestReferenceType>", "true" }
            },
        };
        static TestCaseData[] PerformanceData_FastPath_MixedTypes
        {
            get
            {
                var refTypes = RANGE.Take(5000).Select(o => new TestReferenceType() { A = o }).Cast<object>();
                var valueTypes = RANGE.Skip<int>(5000).Select(o => new TestValueType() { A = o }).Cast<object>();
                var container = new List<object>();

                container.AddRange(refTypes);
                container.AddRange(valueTypes);

                return new TestCaseData[] {
                    new TestCaseData(new SimpleObjectCollection(container.Cast<object>()), true)
                    {
                        ArgDisplayNames = new[] { "IEnumerable<dynamic>", "true" }
                    }
                };
            }
        }

        [TestCaseSource(nameof(PerformanceData_FastPath))]
        [TestCaseSource(nameof(PerformanceData_FastPath_MixedTypes))]
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

        private static IEnumerable<int> RANGE_SLOWPATH = Enumerable.Range(0, 750);
        private static readonly TestCaseData[] SlowpathData =
        {
            new TestCaseData(RANGE_SLOWPATH.Select(o => o.ToString()).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<string>" }
            },
            new TestCaseData(RANGE_SLOWPATH.Select(o => new DateTimeOffset(o, TimeSpan.Zero)).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<DateTimeOffset>" }
            },
            new TestCaseData(RANGE_SLOWPATH.Select(o => (char)o).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<char>" }
            },
            new TestCaseData(RANGE_SLOWPATH.Select(o => (double)o).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<double>" }
            },
            new TestCaseData(RANGE_SLOWPATH.Select(o => new KeyValuePair<int, int>(o, o)).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<KeyValuePair<,>>" }
            },
            new TestCaseData(RANGE_SLOWPATH.Select(o => new DictionaryEntry(o, o)).Cast<object>())
            {
                ArgDisplayNames = new[] { "IEnumerable<DictionaryEntry>" }
            }
        };

        [TestCaseSource(nameof(SlowpathData))]
        public void SlowPath_TakenWhenSpecialTypes(IEnumerable<object> testData)
        {
            var allData = new List<object>();
            allData.Add(new TestValueType() { A = 1 });
            allData.AddRange(testData);

            var items = new SimpleObjectCollection((IEnumerable<object>)allData);
            var constraint = new UniqueItemsConstraint();
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            constraint.ApplyTo(items);
            stopwatch.Stop();

            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(50));
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

            Assert.That(result.IsSuccess, Is.False);
        }

        [Test]
        public void DoesntRespectCultureWhenCasingMatters()
        {
            var constraint = new UniqueItemsConstraint();
            var items = new[] { "r\u00E9sum\u00E9", "re\u0301sume\u0301" };

            var result = constraint.ApplyTo(items) as UniqueItemsConstraintResult;

            Assert.That(result.IsSuccess, Is.True);
        }

        private static TestCaseData[] RequiresDefaultComparer
        {
            get
            {
                var sameRef = new TestReferenceType() { A = 1 };

                return new TestCaseData[] {
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
                                new TestReferenceType_OverridesEquals() { A = 1 },
                                new TestReferenceType_OverridesEquals() { A = 1 }
                            ),
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
            Assert.That(result.IsSuccess, Is.EqualTo(success));
        }

        private sealed class TestReferenceType
        {
            public int A { get; set; }
        }

        private sealed class TestReferenceType_OverridesEquals
        {
            public int A { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is TestReferenceType_OverridesEquals other)
                    return other.A == this.A;
                return false;
            }

            public override int GetHashCode()
            {
                return this.A.GetHashCode();
            }
        }

        private struct TestValueType
        {
            public int A { get; set; }
        }
    }
}
