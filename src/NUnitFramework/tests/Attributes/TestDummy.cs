// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;


namespace NUnit.Framework.Tests.Attributes
{
    public class TestDummy : NUnit.Framework.Internal.Test
    {
        public TestDummy() : base("TestDummy") { }

        #region Overrides

        public string TestKind => "dummy-test";

        public override bool HasChildren => false;

        public override System.Collections.Generic.IList<ITest> Tests => Array.Empty<ITest>();

        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            throw new NotImplementedException();
        }

        public Framework.Internal.Commands.TestCommand MakeTestCommand()
        {
            throw new NotImplementedException();
        }

        public override TestResult MakeTestResult()
        {
            throw new NotImplementedException();
        }

        public override string XmlElementName => throw new NotImplementedException();

        public override object[] Arguments => Array.Empty<object>();

        #endregion
    }
}
