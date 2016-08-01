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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework.Interfaces;

namespace NUnit.Framework.Internal
{
    static class NetCFExtensions
    {
        public static IMethodInfo MakeGenericMethodEx(this IMethodInfo method, object[] arguments)
        {
            var newMi = method.MethodInfo.MakeGenericMethodEx(arguments);
            if (newMi == null) return null;

            return new MethodWrapper(method.TypeInfo.Type, newMi);
        }

        public static MethodInfo MakeGenericMethodEx(this MethodInfo mi, object[] arguments)
        {
            return mi.MakeGenericMethodEx(arguments.Select(a => a == null ? typeof(object) : (a is Type ? typeof(Type) : a.GetType())).ToArray());
        }

        public static MethodInfo MakeGenericMethodEx(this MethodInfo mi, Type[] types)
        {
            if(!mi.ContainsGenericParameters)
                return mi;

            if(!mi.IsGenericMethodDefinition)
                return mi.MakeGenericMethod(types);

            var args = mi.GetGenericArguments();

            if(args.Length == types.Length)
                return mi.MakeGenericMethod(types);

            if(args.Length > types.Length)
                return null;

            var tai = new TypeArrayIterator(types, args.Length);

            foreach(var ta in tai)
            {
                var newMi = mi.MakeGenericMethod(ta);
                var pa = newMi.GetParameters();
                if(types.SequenceEqual(pa.Select(p => p.ParameterType)))
                    return newMi;
            }

            foreach(var ta in tai)
            {
                var newMi = mi.MakeGenericMethod(ta);
                var pa = newMi.GetParameters();
                if(types.Select((t,ix) => (Type)TypeHelper.BestCommonType(t, pa[ix].ParameterType)).SequenceEqual(pa.Select(p => p.ParameterType)))
                    return newMi;
            }

            return null;
        }

    }

    internal class TypeArrayIterator : IEnumerable<Type[]>
    {
        private Type[] _types;
        private int _neededLength;

        public TypeArrayIterator(Type[] types, int neededLength)
        {
            _types = types;
            _neededLength = neededLength;
        }

        #region IEnumerable<CType[]> Members

        public IEnumerator<Type[]> GetEnumerator()
        {
            var indices = new int[_neededLength];
            while(true)
            {
                var results = new List<Type> ();

                for(int i = 0; i < _neededLength; ++i)
                    results.Add(_types[indices[i]]);

                if(indices.Distinct().Count() == _neededLength)
                    yield return results.ToArray();

                for(int j = _neededLength - 1; j >= 0; --j)
                {
                    if(++indices[j] < _types.Length)
                        break;

                    if(j == 0)
                        yield break;

                    indices[j] = 0;
                }
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
#endif