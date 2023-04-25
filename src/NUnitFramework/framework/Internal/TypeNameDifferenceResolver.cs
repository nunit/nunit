// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt

using System;
using System.Collections.Generic;

namespace NUnit.Framework.Internal
{
    /// <summary>
    /// Used for resolving the type difference between objects.
    /// </summary>
    public class TypeNameDifferenceResolver
    {
        /// <summary>
        /// Gets the shortened type name difference between <paramref name="expected"/> and <paramref name="actual"/>.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="expectedTypeShortened">Output of the unique type name for the expected object.</param>
        /// <param name="actualTypeShortened">Output of the unique type name for actual object.</param>
        public void ResolveTypeNameDifference(object expected, object actual, out string expectedTypeShortened, out string actualTypeShortened)
        {
            ResolveTypeNameDifference(expected.GetType(), actual.GetType(), out expectedTypeShortened, out actualTypeShortened);
        }

        /// <summary>
        /// Gets the shortened type name difference between <paramref name="expected"/> and <paramref name="actual"/>.
        /// </summary>
        /// <param name="expected">The expected object <see cref="Type"/>.</param>
        /// <param name="actual">The actual object <see cref="Type"/>.</param>
        /// <param name="expectedTypeShortened">Output of the unique type name for the expected object.</param>
        /// <param name="actualTypeShortened">Output of the unique type name for actual object.</param>
        public void ResolveTypeNameDifference(Type expected, Type actual, out string expectedTypeShortened, out string actualTypeShortened)
        {
            if (IsTypeGeneric(expected) && IsTypeGeneric(actual))
            {
                GetShortenedGenericTypes(expected, actual, out var shortenedGenericTypeExpected, out var shortenedGenericTypeActual);

                GetShortenedGenericParams(expected, actual, out var shortenedParamsExpected, out var shortenedParamsActual);

                expectedTypeShortened = ReconstructGenericTypeName(shortenedGenericTypeExpected, shortenedParamsExpected);
                actualTypeShortened = ReconstructGenericTypeName(shortenedGenericTypeActual, shortenedParamsActual);
            }
            else if (IsTypeGeneric(expected) || IsTypeGeneric(actual))
            {
                expectedTypeShortened = FullyShortenTypeName(expected);
                actualTypeShortened = FullyShortenTypeName(actual);
            }
            else
            {
                ShortenTypeNames(expected, actual, out expectedTypeShortened, out actualTypeShortened);
            }
        }

        /// <summary>
        /// Obtain the shortened generic template parameters of the given <paramref name="expectedFullType"/> and <paramref name="actualFullType"/>,
        /// if they are generic.
        /// </summary>
        /// <param name="expectedFullType">The expected <see cref="Type"/>.</param>
        /// <param name="actualFullType">The actual <see cref="Type"/>.</param>
        /// <param name="shortenedParamsExpected">Shortened generic parameters of the expected <see cref="Type"/>.</param>
        /// <param name="shortenedParamsActual">Shortened generic parameters of the actual <see cref="Type"/>.</param>
        private void GetShortenedGenericParams(Type expectedFullType, Type actualFullType, out List<string> shortenedParamsExpected, out List<string> shortenedParamsActual)
        {
            List<Type> templateParamsExpected = new List<Type>(expectedFullType.GetGenericArguments());
            List<Type> templateParamsActual = new List<Type>(actualFullType.GetGenericArguments());

            shortenedParamsExpected = new List<string>();
            shortenedParamsActual = new List<string>();
            while ((templateParamsExpected.Count > 0) && (templateParamsActual.Count > 0))
            {
                ResolveTypeNameDifference(templateParamsExpected[0], templateParamsActual[0], out var shortenedExpected, out var shortenedActual);

                shortenedParamsExpected.Add(shortenedExpected);
                shortenedParamsActual.Add(shortenedActual);

                templateParamsExpected.RemoveAt(0);
                templateParamsActual.RemoveAt(0);
            }

            foreach (Type genericParamRemaining in templateParamsExpected)
            {
                shortenedParamsExpected.Add(FullyShortenTypeName(genericParamRemaining));
            }

            foreach (Type genericParamRemaining in templateParamsActual)
            {
                shortenedParamsActual.Add(FullyShortenTypeName(genericParamRemaining));
            }
        }

        /// <summary>
        /// Obtain a shortened name of the given <see cref="Type"/>.
        /// </summary>
        public string FullyShortenTypeName(Type genericType)
        {
            if (IsTypeGeneric(genericType))
            {
                string genericTypeDefinition = genericType.GetGenericTypeDefinition().Name;

                List<Type> genericParams = new List<Type>(genericType.GetGenericArguments());
                List<string> shortenedGenericParams = new List<string>();
                genericParams.ForEach(x => shortenedGenericParams.Add(FullyShortenTypeName(x)));

                return ReconstructGenericTypeName(genericTypeDefinition, shortenedGenericParams);
            }
            else
            {
                return genericType.Name;
            }
        }

        /// <summary>
        /// Shorten the given <see cref="Type"/> names by only including the relevant differing namespaces/types, if they differ.
        /// </summary>
        /// <param name="expectedType">The expected <see cref="Type"/>.</param>
        /// <param name="actualType">The actual <see cref="Type"/>.</param>
        /// <param name="expectedTypeShortened">The shortened expected <see cref="Type"/> name.</param>
        /// <param name="actualTypeShortened">The shortened actual <see cref="Type"/> name.</param>
        public void ShortenTypeNames(Type expectedType, Type actualType, out string expectedTypeShortened, out string actualTypeShortened)
        {
            string[] expectedOriginalType = expectedType.FullName().Split('.');
            string[] actualOriginalType = actualType.FullName().Split('.');

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
        /// Returns the fully qualified generic <see cref="Type"/> name of a given <see cref="Type"/>.
        /// </summary>
        public string GetGenericTypeName(Type type)
        {
            Guard.ArgumentNotNull(type, nameof(type));

            if (IsTypeGeneric(type))
            {
                Type generic = type.GetGenericTypeDefinition();
                return generic.FullName();
            }
            else
            {
                throw new ArgumentException($"The provided {nameof(type)} was not generic");
            }
        }

        /// <summary>
        /// Reconstruct a generic type name using the provided generic type name, and a
        /// <see cref="List{T}"/> of the template parameters.
        /// </summary>
        /// <param name="genericTypeName">The name of the generic type, including the number of template parameters expected.</param>
        /// <param name="templateParamNames">A <see cref="List{T}"/> of names of the template parameters of the provided generic type.</param>
        public string ReconstructGenericTypeName(string genericTypeName, List<string> templateParamNames)
        {
            return genericTypeName + "[" + string.Join(",", templateParamNames.ToArray()) + "]";
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
