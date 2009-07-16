// ***********************************************************************
// Copyright (c) 2009 Charlie Poole
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

namespace NUnit.Framework.Constraints.Tests
{
    public class PropertyExistsTest : ConstraintTestBaseWithExceptionTests
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new PropertyExistsConstraint("Length");
            expectedDescription = "property Length";
            stringRepresentation = "<propertyexists Length>";
        }

        static object[] SuccessData = new object[] { new int[0], "hello", typeof(Array) };

        static object[] FailureData = new object[] { 42, new System.Collections.ArrayList(), typeof(Int32) };

        static string[] ActualValues = new string[] { "<System.Int32>", "<System.Collections.ArrayList>", "<System.Int32>" };

        static object[] InvalidData = new TestCaseData[] 
        { 
            new TestCaseData(null).Throws(typeof(ArgumentNullException))
        };
    }

    public class PropertyTest : ConstraintTestBaseWithExceptionTests
    {
        [SetUp]
        public void SetUp()
        {
            theConstraint = new PropertyConstraint("Length", new EqualConstraint(5));
            expectedDescription = "property Length equal to 5";
            stringRepresentation = "<property Length <equal 5>>";
        }

        static object[] SuccessData = new object[] { new int[5], "hello" };

        static object[] FailureData = new object[] { new int[3], "goodbye" };

        static string[] ActualValues = new string[] { "3", "7" };

        static object[] InvalidData = new object[] 
        { 
            new TestCaseData(null).Throws(typeof(ArgumentNullException)),
            new TestCaseData(42).Throws(typeof(ArgumentException)), 
            new TestCaseData(new System.Collections.ArrayList()).Throws(typeof(ArgumentException))
        };
    }
}
