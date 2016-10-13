namespace nunit.integration.tests.Dsl
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    internal class XmlParser
    {
        public IEnumerable<IEnumerable<ItemValue>> Parse(string xmlFileName, string xPathExpression)
        {
            var doc = XDocument.Load(Path.GetFullPath(xmlFileName));
            return
                from item in doc.XPathSelectElements(xPathExpression)
                let attributes =
                    from attr in item.Attributes()
                    select new ItemValue(attr.Name.LocalName, attr.Value)
                select Enumerable.Repeat(new ItemValue(string.Empty, item.Value), 1).Concat(attributes);
        }
    }

    internal class ItemValue
    {
        public ItemValue(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public string Value { get; private set; }
    }
}
