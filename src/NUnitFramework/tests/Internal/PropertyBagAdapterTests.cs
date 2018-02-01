using System;
using System.Collections.Generic;
using System.Text;
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
            Assert.AreEqual("val1", _adapter.Get("key"));
            Assert.AreEqual(42, _adapter.Get("meaningOfLife"));
            Assert.AreEqual(null, _adapter.Get("nonExistantKey"));
        }

        [Test]
        public void PropertyBagAdapter_ContainsKeys()
        {
            Assert.IsTrue(_adapter.ContainsKey("key"));
            Assert.IsTrue(_adapter.ContainsKey("meaningOfLife"));
            Assert.IsFalse(_adapter.ContainsKey("nonExistantKey"));
        }

        [Test]
        public void PropertyBagAdapter_IEnumerableAtKey()
        {
            var enumerable = _adapter["key"];

            var asList = new List<object>(enumerable);
            Assert.AreEqual(2, asList.Count);
            CollectionAssert.Contains(asList, "val1");
            CollectionAssert.Contains(asList, "val2");
        }

        [Test]
        public void PropertyBagAdapter_Count()
        {
            Assert.AreEqual(2, _adapter.Count("key"));
        }

        [Test]
        public void PropertyBagAdapter_Keys()
        {
            var actual = _adapter.Keys;
            var expected = new string[] { "key", "meaningOfLife" };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void PropertyBagAdapter_UpdatesWhenSourcePropertyBagUpdates()
        {
            _source.Add("newKey", "newVal");

            Assert.IsTrue(_adapter.ContainsKey("newKey"));
            Assert.AreEqual("newVal", _adapter.Get("newKey"));
            CollectionAssert.AreEquivalent(new string[] { "newVal" }, _adapter["newKey"]);
            Assert.AreEqual(1, _adapter.Count("newKey"));
            CollectionAssert.AreEquivalent(new string[] { "key", "meaningOfLife", "newKey" }, _adapter.Keys);
        }
    }
}
