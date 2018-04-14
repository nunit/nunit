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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Compatibility;

namespace NUnit.Framework.Interfaces
{
    /// <summary>
    /// Specifies a method declaration in the context of a certain point
    /// in the fixture’s inheritance hierarchy.
    /// </summary>
    public struct FixtureMethod : IEquatable<FixtureMethod>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureMethod"/> struct.
        /// </summary>
        public FixtureMethod(Type fixtureType, MethodInfo method)
        {
            Guard.ArgumentNotNull(fixtureType, nameof(fixtureType));
            Guard.ArgumentNotNull(method, nameof(method));

            if (!method.DeclaringType.GetTypeInfo().IsAssignableFrom(fixtureType))
                throw new ArgumentException("The specified fixture type does not contain the specified method.");

            FixtureType = fixtureType;
            Method = method;
        }

        /// <summary>
        /// May be more derived than <c>Method.DeclaringType</c>. Carries the
        /// information that used to be held in the deprecated <c>MethodInfo.ReflectedType</c>.
        /// </summary>
        public Type FixtureType { get; }

        /// <summary>
        /// The method declaration.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Displays the specified fixture and method.
        /// </summary>
        public override string ToString()
        {
            var parameters = Method.GetParameters();
            var parametersDisplay = new string[parameters.Length];

            for (var i = 0; i < parametersDisplay.Length; i++)
                parametersDisplay[i] = parameters[i].ToString();

            return $"{FixtureType.FullName}.{Method.Name}({string.Join(", ", parametersDisplay)})";
        }

        /// <summary>
        /// Indicates whether this instance and another instance are equal.
        /// </summary>
        public bool Equals(FixtureMethod other)
        {
            return FixtureType == other.FixtureType && Method == other.Method;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is FixtureMethod && Equals((FixtureMethod)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = 662238274;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(FixtureType);
            hashCode = hashCode * -1521134295 + EqualityComparer<MethodInfo>.Default.GetHashCode(Method);
            return hashCode;
        }
    }
}
