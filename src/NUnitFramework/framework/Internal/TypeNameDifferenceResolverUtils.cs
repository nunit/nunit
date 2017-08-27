// ***********************************************************************
// Copyright (c) 2017 Charlie Poole, Rob Prouse
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
using System.Collections.Generic;
using NUnit.Compatibility;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Class to provide generic utilities to assist <see cref="TypeNameDifferenceResolver"/>.
    /// </summary>
    public class TypeNameDifferenceResolverUtils
    {
        /// <summary>
        /// Obtain a shortened name of the given <see cref="Type"/>.
        /// </summary>
        public string FullyShortenTypeName(Type GenericType)
        {
            if (IsTypeGeneric(GenericType))
            {
                string genericType = GenericType.GetGenericTypeDefinition().Name;

                List<Type> genericParams = GetGenericParams(GenericType);
                List<string> shortenedGenericParams = new List<string>();
                genericParams.ForEach(x => shortenedGenericParams.Add(FullyShortenTypeName(x)));

                return ReconstructGenericTypeName(genericType, shortenedGenericParams);
            }
            else
            {
                return GenericType.Name;
            }
        }

        /// <summary>
        /// Shorten the given <see cref="Type"/> names by only including the relevant differing namespaces/types, if they differ.
        /// </summary>
        /// <param name="expectedType">The expected <see cref="Type"/>.</param>
        /// <param name="actuallType">The actual <see cref="Type"/>.</param>
        /// <param name="expectedTypeShortened">The shortened expected <see cref="Type"/> name.</param>
        /// <param name="actualTypeShortened">The shortened actual <see cref="Type"/> name.</param>
        public void ShortenTypeNames(Type expectedType, Type actuallType, out string expectedTypeShortened, out string actualTypeShortened)
        {
            string[] expectedOriginalType = expectedType.FullName.Split('.');
            string[] actualOriginalType = actuallType.FullName.Split('.');

            bool diffDetected = false;
            int actualStart = 0, expectStart = 0;
            for (int expectLen = expectedOriginalType.Length - 1, actualLen = actualOriginalType.Length - 1;
                expectLen >= 0 && actualLen >= 0;
                expectLen--, actualLen--)
            {
                if (expectedOriginalType[expectLen] != actualOriginalType[actualLen])
                {
                    actualStart = actualLen;
                    expectStart = expectLen;
                    diffDetected = true;
                    break;
                }
            }

            if (diffDetected)
            {
                expectedTypeShortened = String.Join(".", expectedOriginalType, expectStart, expectedOriginalType.Length - expectStart);
                actualTypeShortened = String.Join(".", actualOriginalType, actualStart, actualOriginalType.Length - actualStart);
            }
            else
            {
                expectedTypeShortened = expectedOriginalType[expectedOriginalType.Length - 1];
                actualTypeShortened = actualOriginalType[actualOriginalType.Length - 1];
            }

        }

        /// <summary>
        /// Returns whether or not the <see cref="Type"/> is generic.
        /// </summary>
        public bool IsTypeGeneric(Type type)
        {
            Guard.ArgumentNotNull(type, nameof(type));

            return type.GetGenericArguments().Length > 0;
        }

        /// <summary>
        /// Get the generic parameters of a given <see cref="Type"/>.
        /// </summary>
        public List<Type> GetGenericParams(Type type)
        {
            Guard.ArgumentNotNull(type, nameof(type));

            return new List<Type>(type.GetGenericArguments());
        }

        /// <summary>
        /// Returns the fully qualified generic <see cref="Type"/> name of a given <see cref="Type"/>.
        /// </summary>
        public string GetGenericTypeName(Type type)
        {
            Guard.ArgumentNotNull(type, nameof(type));

            if (IsTypeGeneric(type))
            {
                Type generic = type.GetGenericTypeDefinition();
                return generic.FullName;
            }
            else
            {
                throw new ArgumentException($"The provided {nameof(type)} was not generic");
            }
        }

        /// <summary>
        /// Reconstruct a generic type name using the provided generic type name, and a
        /// <see cref="List"/> of the template parameters.
        /// </summary>
        /// <param name="GenericTypeName">The name of the generic type, including the number of template parameters expected.</param>
        /// <param name="TemplateParamNames">A <see cref="List"/> of names of the template parameters of the provided generic type.</param>
        public string ReconstructGenericTypeName(string GenericTypeName, List<string> TemplateParamNames)
        {
            return GenericTypeName + "[" + string.Join(",", TemplateParamNames.ToArray()) + "]";
        }

        /// <summary>
        /// Obtain the shortened generic <see cref="Type"/> names of the given expected and actual <see cref="Type"/>s.
        /// </summary>
        /// <param name="expected">The expected <see cref="Type"/>.</param>
        /// <param name="actual">The actual <see cref="Type"/>.</param>
        /// <param name="shortenedGenericNameExpected">The shortened expected generic name.</param>
        /// <param name="shortenedGenericNameActual">The shortened actual generic name.</param>
        public void GetShortenedGenericTypes(Type expected, Type actual, out string shortenedGenericNameExpected, out string shortenedGenericNameActual)
        {
            Type toplevelGenericExpected = expected.GetGenericTypeDefinition();
            Type toplevelGenericActual = actual.GetGenericTypeDefinition();
            ShortenTypeNames(
                toplevelGenericExpected,
                toplevelGenericActual,
                out shortenedGenericNameExpected,
                out shortenedGenericNameActual);
        }
    }
}