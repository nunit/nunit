// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

namespace NUnit.Framework.Attributes
{
    public class TestDummy : Test
    {
        public TestDummy() : base("TestDummy") { }

        #region Overrides

        public string TestKind
        {
            get { return "dummy-test"; }
        }

        public override bool HasChildren
        {
            get
            {
                return false;
            }
        }

        public override System.Collections.Generic.IList<ITest> Tests
        {
            get
            {
                return Array.Empty<ITest>();
            }
        }

        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Internal.Commands.TestCommand MakeTestCommand()
        {
            throw new NotImplementedException();
        }

        public override TestResult MakeTestResult()
        {
            throw new NotImplementedException();
        }

        public override string XmlElementName
        {
            get { throw new NotImplementedException(); }
        }

        public override object[] Arguments
        {
            get
            {
                return Array.Empty<object>();
            }
        }

        #endregion
    }
}
