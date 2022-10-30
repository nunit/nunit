// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Reflection;
using System.Text;
using System.Linq;
using NUnit.TestData.RandomAttributeTests;

namespace NUnit.Framework.Attributes
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
                    foreach (var child in result.Children)
                    {
                        msg.AppendFormat(" {0}: {1}\n", child.Name, child.ResultState);
                        msg.AppendFormat("{0}\n", child.Message);
                    }

                Assert.Fail(msg.ToString());
            }
        }

        class MethodNames : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                foreach (var method in typeof(RandomAttributeFixture).GetMethods())
                    if (method.HasAttribute<TestAttribute>(inherit: false))
                        yield return new TestCaseData(method.Name).SetName(method.Name);
            }
        }
    }
}
