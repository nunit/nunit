// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

// using System.Collections; (removed as unnecessary)
using System.IO;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Legacy.Tests
{
    [TestFixture]
    public class AssertWrapperSmokeTests
    {
        [Test]
        public void Core_Asserts_Work()
        {
            Assert.AreEqual(5, 5);
            Assert.AreNotEqual(5, 6);
            Assert.IsTrue(true);
            Assert.IsFalse(false);
            Assert.Null(null);
            Assert.NotNull(new object());
            Assert.IsEmpty(string.Empty);
            Assert.IsNotEmpty("x");
            Assert.IsNaN(double.NaN);
        }

        [Test]
        public void Numeric_Asserts_Work()
        {
            Assert.Zero(0);
            Assert.NotZero(1);
            Assert.Positive(1);
            Assert.Negative(-1);

            Assert.Zero(0.0);
            Assert.NotZero(1.23);
            Assert.Positive(2.5);
            Assert.Negative(-2.5);
        }

        [Test]
        public void Comparison_Asserts_Work()
        {
            Assert.Greater(2, 1);
            Assert.GreaterOrEqual(2, 2);
            Assert.Less(1, 2);
            Assert.LessOrEqual(2, 2);

            Assert.Greater(2.5m, 2.4m);
            Assert.LessOrEqual(2.0, 2.0);
        }

        private class Base
        {
        }

        private class Derived : Base
        {
        }

        [Test]
        public void Type_Asserts_Work()
        {
            var obj = new Derived();
            Assert.IsInstanceOf<Derived>(obj);
            Assert.IsNotInstanceOf<Base>(new object());
            Assert.IsAssignableFrom<Derived>(new Base());
            Assert.IsNotAssignableFrom<Base>(new Derived());
        }

        [Test]
        public void Collection_Asserts_Work()
        {
            var list = new[] { 1, 2, 3 };
            Assert.AllItemsAreNotNull(list);
            Assert.AllItemsAreUnique(list);
            Assert.DoesNotContain(new[] { 4 }, list);
            Assert.IsOrdered(list);
            Assert.AreEquivalent(new[] { 1, 2, 3 }, new[] { 3, 2, 1 });
            Assert.IsSubsetOf(new[] { 1, 2 }, new[] { 1, 2, 3 });
            Assert.IsSupersetOf(new[] { 1, 2, 3 }, new[] { 1, 2 });
        }

        [Test]
        public void String_Asserts_Work()
        {
            const string s = "Hello NUnit";
            Assert.StringContains("NUnit", s);
            Assert.DoesNotContain(new[] { 'x' }, s.ToCharArray());
            Assert.StartsWith("Hello", s);
            Assert.DoesNotStartWith("Goodbye", s);
            Assert.EndsWith("NUnit", s);
            Assert.DoesNotEndWith("JUnit", s);
            Assert.AreEqualIgnoringCase("hello nunit", s);
            Assert.IsMatch("^Hello\\s+NUnit$", s);
            Assert.DoesNotMatch("^Goodbye", s);
        }

        [Test]
        public void File_Asserts_Work()
        {
            using var dir = new TestDirectory();
            var path1 = Path.Combine(dir.Directory.FullName, "a.txt");
            var path2 = Path.Combine(dir.Directory.FullName, "b.txt");
            File.WriteAllText(path1, "abc");
            File.WriteAllText(path2, "abc");

            Assert.FileExists(path1);
            Assert.FileAreEqual(path1, path2);

            var path3 = Path.Combine(dir.Directory.FullName, "c.txt");
            Assert.FileDoesNotExist(path3);
        }

        [Test]
        public void Directory_Asserts_Work()
        {
            using var dir = new TestDirectory();
            Assert.DirectoryExists(dir.Directory.FullName);

            var missing = Path.Combine(dir.Directory.FullName, "missing-dir");
            Assert.DirectoryDoesNotExist(missing);
        }
    }
}
