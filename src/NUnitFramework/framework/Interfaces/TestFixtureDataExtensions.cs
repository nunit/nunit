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
using System.Reflection;
using NUnit.Compatibility;
using NUnit.Framework.Internal;

namespace NUnit.Framework.Interfaces
{
    internal static class TestFixtureDataExtensions
    {
        public static Type[] GetTypeGenericArguments(this ITestFixtureData testFixtureData, Type genericTypeDefinition)
        {
            Guard.ArgumentNotNull(testFixtureData, nameof(testFixtureData));
            Guard.ArgumentNotNull(genericTypeDefinition, nameof(genericTypeDefinition));

            if (!genericTypeDefinition.GetTypeInfo().IsGenericTypeDefinition)
                throw new ArgumentException("A generic type definition must be specified.", nameof(genericTypeDefinition));

            object[] arguments = testFixtureData.Arguments;

            Type[] typeArgs = testFixtureData.TypeArgs;
            if (typeArgs == null || typeArgs.Length == 0)
            {
                int cnt = 0;
                foreach (object o in arguments)
                    if (o is Type) cnt++;
                    else break;

                typeArgs = new Type[cnt];
                for (int i = 0; i < cnt; i++)
                    typeArgs[i] = (Type)arguments[i];

                if (cnt > 0)
                {
                    object[] args = new object[arguments.Length - cnt];
                    for (int i = 0; i < args.Length; i++)
                        args[i] = arguments[cnt + i];

                    arguments = args;
                }
            }

            if (typeArgs.Length > 0 ||
                TypeHelper.TryDeduceTypeArgsFromConstructorArgs(genericTypeDefinition, arguments, out typeArgs))
            {
                return typeArgs;
            }

            throw new InvalidOperationException("The test fixture data cannot provide valid generic arguments for this type.");
        }
    }
}
