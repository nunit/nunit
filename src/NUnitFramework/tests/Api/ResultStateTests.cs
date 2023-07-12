// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Tests.Api
{
    [TestFixture]
    public class ResultStateTests
    {
        [TestCase(TestStatus.Failed)]
        [TestCase(TestStatus.Skipped)]
        [TestCase(TestStatus.Inconclusive)]
        [TestCase(TestStatus.Passed)]
        [TestCase(TestStatus.Warning)]
        public void Status_ConstructorWithOneArguments_ReturnsConstructorArgumentStatus(TestStatus status)
        {
            ResultState resultState = new ResultState(status);

            Assert.That(resultState.Status, Is.EqualTo(status));
        }

        [Test]
        public void Label_ConstructorWithOneArguments_ReturnsStringEmpty()
        {
            ResultState resultState = new ResultState(TestStatus.Failed);

            Assert.That(resultState.Label, Is.EqualTo(string.Empty));
        }

        [TestCase(TestStatus.Failed)]
        [TestCase(TestStatus.Skipped)]
        [TestCase(TestStatus.Inconclusive)]
        [TestCase(TestStatus.Passed)]
        [TestCase(TestStatus.Warning)]
        public void Status_ConstructorWithTwoArguments_ReturnsConstructorArgumentStatus(TestStatus status)
        {
            ResultState resultState = new ResultState(status, string.Empty);

            Assert.That(resultState.Status, Is.EqualTo(status));
        }

        [TestCase("")]
        [TestCase("label")]
        public void Label_ConstructorWithTwoArguments_ReturnsConstructorArgumentLabel(string label)
        {
            ResultState resultState = new ResultState(TestStatus.Failed, label);

            Assert.That(resultState.Label, Is.EqualTo(label));
        }

        [Test]
        public void Label_ConstructorWithTwoArgumentsLabelArgumentIsNull_ReturnsEmptyString()
        {
            ResultState resultState = new ResultState(TestStatus.Failed, null);

            Assert.That(resultState.Label, Is.EqualTo(string.Empty));
        }

        [Test]
        public void Site_ConstructorWithOneArguments_ReturnsTest()
        {
            ResultState resultState = new ResultState(TestStatus.Failed);

            Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test));
        }

        [TestCase("")]
        [TestCase("label")]
        public void Site_ConstructorWithTwoArguments_ReturnsTest(string label)
        {
            ResultState resultState = new ResultState(TestStatus.Failed, label);

            Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test));
        }

        [TestCase("", FailureSite.Parent)]
        [TestCase("label", FailureSite.SetUp)]
        public void Site_ConstructorWithThreeArguments_ReturnsSite(string label, FailureSite site)
        {
            ResultState resultState = new ResultState(TestStatus.Failed, label, site);

            Assert.That(resultState.Site, Is.EqualTo(site));
        }

        [TestCase(TestStatus.Skipped, null, "Skipped")]
        [TestCase(TestStatus.Passed, "", "Passed")]
        [TestCase(TestStatus.Passed, "testLabel", "Passed:testLabel")]
        public void ToString_Constructor_ReturnsExpectedString(TestStatus status, string label, string expected)
        {
            ResultState resultState = new ResultState(status, label);

            Assert.That(resultState.ToString(), Is.EqualTo(expected));
        }

        #region EqualityTests

        [Test]
        public void TestEquality_StatusSpecified()
        {
            Assert.That(new ResultState(TestStatus.Failed), Is.EqualTo(new ResultState(TestStatus.Failed)));
        }

        [Test]
        public void TestEquality_StatusAndLabelSpecified()
        {
            Assert.That(new ResultState(TestStatus.Skipped, "Ignored"), Is.EqualTo(new ResultState(TestStatus.Skipped, "Ignored")));
        }

        [Test]
        public void TestEquality_StatusAndSiteSpecified()
        {
            Assert.That(new ResultState(TestStatus.Failed, FailureSite.SetUp), Is.EqualTo(new ResultState(TestStatus.Failed, FailureSite.SetUp)));
        }

        [Test]
        public void TestEquality_StatusLabelAndSiteSpecified()
        {
            Assert.That(new ResultState(TestStatus.Failed, "Error", FailureSite.Child), Is.EqualTo(new ResultState(TestStatus.Failed, "Error", FailureSite.Child)));
        }

        [Test]
        public void TestEquality_StatusDiffers()
        {
            Assert.That(new ResultState(TestStatus.Failed), Is.Not.EqualTo(new ResultState(TestStatus.Passed)));
        }

        [Test]
        public void TestEquality_LabelDiffers()
        {
            Assert.That(new ResultState(TestStatus.Failed), Is.Not.EqualTo(new ResultState(TestStatus.Failed, "Error")));
        }

        [Test]
        public void TestEquality_SiteDiffers()
        {
            Assert.That(new ResultState(TestStatus.Failed, "Error", FailureSite.SetUp), Is.Not.EqualTo(new ResultState(TestStatus.Failed, "Error", FailureSite.Child)));
        }


        [Test]
        public void TestEquality_WrongType()
        {
            var rs = new ResultState(TestStatus.Passed);
            var s = "123";

#pragma warning disable NUnit2021 // Incompatible types for EqualTo constraint
            Assert.That(s, Is.Not.EqualTo(rs));
            Assert.That(rs, Is.Not.EqualTo(s));
#pragma warning restore NUnit2021 // Incompatible types for EqualTo constraint
        }

        [Test]
        public void TestEquality_Null()
        {
            var rs = new ResultState(TestStatus.Passed);

            // Ensure real Equals fails, before checking nunit constraint
#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure
            Assert.That(rs.Equals(null), Is.False);
#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

            Assert.That(rs, Is.Not.EqualTo(null));
            Assert.That(default(ResultState), Is.Not.EqualTo(rs));
        }

        #endregion

        #region WithSite

        [TestCase(TestStatus.Failed, "Error", FailureSite.TearDown)]
        [TestCase(TestStatus.Skipped, "Ignored", FailureSite.Parent)]
        [TestCase(TestStatus.Inconclusive, "", FailureSite.SetUp)]
        public void AddSiteToResult(TestStatus status, string label, FailureSite site)
        {
            var result = new ResultState(status, label).WithSite(site);

            Assert.Multiple(() =>
            {
                Assert.That(result.Status, Is.EqualTo(status));
                Assert.That(result.Label, Is.EqualTo(label));
                Assert.That(result.Site, Is.EqualTo(site));
            });
        }

        #endregion

        #region Test Static Fields with standard ResultStates

        [Test]
        public void Inconclusive_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Inconclusive;

            Assert.Multiple(() =>
            {
                Assert.That(resultState.Status, Is.EqualTo(TestStatus.Inconclusive), "Status not correct.");
                Assert.That(resultState.Label, Is.EqualTo(string.Empty), "Label not correct.");
                Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
            });
        }

        [Test]
        public void NotRunnable_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.NotRunnable;

            Assert.Multiple(() =>
            {
                Assert.That(resultState.Status, Is.EqualTo(TestStatus.Failed), "Status not correct.");
                Assert.That(resultState.Label, Is.EqualTo("Invalid"), "Label not correct.");
                Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
            });
        }

        [Test]
        public void Skipped_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Skipped;

            Assert.Multiple(() =>
            {
                Assert.That(resultState.Status, Is.EqualTo(TestStatus.Skipped), "Status not correct.");
                Assert.That(resultState.Label, Is.EqualTo(string.Empty), "Label not correct.");
                Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
            });
        }

        [Test]
        public void Ignored_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Ignored;

            Assert.Multiple(() =>
            {
                Assert.That(resultState.Status, Is.EqualTo(TestStatus.Skipped), "Status not correct.");
                Assert.That(resultState.Label, Is.EqualTo("Ignored"), "Label not correct.");
                Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
            });
        }

        [Test]
        public void Success_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Success;

            Assert.That(resultState.Status, Is.EqualTo(TestStatus.Passed), "Status not correct.");
            Assert.That(resultState.Label, Is.EqualTo(string.Empty), "Label not correct.");
            Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
        }

        [Test]
        public void Warning_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Warning;

            Assert.That(resultState.Status, Is.EqualTo(TestStatus.Warning), "Status not correct.");
            Assert.That(resultState.Label, Is.EqualTo(string.Empty), "Label not correct.");
            Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
        }

        [Test]
        public void Failure_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Failure;

            Assert.That(resultState.Status, Is.EqualTo(TestStatus.Failed), "Status not correct.");
            Assert.That(resultState.Label, Is.EqualTo(string.Empty), "Label not correct.");
            Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
        }

        [Test]
        public void ChildFailure_ReturnsResultStateWithPropertiesSet()
        {
            ResultState resultState = ResultState.ChildFailure;

            Assert.That(resultState.Status, Is.EqualTo(TestStatus.Failed));
            Assert.That(resultState.Label, Is.EqualTo(string.Empty));
            Assert.That(resultState.Site, Is.EqualTo(FailureSite.Child), "Site not correct.");
        }

        [Test]
        public void Error_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Error;

            Assert.That(resultState.Status, Is.EqualTo(TestStatus.Failed), "Status not correct.");
            Assert.That(resultState.Label, Is.EqualTo("Error"), "Label not correct.");
            Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
        }

        [Test]
        public void Cancelled_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Cancelled;

            Assert.That(resultState.Status, Is.EqualTo(TestStatus.Failed), "Status not correct.");
            Assert.That(resultState.Label, Is.EqualTo("Cancelled"), "Label not correct.");
            Assert.That(resultState.Site, Is.EqualTo(FailureSite.Test), "Site not correct.");
        }

        #endregion
    }
}
