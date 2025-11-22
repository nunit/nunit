// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using NUnit.Framework.Internal;

namespace NUnit.Framework.Tests.Internal
{
    public static class TestDataTests
    {
        [Test]
        public static void TestNameDefaultsToNull()
        {
            Assert.That(new TestCaseParameters(new object[] { 42 }).TestName, Is.Null);

            Assert.That(new TestFixtureParameters(42).TestName, Is.Null);
        }

        [Test]
        public static void ArgDisplayNamesDefaultsToNull()
        {
            Assert.That(new TestCaseParameters(new object[] { 42 }).ArgDisplayNames, Is.Null);

            Assert.That(new TestFixtureParameters(42).ArgDisplayNames, Is.Null);
        }

        [Test]
        public static void NameCanBeSet()
        {
            var caseData = new TestCaseData(42).SetName("Name");
            Assert.That(caseData.TestName, Is.EqualTo("Name"));

            var fixtureData = new TestFixtureData(42).SetName("Name");
            Assert.That(fixtureData.TestName, Is.EqualTo("Name"));
        }

        [Test]
        public static void ArgDisplayNamesCanBeSet()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames("42");
            Assert.That(caseData.ArgDisplayNames, Is.EqualTo(new[] { "42" }));

            var fixtureData = new TestFixtureData(42).SetArgDisplayNames("42");
            Assert.That(fixtureData.ArgDisplayNames, Is.EqualTo(new[] { "42" }));
        }

        [Test]
        public static void NullNameCanBeSet()
        {
            var caseData = new TestCaseData(42).SetName("Name");
            caseData.SetName(null);
            Assert.That(caseData.TestName, Is.Null);

            var fixtureData = new TestFixtureData(42).SetName("Name");
            fixtureData.SetName(null);
            Assert.That(fixtureData.TestName, Is.Null);
        }

        [Test]
        public static void ArgDisplayNamesCanBeReset()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames("42");
            caseData.SetArgDisplayNames(null);
            Assert.That(caseData.ArgDisplayNames, Is.Null);

