// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Xml;
using NUnit.Framework;

namespace NUnit.Common.Tests
{
    public class TestSelectionParserTests
    {
        private TestSelectionParser _parser;

        [SetUp]
        public void CreateParser()
        {
            _parser = new TestSelectionParser();
        }

        [TestCase("cat=Urgent", "<cat>Urgent</cat>")]
        [TestCase("cat==Urgent", "<cat>Urgent</cat>")]
        [TestCase("cat=='Urgent,+-!'", "<cat>Urgent,+-!</cat>")]
        [TestCase("cat!=Urgent", "<not><cat>Urgent</cat></not>")]
        [TestCase("cat!='Urgent,+-!'", "<not><cat>Urgent,+-!</cat></not>")]
        [TestCase("cat =~ Urgent", "<cat re='1'>Urgent</cat>")]
        [TestCase("cat =~ 'Urgent,+-!'", "<cat re='1'>Urgent,+-!</cat>")]
        [TestCase("cat =~ 'Urgent,\\+-!'", "<cat re='1'>Urgent,+-!</cat>")]
        [TestCase("cat =~ 'Urgent,\\\\+-!'", "<cat re='1'>Urgent,\\+-!</cat>")]
        [TestCase("cat !~ Urgent", "<not><cat re='1'>Urgent</cat></not>")]
        [TestCase("cat !~ 'Urgent,+-!'", "<not><cat re='1'>Urgent,+-!</cat></not>")]
        [TestCase("cat !~ 'Urgent,\\+-!'", "<not><cat re='1'>Urgent,+-!</cat></not>")]
        [TestCase("cat !~ 'Urgent,\\\\+-!'", "<not><cat re='1'>Urgent,\\+-!</cat></not>")]
        [TestCase("cat = Urgent || cat = High", "<or><cat>Urgent</cat><cat>High</cat></or>")]
        [TestCase("Priority == High", "<prop name='Priority'>High</prop>")]
        [TestCase("Priority != Urgent", "<not><prop name='Priority'>Urgent</prop></not>")]
        [TestCase("Author =~ Jones", "<prop name='Author' re='1'>Jones</prop>")]
        [TestCase("Author !~ Jones", "<not><prop name='Author' re='1'>Jones</prop></not>")]
        [TestCase("name='SomeTest'", "<name>SomeTest</name>")]
        [TestCase("method=TestMethod", "<method>TestMethod</method>")]
        [TestCase("method=Test1||method=Test2||method=Test3", "<or><method>Test1</method><method>Test2</method><method>Test3</method></or>")]
        [TestCase("test='My.Test.Fixture.Method(42)'", "<test>My.Test.Fixture.Method(42)</test>")]
        [TestCase("test='My.Test.Fixture.Method(\"xyz\")'", "<test>My.Test.Fixture.Method(&quot;xyz&quot;)</test>")]
        [TestCase("test='My.Test.Fixture.Method(\"abc\\'s\")'", "<test>My.Test.Fixture.Method(&quot;abc&apos;s&quot;)</test>")]
        [TestCase("test='My.Test.Fixture.Method(\"x&y&z\")'", "<test>My.Test.Fixture.Method(&quot;x&amp;y&amp;z&quot;)</test>")]
        [TestCase("test='My.Test.Fixture.Method(\"<xyz>\")'", "<test>My.Test.Fixture.Method(&quot;&lt;xyz&gt;&quot;)</test>")]
        [TestCase("cat==Urgent && test=='My.Tests'", "<and><cat>Urgent</cat><test>My.Tests</test></and>")]
        [TestCase("cat==Urgent and test=='My.Tests'", "<and><cat>Urgent</cat><test>My.Tests</test></and>")]
        [TestCase("cat==Urgent || test=='My.Tests'", "<or><cat>Urgent</cat><test>My.Tests</test></or>")]
        [TestCase("cat==Urgent or test=='My.Tests'", "<or><cat>Urgent</cat><test>My.Tests</test></or>")]
        [TestCase("cat==Urgent || test=='My.Tests' && cat == high", "<or><cat>Urgent</cat><and><test>My.Tests</test><cat>high</cat></and></or>")]
        [TestCase("cat==Urgent && test=='My.Tests' || cat == high", "<or><and><cat>Urgent</cat><test>My.Tests</test></and><cat>high</cat></or>")]
        [TestCase("cat==Urgent && (test=='My.Tests' || cat == high)", "<and><cat>Urgent</cat><or><test>My.Tests</test><cat>high</cat></or></and>")]
        [TestCase("cat==Urgent && !(test=='My.Tests' || cat == high)", "<and><cat>Urgent</cat><not><or><test>My.Tests</test><cat>high</cat></or></not></and>")]
        public void TestParser(string input, string output)
        {
            Assert.That(_parser.Parse(input), Is.EqualTo(output));

            XmlDocument doc = new XmlDocument();
            Assert.DoesNotThrow(() => doc.LoadXml(output));
        }

        [TestCase(null, typeof(ArgumentNullException))]
        [TestCase("", typeof(TestSelectionParserException))]
        [TestCase("   ", typeof(TestSelectionParserException))]
        [TestCase("  \t\t ", typeof(TestSelectionParserException))]
        public void TestParser_InvalidInput(string input, Type type)
        {
            Assert.That(() => _parser.Parse(input), Throws.TypeOf(type));
        }
    }
}
