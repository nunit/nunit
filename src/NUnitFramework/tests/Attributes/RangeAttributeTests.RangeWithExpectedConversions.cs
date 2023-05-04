// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NUnit.Framework.Attributes
{
    public partial class RangeAttributeTests
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
                var param = new StubParameterInfo(parameterType);

                if (ExpectedConversions.Contains(parameterType))
                {
                    // Important: do not use conversion procedure from NUnit framework. That is part of the system
                    // under test. This conversion procedure must be dead simple, no more than a wrapper over
                    // new byte[] { 1, 2, 3 }, new sbyte { 1, 2, 3 }, etc.
                    var convertedValues = valuesToConvert.Cast<object>().Select(value => Convert.ChangeType(value, parameterType));

                    Assert.That(Attribute.GetData(param),
                        Is.EqualTo(convertedValues).AsCollection.Using((IEqualityComparer)EqualityComparer<object>.Default));
                }
                else
                {
                    Assert.That(() => Attribute.GetData(param),
                        Throws.Exception.Message.Contains("cannot be passed to a parameter of type"));
                }
            }
        }
    }
}
