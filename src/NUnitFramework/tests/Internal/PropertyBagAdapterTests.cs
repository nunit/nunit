// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using static NUnit.Framework.TestContext;

namespace NUnit.Framework.Tests.Internal
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
            Assert.Multiple(() =>
            {
                Assert.That(_adapter.Get("key"), Is.EqualTo("val1"));
                Assert.That(_adapter.Get("meaningOfLife"), Is.EqualTo(42));
                Assert.That(_adapter.Get("nonExistantKey"), Is.EqualTo(null));
            });
        }

        [Test]
        public void PropertyBagAdapter_ContainsKeys()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_adapter.ContainsKey("key"), Is.True);
                Assert.That(_adapter.ContainsKey("meaningOfLife"), Is.True);
                Assert.That(_adapter.ContainsKey("nonExistantKey"), Is.False);
            });
        }

        [Test]
        public void PropertyBagAdapter_IEnumerableAtKey()
        {
            var enumerable = _adapter["key"];

            var asList = new List<object>(enumerable);
            Assert.That(asList, Has.Count.EqualTo(2));
            Assert.That(asList, Contains.Item("val1"));
            Assert.That(asList, Contains.Item("val2"));
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

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void PropertyBagAdapter_UpdatesWhenSourcePropertyBagUpdates()
        {
            _source.Add("newKey", "newVal");
            Assert.Multiple(() =>
            {
                Assert.That(_adapter.ContainsKey("newKey"), Is.True);
                Assert.That(_adapter.Get("newKey"), Is.EqualTo("newVal"));
                Assert.That(_adapter["newKey"], Is.EquivalentTo(new[] { "newVal" }));
                Assert.That(_adapter.Count("newKey"), Is.EqualTo(1));
                Assert.That(_adapter.Keys, Is.EquivalentTo(new[] { "key", "meaningOfLife", "newKey" }));
            });
        }
    }
}
