// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using static NUnit.Framework.TestContext;

namespace NUnit.Framework.Internal
{
    public class PropertyBagAdapterTests
    {
        private IPropertyBag _source;
        private PropertyBagAdapter _adapter;

        [SetUp]
        public void SetUp()
        {
            _source = new PropertyBag();

            _source.Add("key", "val1");
            _source.Add("key", "val2");
            _source.Add("meaningOfLife", 42);

            _adapter = new PropertyBagAdapter(_source);
        }

        [Test]
        public void PropertyBagAdapter_Get_CanAccessKeysFromSourceIPropertyBag()
        {
            Assert.That(_adapter.Get("key"), Is.EqualTo("val1"));
            Assert.That(_adapter.Get("meaningOfLife"), Is.EqualTo(42));
            Assert.That(_adapter.Get("nonExistantKey"), Is.EqualTo(null));
        }

        [Test]
        public void PropertyBagAdapter_ContainsKeys()
        {
            Assert.That(_adapter.ContainsKey("key"), Is.True);
            Assert.That(_adapter.ContainsKey("meaningOfLife"), Is.True);
            Assert.That(_adapter.ContainsKey("nonExistantKey"), Is.False);
        }

        [Test]
        public void PropertyBagAdapter_IEnumerableAtKey()
        {
            var enumerable = _adapter["key"];

            var asList = new List<object>(enumerable);
            Assert.That(asList, Has.Count.EqualTo(2));
            CollectionAssert.Contains(asList, "val1");
            CollectionAssert.Contains(asList, "val2");
        }

        [Test]
        public void PropertyBagAdapter_Count()
        {
            Assert.That(_adapter.Count("key"), Is.EqualTo(2));
        }

        [Test]
        public void PropertyBagAdapter_Keys()
        {
            var actual = _adapter.Keys;
            var expected = new[] { "key", "meaningOfLife" };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void PropertyBagAdapter_UpdatesWhenSourcePropertyBagUpdates()
        {
            _source.Add("newKey", "newVal");

            Assert.That(_adapter.ContainsKey("newKey"), Is.True);
            Assert.That(_adapter.Get("newKey"), Is.EqualTo("newVal"));
            CollectionAssert.AreEquivalent(new[] { "newVal" }, _adapter["newKey"]);
            Assert.That(_adapter.Count("newKey"), Is.EqualTo(1));
            CollectionAssert.AreEquivalent(new[] { "key", "meaningOfLife", "newKey" }, _adapter.Keys);
        }
    }
}
