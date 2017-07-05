// ***********************************************************************
// Copyright (c) 2015 Charlie Poole
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
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    public class TestParametersTests
    {
        public class CustomParameters : TestParameters
        {
            public CustomParameters(params object[] args)
                : base(args)
            {
                
            }
        }

        #region Align method
        private delegate void DelegateWithZeroParams();
        private delegate void DelegateWithSimpleParams(int a, int b);
        private delegate void DelegateWithSoleObjectsArray(object[] args);
        private delegate void DelegateWithParams(int a, int b, params byte[] args);
        private delegate void DelegateWithOptional(int a, int b = -1, string c = "default");
        private delegate void DelegateWithAll(DateTime a, int b = 1, params decimal[] args);

        private IMethodInfo GetDelegateWrapper<T>()
        {
            var method = typeof(T).GetMethod("Invoke");
            return new MethodWrapper(typeof(T), method);
        }

        /// <summary>
        /// If parameters provided for alignment is null we should throw
        /// <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Align_ParamsIsNull_ThrowException()
        {
            IAlignArguments parms = new CustomParameters(new object[] { 1, 2, 3 });

            var e = Assert.Throws<ArgumentNullException>(() => parms.Align(null as IParameterInfo[]));

            Assert.That(e.ParamName, Is.EqualTo("parameters"));
        }

        /// <summary>
        /// If method provided for alignment with its parameters is null we should
        /// throw <see cref="ArgumentNullException"/>.
        /// </summary>
        [Test]
        public void Align_MethodIsNull_ThrowException()
        {
            var parms = new CustomParameters(new object[] { 1, 2, 3 });

            var e = Assert.Throws<ArgumentNullException>(() => parms.Align(null as IMethodInfo));

            Assert.That(e.ParamName, Is.EqualTo("method"));
        }

        /// <summary>
        /// If arguments and params are equal and no changes expected, the
        /// list of arguments should be the same as it was before aligning.
        /// </summary>
        [Test]
        public void Align_ArgsMeetParams_ArgumentsAreUnchanged()
        {
            var parms = new CustomParameters(new object[] { 1, 2 });
            var initial = parms.Arguments;

            parms.Align(GetDelegateWrapper<DelegateWithSimpleParams>());

            Assert.That(parms.Arguments, Is.EqualTo(initial));
        }

        /// <summary>
        /// If we have zero parameters and non-zero number of arguments,
        /// <see cref="TargetParameterCountException"/> should be thrown.
        /// </summary>
        [Test]
        public void Align_ZeroParamsWithArguments_ThrowException()
        {
            var args = new object[] { 1, 2 };
            var parms = new CustomParameters(args);

            var e = Assert.Throws<TargetParameterCountException>(
                () => parms.Align(GetDelegateWrapper<DelegateWithZeroParams>())
            );

            var message = string.Format(TestParameters.ExceptionParameterCount, 0, args.Length);
            Assert.That(e.Message, Is.EqualTo(message));
        }

        /// <summary>
        /// If we have more arguments than parameters, including optional ones and
        /// excluding params arrays, <see cref="TargetParameterCountException"/>
        /// should be thrown.
        /// </summary>
        [Test]
        public void Align_ExcessiveArguments_ThrowException()
        {
            var args = new object[] { 1, 2, "temp", "extra" };
            var parms = new CustomParameters(args);

            var e = Assert.Throws<TargetParameterCountException>(
                () => parms.Align(GetDelegateWrapper<DelegateWithOptional>())
            );

            var message = string.Format(
                TestParameters.ExceptionParameterCount,
                typeof(DelegateWithOptional).GetMethod("Invoke").GetParameters().Length,
                args.Length
            );

            Assert.That(e.Message, Is.EqualTo(message));
        }

        /// <summary>
        /// If test method has a single parameter with object[] type and arguments do not
        /// meet this signature (more than 1 parameter or 1 parameter with non-object[] type)
        /// then arguments should be wrapped into object array that will be provided to
        /// the test method.
        /// </summary>
        [Test]
        public void Align_SoleParameterIsObjectArray_WrapArgumentsIntoArray()
        {
            var args = new object[] {1, 2};
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithSoleObjectsArray>());

            Assert.That(parms.Arguments, Has.Length.EqualTo(1));
            Assert.That(parms.Arguments, Has.Exactly(1).EqualTo(new object[] {1, 2}));
        }

        /// <summary>
        /// This exception for <see cref="Align_SoleParameterIsObjectArray_WrapArgumentsIntoArray"/>
        /// when the sole argument is object[]. So no wrapping into array expected.
        /// </summary>
        [Test]
        public void Align_SoleParameterAndArgIsObjectArray_NoArrayWrapping()
        {
            var args = new object[] {new object[] {1, 2}};
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithSoleObjectsArray>());

            Assert.That(parms.Arguments, Has.Length.EqualTo(1));
            Assert.That(parms.Arguments, Has.Exactly(1).EqualTo(new object[] { 1, 2 }));
        }

        /// <summary>
        /// When sole parameter is object[] and sole argument is null, we skip
        /// wrapping into array. This test is also good to check for the
        /// <see cref="NullReferenceException"/>.
        /// </summary>
        [Test]
        public void Align_SoleParameterAndArgIsNull_NoArrayWrapping()
        {
            var args = new object[] { null };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithSoleObjectsArray>());

            Assert.That(parms.Arguments, Has.Length.EqualTo(1));
            Assert.That(parms.Arguments, Has.Exactly(1).EqualTo(null));
        }

        /// <summary>
        /// When arguments for optional parameters are omitted, whe should provide
        /// default values. But if argument for optional parameter was provided in
        /// test case source it should be taken in count.
        /// </summary>
        [Test]
        public void Align_OptionalParameter_ReplacedWithDefaultValue()
        {
            // The first argument is mandatory, the second one is optional that we provide value
            // for. Argument for the third optional parameter is missing.
            var args = new object[] { 1, 999 };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithOptional>());

            var expected = new object[] { 1, 999, "default" };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }

        /// <summary>
        /// Just to ensure, that even if we have optional arguments, but lack of required
        /// ones, it still check this edge and throws <see cref="TargetParameterCountException"/>.
        /// </summary>
        [Test]
        public void Align_OptionalParametersNotEnough_ThrowException()
        {
            var args = new object[] { };
            var parms = new CustomParameters(args);

            var e = Assert.Throws<TargetParameterCountException>(
                () => parms.Align(GetDelegateWrapper<DelegateWithOptional>())
            );

            var message = string.Format(TestParameters.ExceptionParameterCount, 1, args.Length);
            Assert.That(e.Message, Is.EqualTo(message));
        }

        [Test]
        public void Align_ArrayParametersWithMultipleArgs_WrappedIntoArray()
        {
            var args = new object[] { 1, 2, 3, 4, 5 };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithParams>());

            var expected = new object[] { 1, 2, new byte[] { 3, 4, 5 } };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }

        [Test]
        public void Align_ArrayParametersWithSingleArg_WrappedIntoArray()
        {
            var args = new object[] { 1, 2, 3 };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithParams>());

            var expected = new object[] { 1, 2, new byte[] { 3 } };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }

        /// <summary>
        /// Instead of an arguments sequence, parameter array may be provided as
        /// array of the same type. In such circumstances, this argument should
        /// be delegated as it is.
        /// </summary>
        [Test]
        public void Align_ArraysParametersAsArray_DelegatedTheArray()
        {
            var args = new object[] { 1, 2, new byte[] { 3 } };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithParams>());

            var expected = new object[] { 1, 2, new byte[] { 3 } };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }

        /// <summary>
        /// Parameter array may have a null value. In this case it shouldn't wrap this
        /// null into array. Instead, it should be delegated as array argument like in
        /// <see cref="Align_ArraysParametersAsArray_DelegatedTheArray"/>,
        /// just with null value.
        /// </summary>
        [Test]
        public void Align_ArraysParametersAsNull_DelegatedAsNull()
        {
            var args = new object[] { 1, 2, null };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithParams>());

            var expected = new object[] { 1, 2, null };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }

        /// <summary>
        /// When there is no arguments for parameter array, the it should be interpreted
        /// as an empty array.
        /// </summary>
        [Test]
        public void Align_ArrayParametersNotExists_EmptyArray()
        {
            var args = new object[] { 1, 2 };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithParams>());

            var expected = new object[] { 1, 2, new string[] { } };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }

        /// <summary>
        /// If parameter array is of incompatible type and conversion is permitted, we should
        /// get converted array. For example, conversion int -> byte is allowed.
        /// Like in <see cref="Align_ForbiddenTypeConversion_ThrowException"/>.
        /// </summary>
        [Test]
        public void Align_ArrayParametersPermittedTypeConversion_ArrayIsConverted()
        {
            var args = new object[] { 1, 2, 3, 4, 5 };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithParams>());

            var expected = new object[] { 1, 2, new byte[] { 3, 4, 5 } };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }

        /// <summary>
        /// If parameter array is of incompatible type and conversion is not permitted,
        /// we should get an exception. For example, conversion string -> byte is not allowed.
        /// Like in <see cref="Align_ForbiddenTypeConversion_ThrowException"/>.
        /// </summary>
        [Test]
        public void Align_ArrayParametersForbiddenTypeConversion_ThrowException()
        {
            var args = new object[] { 1, 2, "3", "4", "5" };
            var parms = new CustomParameters(args);

            var e = Assert.Throws<ArgumentException>(
                () => parms.Align(GetDelegateWrapper<DelegateWithParams>())
            );

            Assert.That(e.ParamName, Is.EqualTo("args"));
        }

        /// <summary>
        /// If parameters and argument type mismatch, but conversion is permitted, then
        /// such transformation should be implemented.
        /// </summary>
        [Test]
        public void Align_PermittedTypeConversion_ArgumentIsConverted()
        {
            var args = new object[] { "2017-01-31" };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithAll>());

            var expected = new object[] { new DateTime(2017, 01, 31), 1, new string[0] };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }

        /// <summary>
        /// If parameters and argument type mismatch, and conversion is forbidden,
        /// then whe should throw an <see cref="ArgumentException"/>.
        /// </summary>
        [Test]
        public void Align_ForbiddenTypeConversion_ThrowException()
        {
            var args = new object[] { 20170131 };
            var parms = new CustomParameters(args);

            var e = Assert.Throws<ArgumentException>(
                () => parms.Align(GetDelegateWrapper<DelegateWithAll>())
            );

            Assert.That(e.ParamName, Is.EqualTo("a"));
        }

        /// <summary>
        /// We should ensure that if we mix many cases above (optional arguments, parameter
        /// arrays, type conversion), then all are handling and no conflicts between them
        /// occured.
        /// </summary>
        [Test]
        public void Align_MixedCases_ProperlyHandled()
        {
            var args = new object[] { "2017-01-31", 12, "1", 2, (double)3 };
            var parms = new CustomParameters(args);

            parms.Align(GetDelegateWrapper<DelegateWithAll>());

            var expected = new object[] { new DateTime(2017, 01, 31), 12, new decimal[] { 1, 2, 3 } };
            Assert.That(parms.Arguments, Is.EqualTo(expected));
        }
        #endregion
    }
}
