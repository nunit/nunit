// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Linq;
using System.Text;
using NUnit.Framework.Interfaces;
using NUnit.TestData.RandomAttributeTests;
using NUnit.Framework.Tests.TestUtilities;

namespace NUnit.Framework.Tests.Attributes
{
    public class RandomAttributeTests
    {

        [TestCaseSource(typeof(MethodNames))]
        public void CheckRandomResult(string methodName)
        {
            var result = TestBuilder.RunParameterizedMethodSuite(typeof(RandomAttributeFixture), methodName);
            Assert.That(result.Children.Count(), Is.EqualTo(RandomAttributeFixture.COUNT));

            if (result.ResultState != ResultState.Success)
            {
                var msg = new StringBuilder();
                msg.AppendFormat("Unexpected Result: {0}\n", result.ResultState);

                if (result.ResultState.Site == FailureSite.Child)
                {
                    foreach (var child in result.Children)
                    {
                        msg.AppendFormat(" {0}: {1}\n", child.Name, child.ResultState);
                        msg.AppendFormat("{0}\n", child.Message);
                    }
                }

                Assert.Fail(msg.ToString());
            }
        }

        private class MethodNames : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                foreach (var method in typeof(RandomAttributeFixture).GetMethods())
                {
                    if (method.HasAttribute<TestAttribute>(inherit: false))
                        yield return new TestCaseData(method.Name).SetName(method.Name);
                }
            }
        }
    }
}
