// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.TestData.RepeatingTests;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    /// <summary>
    /// Tests covering the StopOnFailure dual-property behaviour for RepeatAttribute.
    /// Each test corresponds to a row in the design table:
    ///
    /// | Usage                                              | _stopOnFailureDefault | Explicitly set? | Effective value |
    /// |----------------------------------------------------|-----------------------|-----------------|-----------------|
    /// | [Repeat(5)]                                        | true                  | no              | true            |
    /// | [Repeat(5, false)]                                 | false                 | no              | false           |
    /// | [Repeat(5, StopOnFailure=false)]                   | true                  | yes -> false    | false           |
    /// | [Repeat(5, StopOnFailure=true)]                    | true                  | yes -> true     | true            |
    /// | [Repeat(5, RequiredPassPercentage=80)]              | true                  | no              | no error        |
    /// | [Repeat(5, StopOnFailure=false, RequiredPassPercentage=80)] | true         | yes -> false    | no error        |
    /// | [Repeat(5, StopOnFailure=true,  RequiredPassPercentage=80)] | true         | yes -> true     | ERROR           |
    /// </summary>
    [TestFixture]
    public class RepeatAttributeStopOnFailureTests
    {
        // Row 1: [Repeat(5)] — StopOnFailure not set; defaults true via constructor.
        // Expected: stops after first failure, Count=1.
        [Test]
        public void Row1_DefaultStopOnFailure_StopsAfterFirstFailure_Issue5220()
        {
            var fixture = new StopOnFailureDualProp_Row1_DefaultFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(StopOnFailureDualProp_Row1_DefaultFixture.AlwaysFails));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(fixture.Count, Is.EqualTo(1), "Should stop after first failure — StopOnFailure defaults to true");
            });
        }

        // Row 2: [Repeat(5, false)] — false via constructor argument.
        // Expected: runs all 5, Count=5.
        [Test]
        public void Row2_CtorFalse_RunsAllRepetitions_Issue5220()
        {
            var fixture = new StopOnFailureDualProp_Row2_CtorFalseFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(StopOnFailureDualProp_Row2_CtorFalseFixture.AlwaysFails));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(fixture.Count, Is.EqualTo(5), "Should run all 5 — StopOnFailure=false via constructor");
            });
        }

        // Row 3: [Repeat(5, StopOnFailure=false)] — false via property.
        // Expected: explicit false overrides the constructor default of true, Count=5.
        [Test]
        public void Row3_PropertyFalse_RunsAllRepetitions_Issue5220()
        {
            var fixture = new StopOnFailureDualProp_Row3_PropertyFalseFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(StopOnFailureDualProp_Row3_PropertyFalseFixture.AlwaysFails));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(fixture.Count, Is.EqualTo(5), "Should run all 5 — explicit StopOnFailure=false via property overrides constructor default");
            });
        }

        // Row 4: [Repeat(5, StopOnFailure=true)] — true via property.
        // Expected: stops after first failure, Count=1.
        [Test]
        public void Row4_PropertyTrue_StopsAfterFirstFailure_Issue5220()
        {
            var fixture = new StopOnFailureDualProp_Row4_PropertyTrueFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(StopOnFailureDualProp_Row4_PropertyTrueFixture.AlwaysFails));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Failed));
                Assert.That(fixture.Count, Is.EqualTo(1), "Should stop after first failure — explicit StopOnFailure=true via property");
            });
        }

        // Row 5: [Repeat(5, RequiredPassPercentage=80)] — threshold with StopOnFailure not explicitly set.
        // Expected: no error, all 5 runs complete, result is Passed.
        [Test]
        public void Row5_ThresholdWithDefaultStopOnFailure_NoError_Issue5220()
        {
            var fixture = new StopOnFailureDualProp_Row5_ThresholdDefaultFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(StopOnFailureDualProp_Row5_ThresholdDefaultFixture.AlwaysPasses));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(fixture.Count, Is.EqualTo(5), "All 5 should run — StopOnFailure not explicitly set, so no conflict with threshold");
            });
        }

        // Row 6: [Repeat(5, StopOnFailure=false, RequiredPassPercentage=80)] — threshold with explicit false.
        // Expected: no error (false is compatible with threshold), all 5 runs complete.
        [Test]
        public void Row6_ThresholdWithExplicitFalse_NoError_Issue5220()
        {
            var fixture = new StopOnFailureDualProp_Row6_ThresholdExplicitFalseFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(StopOnFailureDualProp_Row6_ThresholdExplicitFalseFixture.AlwaysPasses));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState.Status, Is.EqualTo(TestStatus.Passed));
                Assert.That(fixture.Count, Is.EqualTo(5), "All 5 should run — explicit false is valid with threshold");
            });
        }

        // Row 7: [Repeat(5, StopOnFailure=true, RequiredPassPercentage=80)] — explicit true conflicts with threshold.
        // Expected: ResultState.Error, test method never runs (Count=0).
        // NOTE: This test is RED until the dual-property implementation is complete.
        [Test]
        public void Row7_ThresholdWithExplicitTrue_ProducesError_Issue5220()
        {
            var fixture = new StopOnFailureDualProp_Row7_ThresholdExplicitTrueFixture();
            ITestResult result = TestBuilder.RunTestCase(fixture, nameof(StopOnFailureDualProp_Row7_ThresholdExplicitTrueFixture.AlwaysPasses));

            Assert.Multiple(() =>
            {
                Assert.That(result.ResultState, Is.EqualTo(ResultState.Error));
                Assert.That(fixture.Count, Is.EqualTo(0), "Test method must not run when configuration is invalid");
            });
        }
    }
}
