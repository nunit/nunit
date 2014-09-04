// ***********************************************************************
// Copyright (c) 2014 Charlie Poole
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

#region Using Directives

using System;
using NUnit.Framework;

#endregion

namespace NUnit.Framework.Tests.Attributes
{
    public enum EnumValues
    {
        One,
        Two,
        Three,
        Four,
        Five
    }

    [TestFixture]
    public class ValuesAttributeEnumTests
    {
        private int _countEnums;
        private int _countBools;

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            _countEnums = 0;
            _countBools = 0;
        }

        [OneTimeTearDown]
        public void TestFixtureTeardown()
        {
            Assert.That(_countEnums, Is.EqualTo(5), "The TestEnumValues method should have been called 5 times");
            Assert.That(_countBools, Is.EqualTo(2), "The TestBoolValues method should have been called twice");
        }

        [Test]
        public void TestEnumValues([Values]EnumValues value)
        {
            _countEnums++;
        }

        [Test]
        public void TestBoolValues([Values]bool value)
        {
            _countBools++;
        }
    }
}