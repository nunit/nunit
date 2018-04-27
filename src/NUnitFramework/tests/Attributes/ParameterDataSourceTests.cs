// ***********************************************************************
// Copyright (c) 2018 Charlie Poole, Rob Prouse
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
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

        #region DataAttribute TestClass

        public enum ValuesEnum
        {
            ValueOne,
            ValueTwo
        }

        public class VariableValues : Attribute, IParameterDataSource
        {
            public int Count { get; set; }

            public IEnumerable GetData(Type fixtureType, ParameterInfo parameter)
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