            var fixtureData = new TestFixtureData(42).SetArgDisplayNames("42");
            fixtureData.SetArgDisplayNames(null);
            Assert.That(fixtureData.ArgDisplayNames, Is.Null);
        }

        [Test]
        public static void EmptyArgDisplayNamesAreAllowed()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames();
            Assert.That(caseData.ArgDisplayNames, Is.Empty);

            var fixtureData = new TestFixtureData(42).SetArgDisplayNames();
            Assert.That(fixtureData.ArgDisplayNames, Is.Empty);
        }

        [Test]
        public static void FewerArgDisplayNamesAreAllowed()
        {
            var caseData = new TestCaseData(4, 2).SetArgDisplayNames("42");
            Assert.That(caseData.ArgDisplayNames, Is.EqualTo(new[] { "42" }));

            var fixtureData = new TestFixtureData(4, 2).SetArgDisplayNames("42");
            Assert.That(fixtureData.ArgDisplayNames, Is.EqualTo(new[] { "42" }));
        }

        [Test]
        public static void ExtraArgDisplayNamesAreAllowed()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames("4", "2");
            Assert.That(caseData.ArgDisplayNames, Is.EqualTo(new[] { "4", "2" }));

            var fixtureData = new TestFixtureData(42).SetArgDisplayNames("4", "2");
            Assert.That(fixtureData.ArgDisplayNames, Is.EqualTo(new[] { "4", "2" }));
        }

        [Test]
        public static void SettingArgDisplayNamesAfterSettingNameThrowsInvalidOperationException()
        {
            var caseData = new TestCaseData(42).SetName("Name");
            Assert.That(() => caseData.SetArgDisplayNames("42"), Throws.InvalidOperationException);

            var fixtureData = new TestCaseData(42).SetName("Name");
            Assert.That(() => fixtureData.SetArgDisplayNames("42"), Throws.InvalidOperationException);
        }

        [Test]
        public static void ResettingNameAfterSettingArgDisplayNamesIsNoOp()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames("42");
            caseData.SetName(null);
            Assert.That(caseData.ArgDisplayNames, Is.EqualTo(new[] { "42" }));

            var fixtureData = new TestCaseData(42).SetArgDisplayNames("42");
            fixtureData.SetName(null);
            Assert.That(fixtureData.ArgDisplayNames, Is.EqualTo(new[] { "42" }));
        }

        [Test]
        public static void ResettingArgDisplayNamesAfterSettingNameIsNoOp()
        {
            var caseData = new TestCaseData(42).SetName("Name");
            caseData.SetArgDisplayNames(null);
            Assert.That(caseData.TestName, Is.EqualTo("Name"));

            var fixtureData = new TestCaseData(42).SetName("Name");
            fixtureData.SetArgDisplayNames(null);
            Assert.That(fixtureData.TestName, Is.EqualTo("Name"));
        }

        [Test]
        public static void SettingNameAfterSettingArgDisplayNamesThrowsInvalidOperationException()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames("42");
            Assert.That(() => caseData.SetName("Name"), Throws.InvalidOperationException);

            var fixtureData = new TestCaseData(42).SetArgDisplayNames("42");
            Assert.That(() => fixtureData.SetName("Name"), Throws.InvalidOperationException);
        }

        [Test]
        public static void ArgDisplayNamesCanBeSetWithObjects()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames(42);
            Assert.That(caseData.ArgDisplayNames, Is.EqualTo(new[] { "42" }));

            var fixtureData = new TestFixtureData(42).SetArgDisplayNames(42);
            Assert.That(fixtureData.ArgDisplayNames, Is.EqualTo(new[] { "42" }));
        }

        [Test]
        public static void ArgDisplayNamesCanBeSetWithMultipleObjects()
        {
            var caseData = new TestCaseData(42, "test", 3.14).SetArgDisplayNames(42, "test", 3.14);
            Assert.That(caseData.ArgDisplayNames, Has.Length.EqualTo(3));
            Assert.That(caseData.ArgDisplayNames![0], Is.EqualTo("42"));
            Assert.That(caseData.ArgDisplayNames![1], Is.EqualTo("\"test\""));
            Assert.That(caseData.ArgDisplayNames![2], Does.StartWith("3.14").And.EndsWith("d"));

            var fixtureData = new TestFixtureData(42, "test", 3.14).SetArgDisplayNames(42, "test", 3.14);
            Assert.That(fixtureData.ArgDisplayNames, Has.Length.EqualTo(3));
            Assert.That(fixtureData.ArgDisplayNames![0], Is.EqualTo("42"));
            Assert.That(fixtureData.ArgDisplayNames![1], Is.EqualTo("\"test\""));
            Assert.That(fixtureData.ArgDisplayNames![2], Does.StartWith("3.14").And.EndsWith("d"));
        }

        [Test]
        public static void ArgDisplayNamesWithObjectsFormatsCorrectly()
        {
            // Test with byte to verify special formatting (e.g., byte.MaxValue)
            var caseData = new TestCaseData(255).SetArgDisplayNames((byte)255);
            Assert.That(caseData.ArgDisplayNames, Has.Length.EqualTo(1));
            Assert.That(caseData.ArgDisplayNames![0], Is.EqualTo("255"));

            // Test with null value
            var caseDataWithNull = new TestCaseData((object?)null).SetArgDisplayNames((object?)null);
            Assert.That(caseDataWithNull.ArgDisplayNames, Is.EqualTo(new[] { "null" }));

            var fixtureData = new TestFixtureData(255).SetArgDisplayNames((byte)255);
            Assert.That(fixtureData.ArgDisplayNames, Has.Length.EqualTo(1));
            Assert.That(fixtureData.ArgDisplayNames![0], Is.EqualTo("255"));
        }

        [Test]
        public static void ArgDisplayNamesWithObjectsCanBeReset()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames(42);
            caseData.SetArgDisplayNames((object[]?)null);
            Assert.That(caseData.ArgDisplayNames, Is.Null);

            var fixtureData = new TestFixtureData(42).SetArgDisplayNames(42);
            fixtureData.SetArgDisplayNames((object[]?)null);
            Assert.That(fixtureData.ArgDisplayNames, Is.Null);
        }

        [Test]
        public static void SettingArgDisplayNamesWithObjectsAfterSettingNameThrowsInvalidOperationException()
        {
            var caseData = new TestCaseData(42).SetName("Name");
            Assert.That(() => caseData.SetArgDisplayNames(42), Throws.InvalidOperationException);

            var fixtureData = new TestCaseData(42).SetName("Name");
            Assert.That(() => fixtureData.SetArgDisplayNames(42), Throws.InvalidOperationException);
        }

        [Test]
        public static void SettingNameAfterSettingArgDisplayNamesWithObjectsThrowsInvalidOperationException()
        {
            var caseData = new TestCaseData(42).SetArgDisplayNames(42);
            Assert.That(() => caseData.SetName("Name"), Throws.InvalidOperationException);

            var fixtureData = new TestCaseData(42).SetArgDisplayNames(42);
            Assert.That(() => fixtureData.SetName("Name"), Throws.InvalidOperationException);
        }
    }
}
