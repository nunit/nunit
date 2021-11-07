// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using NUnit.Framework;

namespace NUnit.TestData.CategoryAttributeData
{
    [TestFixture, InheritableCategory("MyCategory")]
    public abstract class AbstractBase { }
    
    [TestFixture, Category( "DataBase" )]
    public class FixtureWithCategories : AbstractBase
    {
        [Test, Category("Long")]
        public void Test1() { }

        [Test, Critical]
        public void Test2() { }

        [Test, Category("Top")]
        [TestCaseSource(nameof(Test3Data))]
        public void Test3(int x) { }

        [Test, Category("A-B"), Category("A,B"), Category("A!B"), Category("A+B")]
        public void TestValidSpecialChars() { }

#pragma warning disable 414
        private static TestCaseData[] Test3Data = new TestCaseData[] {
            new TestCaseData(5).SetCategory("Bottom")
        };
#pragma warning restore 414
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
    public class CriticalAttribute : CategoryAttribute { }
    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public class InheritableCategoryAttribute : CategoryAttribute
    {
        public InheritableCategoryAttribute(string name) : base(name) { }
    }
}
