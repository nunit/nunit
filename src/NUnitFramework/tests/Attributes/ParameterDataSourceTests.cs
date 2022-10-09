// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System.Collections;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Attributes
{
    [TestFixture]
    public class ParameterDataSourceTests
    {
        [Test]
        public void TestWithParameterizedArgumentsShouldRun([VariableValues(Count = 3)] string context, [Values] ValuesEnum values)
        {
            Assert.Pass();
        }

        [Test]
        public void TestWithMultipleParameterizedArgumentsShouldRun([VariableValues(Count = 1)] string context)
        {
            Assert.Pass();
        }

        [Test]
        public void TestWithParameterizedArgumentsShouldNotRunWhenNoParametersPassedIn([VariableValues(Count = 0)] string context, [Values] ValuesEnum values)
        {
            Assert.Fail("Test Should Not Run By Design.");
        }

        [Test]
        public void TestWithSingleParameterizedArgumentsShouldNotRunWhenNoParametersPassedIn([VariableValues(Count = 0)] string context)
        {
            Assert.Fail("Test Should Not Run By Design.");
        }

        #region IParameterDataSource Attribute

        public enum ValuesEnum
        {
            ValueOne,
            ValueTwo
        }

        public class VariableValues : Attribute, IParameterDataSource
        {
            public int Count { get; set; }

            public IEnumerable GetData(IParameterInfo parameter)
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return i.ToString();
                }
            }
        }

        #endregion
    }
}
