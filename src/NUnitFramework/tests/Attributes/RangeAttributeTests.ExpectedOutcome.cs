// ***********************************************************************
// Copyright (c) 2009-2015 Charlie Poole, Rob Prouse
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
using System.Reflection;
using NUnit.Framework.Constraints;

#if NETCOREAPP1_1
using System.Linq;
#endif

namespace NUnit.Framework.Attributes
{
    partial class RangeAttributeTests
    {
        public struct ExpectedOutcome
        {
            private readonly IEnumerable _values;

            private ExpectedOutcome(IEnumerable values)
            {
                _values = values;
            }

            public override string ToString()
            {
                return _values == null
                    ? "Expected: coercion error"
                    : "Expected: values " + MsgUtils.FormatCollection(_values, 0, int.MaxValue);
            }

            public static ExpectedOutcome CoercionError => new ExpectedOutcome(null);

            public static ExpectedOutcome Values(IEnumerable expected)
            {
                Guard.ArgumentNotNull(expected, nameof(expected));
                return new ExpectedOutcome(expected);
            }

            public void Assert(RangeAttribute attribute, Type parameterType)
            {
                var param = GetStubParameter(parameterType);

                if (_values != null)
                {
                    Framework.Assert.That(attribute.GetData(null, param),
                        Is.EqualTo(_values).AsCollection.Using((IEqualityComparer)EqualityComparer<object>.Default));
                }
                else
                {
                    Framework.Assert.That(() => attribute.GetData(null, param),
                        Throws.Exception.Message.Contains("cannot be passed to a parameter of type"));
                }
            }

            private static ParameterInfo GetStubParameter(Type parameterType)
            {
                return typeof(ExpectedOutcome)
                       .GetMethod(nameof(DummyMethod), BindingFlags.Static | BindingFlags.NonPublic)
                       .MakeGenericMethod(parameterType)
                       .GetParameters()[0];
            }

            private static void DummyMethod<T>(T parameter) { }
        }
    }
}
