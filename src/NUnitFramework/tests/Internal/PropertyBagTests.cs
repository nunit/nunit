// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Extensions;

namespace NUnit.Framework.Tests.Internal
{
    [TestFixture]
    public class PropertyBagTests
    {
        private PropertyBag? _bag;

        [SetUp]
        public void SetUp()
        {
            _bag = new PropertyBag();
            _bag.Add("Answer", 42);
            _bag.Add("Tag", "bug");
            _bag.Add("Tag", "easy");
        }

        [Test]
        public void IndexGetsListOfValues()
        {
            Assert.That(_bag["Answer"], Has.Count.EqualTo(1));
            Assert.That(_bag["Answer"], Contains.Item(42));

            Assert.That(_bag["Tag"], Has.Count.EqualTo(2));
            Assert.That(_bag["Tag"], Contains.Item("bug"));
            Assert.That(_bag["Tag"], Contains.Item("easy"));
        }

        [Test]
        public void IndexGetsEmptyListIfNameIsNotPresent()
        {
            Assert.That(_bag["Level"], Is.Empty);
        }

        [Test]
        public void IndexSetsListOfValues()
        {
            _bag["Zip"] = new[] { "junk", "more junk" };
            Assert.That(_bag["Zip"], Has.Count.EqualTo(2));
            Assert.That(_bag["Zip"], Contains.Item("junk"));
            Assert.That(_bag["Zip"], Contains.Item("more junk"));
        }

        [Test]
        public void AllKeysAreListed()
        {
            Assert.That(_bag.Keys, Has.Count.EqualTo(2));
            Assert.That(_bag.Keys, Has.Member("Answer"));
            Assert.That(_bag.Keys, Has.Member("Tag"));
        }

        [Test]
        public void ContainsKey()
        {
            Assert.That(_bag.ContainsKey("Answer"));
            Assert.That(_bag.ContainsKey("Tag"));
            Assert.That(_bag.ContainsKey("Target"), Is.False);
        }

        [Test]
        public void GetReturnsSingleValue()
        {
            Assert.That(_bag.Get("Answer"), Is.EqualTo(42));
            Assert.That(_bag.Get("Tag"), Is.EqualTo("bug"));
        }

        [Test]
        public void SetAddsNewSingleValue()
        {
            _bag.Set("Zip", "ZAP");
            Assert.That(_bag["Zip"], Has.Count.EqualTo(1));
            Assert.That(_bag["Zip"], Has.Member("ZAP"));
            Assert.That(_bag.Get("Zip"), Is.EqualTo("ZAP"));
        }

        [Test]
        public void SetReplacesOldValues()
        {
            _bag.Set("Tag", "ZAPPED");
            Assert.That(_bag["Tag"], Has.Count.EqualTo(1));
            Assert.That(_bag.Get("Tag"), Is.EqualTo("ZAPPED"));
        }

        [Test]
        public void XmlIsProducedCorrectly()
        {
            TNode topNode = _bag.ToXml(true);
            Assert.That(topNode.Name, Is.EqualTo("properties"));

            string[] props = new string[topNode.ChildNodes.Count];
            for (int i = 0; i < topNode.ChildNodes.Count; i++)
            {
                TNode node = topNode.ChildNodes[i];

                Assert.That(node.Name, Is.EqualTo("property"));

                props[i] = $"{node.Attributes["name"]}={node.Attributes["value"]}";
            }

            Assert.That(props,
                Is.EquivalentTo(new[] { "Answer=42", "Tag=bug", "Tag=easy" }));
        }

        [Test]
        public void TestNullPropertyValueIsntAdded()
        {
            Assert.Throws<ArgumentNullException>(() => _bag.Add("dontAddMe", null!));
            Assert.That(_bag.ContainsKey("dontAddMe"), Is.False);
        }

        [Test]
        public void TestTryGet()
        {
            Assert.That(_bag.TryGet("Answer", 37), Is.EqualTo(42));
            Assert.That(_bag.TryGet("WrongAnswer", 37), Is.EqualTo(37));

            Assert.That(_bag.TryGet("Tag", "none"), Is.EqualTo("bug"));
            Assert.That(_bag.TryGet("Target", "netstandard"), Is.EqualTo("netstandard"));
        }
    }
}
