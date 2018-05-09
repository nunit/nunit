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

using NUnit.Framework.Constraints;

namespace NUnit.Framework.Syntax
{
    public class SyntaxHelperExtensionTests
    {
        [Test]
        public void CanExtendIsHelper()
        {
            Assert.That(Is.Custom, Is.TypeOf<CustomConstraint>());
        }

        [Test]
        public void CanExtendHasHelper()
        {
            Assert.That(Has.Custom, Is.TypeOf<CustomConstraint>());
        }

        [Test]
        public void CanExtendDoesHelper()
        {
            Assert.That(Does.Custom, Is.TypeOf<CustomConstraint>());
        }

        [Test]
        public void CanExtendContainsHelper()
        {
            Assert.That(Contains.Custom, Is.TypeOf<CustomConstraint>());
        }

        #region Nested Syntax Helper Extensions

        public class Is : Framework.Is
        {
            public static CustomConstraint Custom { get { return new CustomConstraint(); } }
        }

        public class Has : Framework.Has
        {
            public static CustomConstraint Custom { get { return new CustomConstraint(); } }
        }

        public class Does : Framework.Does
        {
            public static CustomConstraint Custom { get { return new CustomConstraint(); } }
        }

        public class Contains : Framework.Contains
        {
            public static CustomConstraint Custom { get { return new CustomConstraint(); } }
        }

        #endregion

        #region Nested CustomConstraint used for tests

        public class CustomConstraint : Constraint
        {
            public override ConstraintResult ApplyTo<TActual>(TActual actual)
            {
                throw new System.NotImplementedException();
            }
        }
        #endregion
    }
}
