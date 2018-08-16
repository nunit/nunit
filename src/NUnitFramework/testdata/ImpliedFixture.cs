using System;
using NUnit.Framework.Interfaces;

namespace NUnit.TestData
{
    public static class ImpliedFixture
    {
        [AttributeImplyingFixture]
        public static void SomeMethod()
        {
        }

        private sealed class AttributeImplyingFixture : Attribute, IImplyFixture
        {
        }
    }
}
