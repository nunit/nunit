// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
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

using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class ResultStateTests
    {
        [TestCase(TestStatus.Failed)]
        [TestCase(TestStatus.Skipped)]
        [TestCase(TestStatus.Inconclusive)]
        [TestCase(TestStatus.Passed)]
        public void Status_ConstructorWithOneArguments_ReturnsConstructorArgumentStatus(TestStatus status)
        {
            ResultState resultState = new ResultState(status);

            Assert.AreEqual(status, resultState.Status);
        }

        [Test]
        public void Label_ConstructorWithOneArguments_ReturnsStringEmpty()
        {
            ResultState resultState = new ResultState(TestStatus.Failed);

            Assert.AreEqual(string.Empty, resultState.Label);
        }

        [TestCase(TestStatus.Failed)]
        [TestCase(TestStatus.Skipped)]
        [TestCase(TestStatus.Inconclusive)]
        [TestCase(TestStatus.Passed)]
        public void Status_ConstructorWithTwoArguments_ReturnsConstructorArgumentStatus(TestStatus status)
        {
            ResultState resultState = new ResultState(status, string.Empty);

            Assert.AreEqual(status, resultState.Status);
        }

        [TestCase("")]
        [TestCase("label")]
        public void Label_ConstructorWithTwoArguments_ReturnsConstructorArgumentLabel(string label)
        {
            ResultState resultState = new ResultState(TestStatus.Failed, label);

            Assert.AreEqual(label, resultState.Label);
        }

        [Test]
        public void Label_ConstructorWithTwoArgumentsLabelArgumentIsNull_ReturnsEmptyString()
        {
            ResultState resultState = new ResultState(TestStatus.Failed, null);

            Assert.AreEqual(string.Empty, resultState.Label);
        }

        [Test]
        public void Site_ConstructorWithOneArguments_ReturnsTest()
        {
            ResultState resultState = new ResultState(TestStatus.Failed);

            Assert.AreEqual(FailureSite.Test, resultState.Site);
        }

        [TestCase("")]
        [TestCase("label")]
        public void Site_ConstructorWithTwoArguments_ReturnsTest(string label)
        {
            ResultState resultState = new ResultState(TestStatus.Failed, label);

            Assert.AreEqual(FailureSite.Test, resultState.Site);
        }

        [TestCase("", FailureSite.Parent)]
        [TestCase("label", FailureSite.SetUp)]
        public void Site_ConstructorWithThreeArguments_ReturnsSite(string label, FailureSite site)
        {
            ResultState resultState = new ResultState(TestStatus.Failed, label, site);

            Assert.AreEqual(site, resultState.Site);
        }

        [TestCase(TestStatus.Skipped, SpecialValue.Null, "Skipped")]
        [TestCase(TestStatus.Passed, "", "Passed")]
        [TestCase(TestStatus.Passed, "testLabel", "Passed:testLabel")]
        public void ToString_Constructor_ReturnsExpectedString(TestStatus status, string label, string expected)
        {
            ResultState resultState = new ResultState(status, label);

            Assert.AreEqual(expected, resultState.ToString());
        }

        #region EqualityTests

        [Test]
        public void TestEquality_StatusSpecified()
        {
            Assert.AreEqual(new ResultState(TestStatus.Failed), new ResultState(TestStatus.Failed));
        }

        [Test]
        public void TestEquality_StatusAndLabelSpecified()
        {
            Assert.AreEqual(new ResultState(TestStatus.Skipped, "Ignored"), new ResultState(TestStatus.Skipped, "Ignored"));
        }

        [Test]
        public void TestEquality_StatusAndSiteSpecified()
        {
            Assert.AreEqual(new ResultState(TestStatus.Failed, FailureSite.SetUp), new ResultState(TestStatus.Failed, FailureSite.SetUp));
        }

        [Test]
        public void TestEquality_StatusLabelAndSiteSpecified()
        {
            Assert.AreEqual(new ResultState(TestStatus.Failed, "Error", FailureSite.Child), new ResultState(TestStatus.Failed, "Error", FailureSite.Child));
        }

        [Test]
        public void TestEquality_StatusDiffers()
        {
            Assert.AreNotEqual(new ResultState(TestStatus.Passed), new ResultState(TestStatus.Failed));
        }

        [Test]
        public void TestEquality_LabelDiffers()
        {
            Assert.AreNotEqual(new ResultState(TestStatus.Failed, "Error"), new ResultState(TestStatus.Failed));
        }

        [Test]
        public void TestEquality_SiteDiffers()
        {
            Assert.AreNotEqual(new ResultState(TestStatus.Failed, "Error", FailureSite.Child), new ResultState(TestStatus.Failed, "Error", FailureSite.SetUp));
        }


        [Test]
        public void TestEquality_WrongType()
        {
            var rs = new ResultState(TestStatus.Passed);
            var s = "123";

            Assert.AreNotEqual(rs, s);
            Assert.AreNotEqual(s, rs);
            Assert.False(rs.Equals(s));
        }

        [Test]
        public void TestEquality_Null()
        {
            var rs = new ResultState(TestStatus.Passed);
            Assert.AreNotEqual(null, rs);
            Assert.AreNotEqual(rs, null);
            Assert.False(rs.Equals(null));
        }

        #endregion

        #region WithSite

        [TestCase(TestStatus.Failed, "Error", FailureSite.TearDown)]
        [TestCase(TestStatus.Skipped, "Ignored", FailureSite.Parent)]
        [TestCase(TestStatus.Inconclusive, "", FailureSite.SetUp)]
        public void AddSiteToResult(TestStatus status, string label, FailureSite site)
        {
            var result = new ResultState(status, label).WithSite(site);

            Assert.That(result.Status, Is.EqualTo(status));
            Assert.That(result.Label, Is.EqualTo(label));
            Assert.That(result.Site, Is.EqualTo(site));
        }

        #endregion

        #region Test Static Fields with standard ResultStates

        [Test]
        public void Inconclusive_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Inconclusive;

            Assert.AreEqual(TestStatus.Inconclusive, resultState.Status, "Status not correct.");
            Assert.AreEqual(string.Empty, resultState.Label, "Label not correct.");
            Assert.AreEqual(FailureSite.Test, resultState.Site, "Site not correct.");
        }

        [Test]
        public void NotRunnable_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.NotRunnable;

            Assert.AreEqual(TestStatus.Failed, resultState.Status, "Status not correct.");
            Assert.AreEqual("Invalid", resultState.Label, "Label not correct.");
            Assert.AreEqual(FailureSite.Test, resultState.Site, "Site not correct.");
        }

        [Test]
        public void Skipped_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Skipped;

            Assert.AreEqual(TestStatus.Skipped, resultState.Status, "Status not correct.");
            Assert.AreEqual(string.Empty, resultState.Label, "Label not correct.");
            Assert.AreEqual(FailureSite.Test, resultState.Site, "Site not correct.");
        }

        [Test]
        public void Ignored_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Ignored;

            Assert.AreEqual(TestStatus.Skipped, resultState.Status, "Status not correct.");
            Assert.AreEqual("Ignored", resultState.Label, "Label not correct.");
            Assert.AreEqual(FailureSite.Test, resultState.Site, "Site not correct.");
        }
        
        [Test]
        public void Success_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Success;

            Assert.AreEqual(TestStatus.Passed, resultState.Status, "Status not correct.");
            Assert.AreEqual(string.Empty, resultState.Label, "Label not correct.");
            Assert.AreEqual(FailureSite.Test, resultState.Site, "Site not correct.");
        }

        [Test]
        public void Failure_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Failure;

            Assert.AreEqual(TestStatus.Failed, resultState.Status, "Status not correct.");
            Assert.AreEqual(string.Empty, resultState.Label, "Label not correct.");
            Assert.AreEqual(FailureSite.Test, resultState.Site, "Site not correct.");
        }

        [Test]
        public void ChildFailure_ReturnsResultStateWithPropertiesSet()
        {
            ResultState resultState = ResultState.ChildFailure;

            Assert.AreEqual(TestStatus.Failed, resultState.Status);
            Assert.AreEqual(string.Empty, resultState.Label);
            Assert.AreEqual(FailureSite.Child, resultState.Site, "Site not correct.");
        }

        [Test]
        public void Error_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Error;

            Assert.AreEqual(TestStatus.Failed, resultState.Status, "Status not correct.");
            Assert.AreEqual("Error", resultState.Label, "Label not correct.");
            Assert.AreEqual(FailureSite.Test, resultState.Site, "Site not correct.");
        }

        [Test]
        public void Cancelled_ReturnsResultStateWithPropertiesCorrectlySet()
        {
            ResultState resultState = ResultState.Cancelled;

            Assert.AreEqual(TestStatus.Failed, resultState.Status, "Status not correct.");
            Assert.AreEqual("Cancelled", resultState.Label, "Label not correct.");
            Assert.AreEqual(FailureSite.Test, resultState.Site, "Site not correct.");
        }

        #endregion
    }
}