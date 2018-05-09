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
using System.Collections.Generic;
using System.Linq;
using NUnit.TestUtilities;

namespace NUnit.Framework.Attributes
{
    partial class RangeAttributeTests
    {
        public sealed class RangeWithExpectedConversions
        {
            public RangeAttribute Attribute { get; }
            public Type[] ExpectedConversions { get; }

            public RangeWithExpectedConversions(RangeAttribute attribute, Type[] expectedConversions)
            {
                Attribute = attribute;
                ExpectedConversions = expectedConversions;
            }

            public override string ToString()
            {
                return Attribute.ToString();
            }

            /// <summary>
            /// Helper method. If the parameter type is contained in <see cref="ExpectedConversions"/>,
            /// converts the specified values to that type and asserts that the attribute produces those
            /// exact values. Otherwise, asserts that a type coercion exception is thrown.
            /// </summary>
            public void AssertCoercionErrorOrMatchingSequence(Type parameterType, IEnumerable valuesToConvert)
            {
                var param = StubParameterInfo.OfType(parameterType);

                if (ExpectedConversions.Contains(parameterType))
                {
                    // Important: do not use conversion procedure from NUnit framework. That is part of the system
                    // under test. This conversion procedure must be dead simple, no more than a wrapper over
                    // new byte[] { 1, 2, 3 }, new sbyte { 1, 2, 3 }, etc.
                    var convertedValues = valuesToConvert.Cast<object>().Select(value => Convert.ChangeType(value, parameterType));

                    Assert.That(Attribute.GetData(null, param),
                        Is.EqualTo(convertedValues).AsCollection.Using((IEqualityComparer)EqualityComparer<object>.Default));
                }
                else
                {
                    Assert.That(() => Attribute.GetData(null, param),
                        Throws.Exception.Message.Contains("cannot be passed to a parameter of type"));
                }
            }
        }
    }
}
