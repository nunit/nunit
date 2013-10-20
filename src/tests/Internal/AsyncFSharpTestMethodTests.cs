using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Builders;
using NUnit.TestUtilities;

namespace NUnit.Framework.Internal
{
    [TestFixture]
    public class AsyncFSharpTestMethodTests
    {
        NUnitTestCaseBuilder _builder;
        object _testObject;

        [SetUp]
        public void Setup()
        {
            _builder = new NUnitTestCaseBuilder();
            _testObject = typeof(FsharpAsyncTestFixture);
        }

        public IEnumerable TestCases
        {
            get
            {
                //yield return new object[] { Method("async int success"), ResultState.Success, 0 };
                yield return new object[] { Method("async unit success"), ResultState.Success, 0 };
                //yield return new object[] { Method("async tailcall success"), ResultState.Success, 0 };
                //yield return new object[] { Method("async single true assert success"), ResultState.Success, 1 };
                //yield return new object[] { Method("async multiple asserts success"), ResultState.Success, 2 };
                //yield return new object[] { Method("calling into async code success"), ResultState.Success, 0 };

                //yield return new object[] { Method("when throwing exception fails"), ResultState.Failure, 0 };
                //yield return new object[] { Method("async single false assert failure"), ResultState.Failure, 1 };
            }
        }

        [Test]
        [TestCaseSource("TestCases")]
        public void RunTests(MethodInfo method, ResultState resultState, int assertionCount)
        {
            var test = _builder.BuildFrom(method);
            var result = TestBuilder.RunTest(test, _testObject);

            Assert.That(result.ResultState, Is.EqualTo(resultState),
                string.Format("Wrong result state; message: {0}", result.Message));
            Assert.That(result.AssertCount, Is.EqualTo(assertionCount), "Wrong assertion count");
        }

        static MethodInfo Method(string name)
        {
            return typeof(FsharpAsyncTestFixture).GetMethod(name);
        }
    }
}