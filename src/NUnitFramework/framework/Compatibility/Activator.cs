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

#if NETCF
using System;
using System.Reflection;

namespace NUnit.Framework.Compatibility
{
    /// <summary>
    /// Replacement for System.Activator used in the CF build
    /// </summary>
    public static class Activator
    {
        /// <summary>
        /// Create an instance of a type using the default constructor
        /// </summary>
        public static object CreateInstance(Type type)
        {
            return System.Activator.CreateInstance(type);
        }

        /// <summary>
        /// Create an instance of a type using a constructor taking arguments
        /// </summary>
        public static object CreateInstance(Type type, object[] args)
        {
            Type[] argTypes = new Type[args.Length];

            for (int i = 0; i < args.Length; i++)
                argTypes[i] = args[i] == null ? null : args[i].GetType();

            ConstructorInfo ci = type.GetConstructor(argTypes);

            if (ci == null)
                throw new MissingMethodException();

            return ci.Invoke(args);
        }
    }
}
#endif

