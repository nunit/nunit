// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Syntax
{
    [TestFixture]
    public class BinarySerializableTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<binaryserializable>";
            StaticSyntax = Is.BinarySerializable;
            BuilderSyntax = Builder().BinarySerializable;
        }
    }

    [TestFixture]
    public class XmlSerializableTest : SyntaxTest
    {
        [SetUp]
        public void SetUp()
        {
            ParseTree = "<xmlserializable>";
            StaticSyntax = Is.XmlSerializable;
            BuilderSyntax = Builder().XmlSerializable;
        }
    }
}
